using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FilkoKartya
{
    internal class Menu
    {
        /// <summary>
        /// A teljes program ezen változó alatt fog futni. Ha ez hamis, a program leáll.
        /// </summary>
        private bool Run = true;
        /// <summary>
        /// A Game osztályt itt tároljuk
        /// </summary>
        private static Game _game;

        /// <summary>
        /// A játékos utasításainak végrehajtásáért felelős void.
        /// </summary>
        internal void Watcher()
        {
            while (Run)
            {
                Help();
                string s = Console.ReadLine();
                int i;
                bool b = int.TryParse(s, out i);
                while (!b)
                {
                    Console.WriteLine("Hibás Opció! Listázom a lehetőségeket.");
                    Help();
                    s = Console.ReadLine();
                    b = int.TryParse(s, out i);
                }
                switch (i)
                {
                    case 1:
                        {
                            Console.WriteLine("4, vagy 6 játékos?");
                            string s2 = Console.ReadLine();
                            int i2;
                            bool b2 = int.TryParse(s2, out i2);
                            if (!b2 || (i2 != 4 && i2 != 6))
                            {
                                i2 = 4;
                            }
                            _game = new Game(i2);
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("Adj meg egy file nevet! (Kiterjesztés nem szükséges)");
                            string filenev = Console.ReadLine();
                            while (string.IsNullOrEmpty(filenev))
                            {
                                Console.WriteLine("Hibás argumentum! Adj meg egy file nevet! (Kiterjesztés nem szükséges)");
                                filenev = Console.ReadLine();
                            }
                            while (!File.Exists(filenev + ".ini"))
                            {
                                Console.WriteLine("Nincs ilyen file a jelenlegi mappában!");
                                filenev = Console.ReadLine();
                            }
                            IniParser ini = new IniParser(filenev + ".ini");
                            int FirstAceorAduPos = int.Parse(ini.GetSetting("GameState", "FirstAceorAduPos"));
                            int LastAceorAduPos = int.Parse(ini.GetSetting("GameState", "LastAceorAduPos"));
                            int LastAceorAduPosPlacer = int.Parse(ini.GetSetting("GameState", "LastAceorAduPosPlacer"));
                            int TeamOnePoints = int.Parse(ini.GetSetting("GameState", "TeamOnePoints"));
                            int TeamTwoPoints = int.Parse(ini.GetSetting("GameState", "TeamTwoPoints"));
                            int Players = int.Parse(ini.GetSetting("GameState", "Players"));
                            int NextPlayer = int.Parse(ini.GetSetting("GameState", "NextPlayer"));
                            string[] list = ini.GetSetting("Cards", "CardsOnBoard").Split(',');

                            // LINQ: List<Game.Cards> CardsOnBoard = (from x in list where !string.IsNullOrEmpty(x) select int.Parse(x) into cardnum select (Game.Cards) cardnum).ToList();
                            List<Game.Cards> CardsOnBoard = new List<Game.Cards>();
                            foreach (var x in list)
                            {
                                if (!string.IsNullOrEmpty(x))
                                {
                                    int cardnum = int.Parse(x);
                                    CardsOnBoard.Add((Game.Cards) cardnum);
                                }
                            }

                            Dictionary<int, List<Game.Cards>> PlayerCards = new Dictionary<int, List<Game.Cards>>();

                            string[] enumsec = ini.EnumSection("PlayerCards");
                            foreach (var x in enumsec)
                            {
                                int playerid = int.Parse(x);
                                PlayerCards[playerid] = new List<Game.Cards>();
                                string[] list2 = ini.GetSetting("PlayerCards", x).Split(',');
                                foreach (var x2 in list2)
                                {
                                    if (!string.IsNullOrEmpty(x2))
                                    {
                                        int playercard = int.Parse(x2);
                                        PlayerCards[playerid].Add((Game.Cards) playercard);
                                    }
                                }
                            }
                            Console.WriteLine("Mentés beolvasása sikeres. Betöltés....");
                            Thread.Sleep(2000);
                            _game = new Game(Players, TeamOnePoints, TeamTwoPoints, FirstAceorAduPos, LastAceorAduPos, LastAceorAduPosPlacer, NextPlayer, CardsOnBoard, PlayerCards);
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Hibás Opció! Listázom a lehetőségeket.");
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Szimpla 'Help' üzenetek
        /// </summary>
        internal void Help()
        {
            Console.WriteLine();
            Console.WriteLine("Válassz egy opciót (Üss be egy számot)");
            Console.WriteLine("1. Játék");
            Console.WriteLine("2. Játék betöltése.");
        }
    }
}
