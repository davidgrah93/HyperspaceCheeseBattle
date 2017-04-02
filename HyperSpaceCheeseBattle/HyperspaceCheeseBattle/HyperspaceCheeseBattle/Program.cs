using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    class Program
    {
        enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Win
        }

        static Direction[,] board = new Direction[,] 
            { // 1=up 2=left 3=down 4=right 0=win
                {Direction.Up,Direction.Up,Direction.Up,Direction.Up,Direction.Up,Direction.Up,Direction.Up,Direction.Up}, // row 0
                {Direction.Right,Direction.Right,Direction.Up,Direction.Down,Direction.Up,Direction.Up,Direction.Left,Direction.Left}, // row 1
                {Direction.Right,Direction.Right,Direction.Up,Direction.Left,Direction.Left,Direction.Left,Direction.Left,Direction.Left}, // row 2
                {Direction.Right,Direction.Right,Direction.Up,Direction.Right,Direction.Up,Direction.Up,Direction.Left,Direction.Left}, // row 3
                {Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Up,Direction.Up,Direction.Left,Direction.Left}, // row 4
                {Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Up,Direction.Up,Direction.Left,Direction.Left}, // row 5
                {Direction.Right,Direction.Right,Direction.Up,Direction.Down,Direction.Up,Direction.Left,Direction.Left,Direction.Left}, // row 6
                {Direction.Down,Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Down,Direction.Win}, // row 7
            };

        struct Player
        {
            public string Name;
            public int X;
            public int Y;
        }

        static Player[] players;
        static int NumberOfPlayers;
        static bool GameOver = false;
        static bool TestMode = false;
        static Random diceRandom = new Random();
        static void ResetGame()
        {
            GameOver = false;
            NumberOfPlayers = ReadNumber("Please enter number of players", 2, 4);
            players = new Player[NumberOfPlayers];

            // players to put each player at square (0,0)
            for (int i = 0; i < NumberOfPlayers; i = i + 1)
            {
                Console.Write("Enter the name of player:");
                players[i].Name = Console.ReadLine();
                players[i].X = 0;
                players[i].Y = 0;
            }

        }
        static bool RocketInSquare(int X, int Y, int currentplayer)
        {
            for (int i = 0; i < NumberOfPlayers; i = i + 1)
            {
                if (currentplayer != i)
                {
                    if (players[i].X == X && players[i].Y == Y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static int ReadNumber(string prompt, int min, int max)
        {
            int result = 0;
            do
            {//The instructions below validates answers given by the User (E.g. The number of players or the Dice value in test mode)
                Console.Write(prompt);
                string numberString = Console.ReadLine();
                try
                {
                    result = int.Parse(numberString);
                }
                catch
                {
                    Console.WriteLine("Invalid value");
                }
                if (result > max || result < min)
                    Console.WriteLine("Please enter a value in the range " + min + " to " + max);
                else
                    break;
            } while (true);
            return result;
        }

        static int DiceThrow()
        {
            int DiceSpots = diceRandom.Next(1, 7);
            return DiceSpots;
        }

        private static void PlayerTurn(int PlayerNumber)
        {
            int newY;
            int newX;
            int DiceResult = 1;
            if (TestMode == false)
            {
                DiceResult = DiceThrow();
            }
            else
            {
                DiceResult = ReadNumber("Enter a dice value", 1, 6);
                // if test mode is engaged the player can enter the value that the dice will give when it is rolled
            }
            Console.WriteLine();
            Console.WriteLine(players[PlayerNumber].Name + " turn: Press enter to roll dice");
            Console.ReadLine();
            Console.WriteLine("You have thrown " + DiceResult);
            switch (board[players[PlayerNumber].Y, players[PlayerNumber].X])
            {
                case (Direction.Up): newY = players[PlayerNumber].Y + DiceResult;
                    if (newY <= 7)
                        players[PlayerNumber].Y = newY;
                    break;
                case (Direction.Left): newX = players[PlayerNumber].X - DiceResult;
                    if (newX >= 0)
                        players[PlayerNumber].X = newX;
                    break;
                case (Direction.Down): newY = players[PlayerNumber].Y - DiceResult;
                    if (newY >= 0)
                        players[PlayerNumber].Y = newY;
                    break;
                case (Direction.Right): newX = players[PlayerNumber].X + DiceResult;
                    if (newX <= 7)
                        players[PlayerNumber].X = newX;
                    break;
            }
            Console.WriteLine(players[PlayerNumber].Name + " has moved to posision " + players[PlayerNumber].X + " " + players[PlayerNumber].Y);

            Console.WriteLine();

            while (RocketInSquare(players[PlayerNumber].X, players[PlayerNumber].Y, PlayerNumber))
            {
                switch (board[players[PlayerNumber].Y, players[PlayerNumber].X])
                {// The instructions below move the player to the next square in the event of a collision
                    case (Direction.Up): newY = players[PlayerNumber].Y + 1;
                        if (newY <= 7)
                            players[PlayerNumber].Y = newY;
                        break;
                    case (Direction.Left): newX = players[PlayerNumber].X - 1;
                        if (newX >= 0)
                            players[PlayerNumber].X = newX;
                        break;
                    case (Direction.Down): newY = players[PlayerNumber].Y - 1;
                        if (newY >= 0)
                            players[PlayerNumber].Y = newY;
                        break;
                    case (Direction.Right): newX = players[PlayerNumber].X + 1;
                        if (newX <= 7)
                            players[PlayerNumber].X = newX;
                        break;

                }
                Console.WriteLine("You have collided and moved to the next square");
            }

            if (CheeseSquare(players[PlayerNumber].X, players[PlayerNumber].Y))
            {
                CheesePower(PlayerNumber);
            }

            for (int i = 0; i < players.Length; i = i + 1)
            {
                if (players[i].X == 7 && players[i].Y == 7) // checks if a player is at (7,7) which is the goal
                {
                    GameOver = true;
                    Console.WriteLine(players[i].Name + " Wins!!!");
                    break;
                }
            }
        }
        static bool CheeseSquare(int x, int y)
        {
            if ((x == 0 && y == 3)
                || (x == 3 && y == 5)
                || (x == 4 && y == 1)
                || (x == 6 && y == 4))
                return true;
            else
                return false;
        }
        static void CheesePower(int PlayerNumber)
        {
            Console.WriteLine("Landed on a cheese square!");
            string CheeseAction;
            do
            {
                Console.WriteLine("Absorb cheese power into engines and move again(1)");
                Console.WriteLine("or Fire cannons and send player to bottom of board(2)?");
                Console.WriteLine("Enter 1 or 2 for corresponding answers");
                CheeseAction = Console.ReadLine();
            }
            while (!(CheeseAction == "1" || CheeseAction == "2"));//loops while neither 1 or 2 is inputted

            if (CheeseAction == "1")
            {
                Console.WriteLine("You've decided to move again");
                PlayerTurn(PlayerNumber);//repeats the current players move process(player move)
            }
            else
            {
                if (CheeseAction == "2")
                {
                    bool ValidName = false;
                    int PlayerSelected = -1;
                    do
                    {
                        Console.WriteLine("Enter name of Player to fire at");
                        Console.WriteLine("You can also fire at yourself");
                        string VictimName = Console.ReadLine();
                        for (int i = 0; i < NumberOfPlayers; i = i + 1)
                        {
                            if (VictimName == players[i].Name)
                            {
                                PlayerSelected = i;
                                ValidName = true;
                                break;
                            }
                        }
                    }
                    while (ValidName != true);//loops through until a correct player name is given

                    players[PlayerSelected].Y = 0;
                    Console.WriteLine(players[PlayerSelected].Name + " you have been shot to the bottom of the board");
                    players[PlayerSelected].X = ReadNumber("please enter the X value of the bottom panel which you wish to land on", 0, 7);
                }         
            }
        }
       
        static void ShowStatus()
        {
            Console.WriteLine("Hyperspace Cheese Battle Status");
            Console.WriteLine();
            Console.WriteLine("There are " + NumberOfPlayers + " people playing");
            for (int i = 0; i < NumberOfPlayers; i = i + 1)
            {
                Console.WriteLine(players[i].Name + " is on square " + players[i].X + "," + players[i].Y);
            }
        }
        static void Main(string[] args)
        {
            string ReplayResponse = "y";
            do
            {
                string TestResponse;
                Console.WriteLine("Do you wish to use test mode? Enter y if you do");
                // any other response will be treated as no, hence test mode will only be engaged if the user responds with y
                TestResponse = Console.ReadLine();
                if (TestResponse == "y")
                {
                    TestMode = true;
                }
                else
                {
                    TestMode = false;
                }
                ResetGame();
                do
                {
                    for (int i = 0; i < players.Length; i = i + 1)
                    {
                        PlayerTurn(i);
                        ShowStatus();
                        if (GameOver == true)
                        {
                            break;
                        }
                    }
                }
                while (GameOver == false);

                do
                {
                    Console.WriteLine("Play again? Enter y or n");
                    ReplayResponse = Console.ReadLine();
                }
                while (ReplayResponse != "y" && ReplayResponse != "n");

            }
            while (ReplayResponse == "y");
        }


    }
}
