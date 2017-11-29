using System;

namespace FilkoKartya
{
    class Menu
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
        }
    }
}
