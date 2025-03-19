using System;

namespace EnhancedReactionMachine
{
    class Tester
    {
        private static IController controller;
        private static IGui gui;
        private static string displayText;
        private static int randomNumber;
        private static int passed = 0;

        static void Main(string[] args)
        {
            // run simple test
            SimpleTest();
            Console.WriteLine("\n=====================================\nSummary: {0} tests passed out of 71", passed);
            Console.ReadKey();
        }

        private static void SimpleTest()
        {
            //Construct a ReactionController
            controller = new EnhancedReactionController();
            gui = new DummyGui();

            //Connect them to each other
            gui.Connect(controller);
            controller.Connect(gui, new RndGenerator());

            //Reset the components()
            gui.Init();

            //Test the EnhancedReactionController

            //Test 3 Games
            // *********************************new game/1st game
            //IDLE / WaitingForCoin 
            DoReset("1", controller, "Insert coin");
            DoGoStop("2", controller, "Insert coin"); //no action
            DoTicks("3", controller, 1, "Insert coin"); //no action

            //coinInserted
            DoInsertCoin("4", controller, "Press GO!");

            //READY / WaitingForGo
            DoTicks("5", controller, 1, "Press GO!");
            DoInsertCoin("6", controller, "Press GO!"); //no action

            //goStop
            randomNumber = 117; //delay
            DoGoStop("7", controller, "Wait...");

            //WAIT tick(s) / WaitingForRandomTime
            DoTicks("8", controller, randomNumber - 1, "Wait...");

            //RUN tick(s) / CountingReactionTime
            DoTicks("9", controller, 1, "0.00");
            DoTicks("10", controller, 1, "0.01");
            DoTicks("11", controller, 11, "0.12");
            DoTicks("12", controller, 111, "1.23");

            //goStop
            DoGoStop("13", controller, "Your reaction time: 1.23 seconds");

            //STOP tick(s) / DisplayingResult
            //display averageResult for 3seconds
            DoTicks("14", controller, 299, "Your reaction time: 1.23 seconds"); 


            // *********************************2nd game
            //tick / CountingGameState
            DoTicks("15", controller, 1, "Game 1/3 ends!");

            randomNumber = 167;
            DoTicks("16", controller, 1, "Wait...");

            //WAIT tick(s) / WaitingForRandomTime
            DoTicks("17", controller, randomNumber - 1, "Wait...");

            //RUN tick(s) / CountingReactionTime
            DoTicks("18", controller, 1, "0.00");
            DoTicks("19", controller, 1, "0.01");
            DoTicks("20", controller, 12, "0.13");
            DoTicks("21", controller, 112, "1.25");

            //goStop
            DoGoStop("22", controller, "Your reaction time: 1.25 seconds");

            //STOP tick(s) / DisplayingResult
            //display averageResult for 3seconds
            DoTicks("23", controller, 299, "Your reaction time: 1.25 seconds");


            // *********************************3nd game
            //tick / CountingGameState
            DoTicks("24", controller, 1, "Game 2/3 ends!");

            randomNumber = 123;
            DoTicks("25", controller, 1, "Wait...");

            //WAIT tick(s) / WaitingForRandomTime
            DoTicks("26", controller, randomNumber - 1, "Wait...");

            //RUN tick(s) / CountingReactionTime
            DoTicks("27", controller, 1, "0.00");
            DoTicks("28", controller, 1, "0.01");
            DoTicks("29", controller, 13, "0.14");
            DoTicks("30", controller, 113, "1.27");

            //goStop
            DoGoStop("31", controller, "Your reaction time: 1.27 seconds");

            //STOP tick(s) / DisplayingResult
            //display averageResult for 3seconds
            DoTicks("32", controller, 299, "Your reaction time: 1.27 seconds");


            // *********************************show average reaction time
            //tick / CountingGameState
            DoTicks("33", controller, 1, "Game 3/3 ends!");

            //DisplayingAverageResult
            DoTicks("34", controller, 1, "Your avg reaction time: 1.25 seconds");

            //display averageResult for 5seconds
            DoTicks("35", controller, 499, "Your avg reaction time: 1.25 seconds");

            // *********************************new game
            //IDLE init
            gui.Init();
            DoReset("36", controller, "Insert coin");



            //Test Idle for 10seconds at WaitingForGo state
            // *********************************new game
            //IDLE coinInserted
            DoInsertCoin("37", controller, "Press GO!");

            //READY / WaitingForGo
            DoTicks("38", controller, 1, "Press GO!");
            DoTicks("39", controller, 999, "Press GO!");

            //Game aborted / DisplayingResult
            DoTicks("40", controller, 1, "Game Aborted! Insert another coin!");

            //WAIT tick(s) goStop
            DoTicks("41", controller, 1, "Game Aborted! Insert another coin!");
            DoGoStop("42", controller, "Insert coin");



            //Test cheating
            // *********************************new game
            //IDLE coinInserted
            DoInsertCoin("43", controller, "Press GO!");

            //READY goStop / WaitingForGo
            randomNumber = 167;
            DoGoStop("44", controller, "Wait...");

            //WAIT tick(s) goStop / WaitingForRandomTime
            DoTicks("45", controller, randomNumber - 1, "Wait...");
            DoGoStop("46", controller, "Cheated! Insert another coin!");

            //DisplayingResult
            DoGoStop("47", controller, "Insert coin");




            //Test 3 Games (all 3 games have timeout reaction time eg 2seconds)
            // *********************************new game/1st game
            //IDLE / WaitingForCoin 
            gui.Init();
            DoReset("48", controller, "Insert coin");

            //coinInserted
            DoInsertCoin("49", controller, "Press GO!");

            //READY / WaitingForGo
            DoTicks("50", controller, 1, "Press GO!");

            //goStop
            randomNumber = 117; //delay
            DoGoStop("51", controller, "Wait...");

            //WAIT tick(s) / WaitingForRandomTime
            DoTicks("52", controller, randomNumber - 1, "Wait...");

            //RUN tick(s) / CountingReactionTime
            DoTicks("53", controller, 1, "0.00");
            DoTicks("54", controller, 1, "0.01");
            DoTicks("55", controller, 199, "0.00"); //reset to 0 after storing into the reactionTimeList

            //goStop / DisplayingResult
            DoGoStop("56", controller, "Game 1/3 ends!");

            //WAIT
            randomNumber = 127;
            DoTicks("57", controller, 1, "Wait...");

            //WAIT tick(s) / WaitingForRandomTime
            DoTicks("58", controller, randomNumber - 1, "Wait...");

            //RUN tick(s) / CountingReactionTime
            DoTicks("59", controller, 1, "0.00");
            DoTicks("60", controller, 1, "0.01");
            DoTicks("61", controller, 199, "0.00"); //reset to 0 after storing into the reactionTimeList

            //goStop / DisplayingResult
            DoGoStop("62", controller, "Game 2/3 ends!");

            //WAIT
            randomNumber = 137;
            DoTicks("63", controller, 1, "Wait...");

            //WAIT tick(s) / WaitingForRandomTime
            DoTicks("64", controller, randomNumber - 1, "Wait...");

            //RUN tick(s) / CountingReactionTime
            DoTicks("65", controller, 1, "0.00");
            DoTicks("66", controller, 1, "0.01");
            DoTicks("67", controller, 199, "0.00"); //reset to 0 after storing into the reactionTimeList

            //goStop / DisplayingResult
            DoGoStop("68", controller, "Game 3/3 ends!");


            //DisplayingAverageResult
            DoTicks("69", controller, 1, "Your avg reaction time: 1.99 seconds");
            DoGoStop("70", controller, "Insert coin"); //new game

            // *********************************new game
            //IDLE init
            gui.Init();
            DoReset("71", controller, "Insert coin");

        }

        private static void DoReset(string ch, IController controller, string msg)
        {
            try
            {
                controller.Init();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoGoStop(string ch, IController controller, string msg)
        {
            try
            {
                controller.GoStopPressed();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoInsertCoin(string ch, IController controller, string msg)
        {
            try
            {
                controller.CoinInserted();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void DoTicks(string ch, IController controller, int n, string msg)
        {
            try
            {
                for (int t = 0; t < n; t++) controller.Tick();
                GetMessage(ch, msg);
            }
            catch (Exception exception)
            {
                Console.WriteLine("test {0}: failed with exception {1})", ch, msg, exception.Message);
            }
        }

        private static void GetMessage(string ch, string msg)
        {
            if (msg.ToLower() == displayText.ToLower())
            {
                Console.WriteLine("test {0}: passed successfully", ch);
                passed++;
            }
            else
                Console.WriteLine("test {0}: failed with message ( expected {1} | received {2})", ch, msg, displayText);
        }

        private class DummyGui : IGui
        {

            private IController controller;

            public void Connect(IController controller)
            {
                this.controller = controller;
            }

            public void Init()
            {
                displayText = "Insert coin";
            }

            public void Reset() { 
            
            }

            public void SetDisplay(string msg)
            {
                displayText = msg;
            }
        }

        private class RndGenerator : IRandom
        {
            public int GetRandom(int from, int to)
            {
                return randomNumber;
            }
        }

    }

}
