namespace EnhancedReactionMachine
{
    public class EnhancedReactionController : IController
    {

        public readonly double TIME_INTERVAL_PER_TICK = 0.01;
        public readonly double MAX_REACTION_TIME = 2.0;
        public readonly double MAX_DISPLAY_RESULT_TIME = 3.0;
        public readonly double MAX_DISPLAY_AVG_RESULT_TIME = 5.0;
        public readonly double MAX_WAITING_FOR_GO_TIME = 10.0;
        public readonly int MAX_GAME_NUM = 3;

        private IGui gui;
        private IRandom rng;
        private State curState;
        private int delay;
        public int Delay
        {
            get { return delay; }
        }
        private int tick;
        public int Ticks
        {
            get { return tick; }
            set { tick = value; }
        }
        private double reactionTime;
        public double ReactionTime
        {
            get { return reactionTime; }
            set { reactionTime = value; }
        }

        private double displayTime;
        public double DisplayTime
        {
            get { return displayTime; }
            set { displayTime = value; }
        }

        private double waitingForGoTime;
        public double WaitingForGoTime
        {
            get { return waitingForGoTime; }
            set { waitingForGoTime = value; }
        }

        private int gameNum;
        public int GameNum
        {
            get { return gameNum; }
            set { gameNum = value; }
        }

        private bool gameAborted;
        public bool GameAborted
        {
            get { return gameAborted; }
            set { gameAborted = value; }
        }

        private bool timeOut;
        public bool TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        public List<double> reactionTimeList = new List<double>(3);

        //Connect controller to gui
        //(This method will be called before any other methods)
        public void Connect(IGui gui, IRandom rng)
        {
            this.gui = gui;
            this.rng = rng;

            Init();
        }

        //Called to initialise the controller
        public void Init()
        {

            delay = 0;
            tick = 0;
            reactionTime = 0.0;
            displayTime = 0.01;
            waitingForGoTime = 0;
            gameNum = 0;
            gameAborted = false;
            timeOut = false;
            reactionTimeList.Clear();

            ChangeState(new WaitingForCoin());

        }

        //Called whenever a coin is inserted into the machine
        public void CoinInserted()
        {
            curState.CoinInserted();
        }

        //Called whenever the go/stop button is pressed
        public void GoStopPressed()
        {
            curState.GoStopPressed();
        }

        //Called to deliver a TICK to the controller
        public void Tick()
        {
            if (curState != null)
            {

                curState.Tick();
            }
        }

        public void ChangeState(State newState)
        {
            curState = newState;
            curState.SetContext(this);
        }

        public void SetDisplayMessage(string msg)
        {
            gui.SetDisplay(msg);
        }

        public void StartWaitingForRandomTime()
        {
            timeOut = false;
            delay = rng.GetRandom(100, 250);
            gui.Reset();
            SetDisplayMessage("Wait...");
            ChangeState(new WaitingForRandomTime());
        }

        public void DisplayResult()
        {
            reactionTimeList.Add(reactionTime);
            //SetDisplayMessage($"Your reaction time: {reactionTime:F2} seconds");
        }

        public void DisplayAverageResult()
        {
            double avgReactionTime = 0;
            for (int i = 0; i < 3; i++)
            {
                avgReactionTime += reactionTimeList[i];
            }

            avgReactionTime /= 3;

            SetDisplayMessage($"Your avg reaction time: {avgReactionTime:F2} seconds");
        }

        public double UpdateTick(double targetTimer, double maxTimerTime, double increment, Action action)
        {
            targetTimer += increment;

            if (targetTimer >= maxTimerTime)
            {
                action();
                return 0;
            }

            return targetTimer;
        }

        public int UpdateTick(double targetTimer, double maxTimerTime, double increment, Action action1, Action action2)
        {
            targetTimer += increment;

            if (targetTimer >= maxTimerTime)
            {
                action1();
                return 0;
            }
            else
            {
                action2();
            }

            return (int)targetTimer;
        }

        public void BackToInsertCoin()
        {
            /*delay = 0;
            tick = 0;
            reactionTime = 0;
            displayTime = 0;
            waitingForGoTime = 0;
            gameNum = 0;
            ChangeState(new WaitingForCoin());
            */

            gui.Reset();
            Init();

            SetDisplayMessage("Insert coin");
        }

    }

    public interface ICoinInserted
    {
        void CoinInserted();
    }

    public interface IGoStopPressed
    {
        void GoStopPressed();
    }

    public interface ITick
    {
        void Tick();
    }

    public abstract class State : ICoinInserted, IGoStopPressed, ITick
    {

        protected EnhancedReactionController controller;

        public void SetContext(EnhancedReactionController controller)
        {
            this.controller = controller;
        }

        public virtual void CoinInserted() { }

        public virtual void GoStopPressed() { }

        public virtual void Tick() { }

    }

    public class WaitingForCoin : State
    {

        public override void CoinInserted()
        {
            controller.WaitingForGoTime = 0;
            controller.SetDisplayMessage("Press Go!");
            controller.ChangeState(new WaitingForGo());
        }

    }

    public class WaitingForGo : State
    {
        public override void GoStopPressed()
        {
            controller.StartWaitingForRandomTime();
        }

        public override void Tick()
        {
            //to detect if player is idle for 10 seconds without pressing go or stop
            //if yes, then the game ends
            controller.WaitingForGoTime = controller.UpdateTick(controller.WaitingForGoTime, controller.MAX_WAITING_FOR_GO_TIME, controller.TIME_INTERVAL_PER_TICK, () => {
                controller.GameAborted = true;
                controller.SetDisplayMessage("Game Aborted! Insert another coin!");
                controller.ChangeState(new DisplayingResult());
            });
        }

    }

    public class WaitingForRandomTime : State
    {
        public override void GoStopPressed()
        {
            controller.GameAborted = true;
            controller.SetDisplayMessage("Cheated! Insert another coin!");
            controller.ChangeState(new DisplayingResult());
        }

        public override void Tick()
        {
            controller.Ticks = (int)controller.UpdateTick(controller.Ticks, controller.Delay, 1, () => {
                controller.ReactionTime = 0;
                controller.DisplayTime = 0.01;
                controller.SetDisplayMessage($"{controller.ReactionTime:F2}");
                controller.ChangeState(new CountingReactionTime());
            });
        }

    }

    public class CountingReactionTime : State
    {
        public override void GoStopPressed()
        {
            controller.DisplayResult();
            controller.ChangeState(new DisplayingResult());
        }

        public override void Tick()
        {
            controller.ReactionTime = controller.UpdateTick(controller.ReactionTime, controller.MAX_REACTION_TIME, controller.TIME_INTERVAL_PER_TICK, () => {
                controller.TimeOut = true;
                GoStopPressed();
            });

            if (!controller.TimeOut)
            {

                controller.SetDisplayMessage($"{controller.ReactionTime:F2}");
            }
            else
            {
                controller.SetDisplayMessage("1.99");
            }
        }

    }

    public class DisplayingResult : State
    {

        public override void GoStopPressed()
        {
            if (!controller.GameAborted)
            {
                //go to counting game state
                controller.SetDisplayMessage($"Game {controller.GameNum + 1}/3 ends!");
                controller.ChangeState(new CountingGameState());
            }
            else
            {
                controller.BackToInsertCoin();
            }
        }

        public override void Tick()
        {
            controller.DisplayTime = controller.UpdateTick(controller.DisplayTime, controller.MAX_DISPLAY_RESULT_TIME, controller.TIME_INTERVAL_PER_TICK, () => {
                GoStopPressed();
            });
        }

    }

    public class CountingGameState : State
    {
        //put the checking logic in the tick function because it represent the game update method
        public override void Tick()
        {
            controller.GameNum = controller.UpdateTick(controller.GameNum, controller.MAX_GAME_NUM, 1, () => {
                //completed 3 games
                controller.DisplayTime = 0.01;
                controller.DisplayAverageResult();
                controller.ChangeState(new DisplayingAverageResult());
            }, () => {
                //not yet complete 3 games
                controller.StartWaitingForRandomTime();
            });
        }

    }

    public class DisplayingAverageResult : State
    {

        public override void GoStopPressed()
        {
            controller.BackToInsertCoin();
        }

        public override void Tick()
        {
            controller.DisplayTime = controller.UpdateTick(controller.DisplayTime, controller.MAX_DISPLAY_AVG_RESULT_TIME, controller.TIME_INTERVAL_PER_TICK, () => {
                GoStopPressed();
            });
        }

    }

}