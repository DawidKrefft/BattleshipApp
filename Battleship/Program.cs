using BattleshipLibrary;
using BattleshipLibrary.Models;
using System;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();
            LoadGame();
            Console.ReadKey();
            
        }

        private static void LoadGame()
        {
            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                // Display grid from activePlayer on where they fired
                DisplayShotGrid(activePlayer);

                // Ask activePlayer for a shot


                // Determine if it is a valid shot
                // Determine shot results
                RecordPlayerShot(activePlayer, opponent);
                // Determine if the game is over
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);
                // If over, set activePlayer as the winner
                // else, swap positions (activePlayer to opponent)
                if (doesGameContinue == true)
                {
                    //swap positions
                    (activePlayer, opponent) = (opponent, activePlayer);
                    //PlayerInfoModel tempHolder = opponent;
                    //opponent = activePlayer;
                    //activePlayer = tempHolder;
                }
                else
                {
                    winner = activePlayer;
                }

            } while (winner == null);

            IndentifyWinner(winner);
        }
        private static void IndentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congratulations to { winner.UserName} for winning!");
            Console.WriteLine($"{ winner.UserName } took { GameLogic.GetShotCount(winner) } shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;

            do
            {
                string shot = AskForShot(activePlayer);
                try
                {
                    (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                    isValidShot = GameLogic.ValidateShot(activePlayer, row, column);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Error: " + ex.Message);
                    isValidShot = false;
                }


                if (isValidShot == false)
                {
                    Console.WriteLine("Invalid Shot Location. Please try again.");
                }

            } while (isValidShot == false);

            // Determine shot results
            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column);
            // Record results
            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            DisplayShotResults(row, column, isAHit);
        }

        private static void DisplayShotResults(string row, int column, bool isAHit)
        {
            if (isAHit)
            {
                Console.WriteLine($" ****** { row }{ column } is a hit! ******"); 
            }
            else
            {
                Console.WriteLine($" ****** { row }{ column } is a miss! ******");
            }
            Console.WriteLine();
        }

        private static string AskForShot(PlayerInfoModel player)
        {
            Console.Write($" { player.UserName }, please enter your shot selection: ");
            string output = Console.ReadLine();
            Console.WriteLine();
            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter} {gridSpot.SpotNumber} ");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Console.Write(" -X- ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Console.Write(" -O- ");
                }
                else
                {
                    Console.Write(" -?- ");
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine(" Welcome to Battleship Game");
            Console.WriteLine(" **************************");
            Console.WriteLine(" Guide:");
            Console.WriteLine(" -O- missed ship");
            Console.WriteLine(" -X- hit ship");
            Console.WriteLine("\n Spots to choose: ");
            Console.WriteLine("\n A1 A2 A3 A4 A5");
            Console.WriteLine(" B1 B2 B3 B4 B5");
            Console.WriteLine(" C1 C2 C3 C4 C5");
            Console.WriteLine(" D1 D2 D3 D4 D5");
            Console.WriteLine(" E1 E2 E3 E4 E5");
            Console.WriteLine();
        }
        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine($" Enter name for {playerTitle}:");

            // Ask the user for therir name
            output.UserName = AskForUsersName();
            // Load uo the shot grid
            GameLogic.CreateGrid(output);
            // Ask the user for their 5 ship placements
            PlaceShips(output);
            // Clear
            Console.Clear();

            return output;
        }

        private static string AskForUsersName()
        {
            Console.Write(" "); // what is user's name
            string output = Console.ReadLine();
            Console.WriteLine();

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($" Where do you want to place ship number {model.ShipLocations.Count + 1}: ");
                string location = Console.ReadLine();

                bool isValidLocation = false;

                try
                {
                    isValidLocation = GameLogic.PlaceShip(model, location);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (isValidLocation == false)
                {
                    Console.WriteLine("That was not a valid location. Please try again.");
                }
            } while (model.ShipLocations.Count<5);
        }
    }
}
