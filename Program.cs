using System;

namespace FilkoKartya
{
    internal class Program
    {

        /// <summary>
        /// A menü osztálya, és annak változója
        /// </summary>
        private static Menu MHandler;

        /// <summary>
        /// A Main függvény, alap beállítások, deklarálások, menü elindítása
        /// </summary>
        /// <param name="args">Console Argumentumjai, amelyek jelen esetben nincsenek/nem szükségesek</param>
        internal static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Üdvözöllek a Filkó Kártya játékban!");
            //TopListPath = Directory.GetCurrentDirectory() + "\\TopList.txt";
            /*if (!File.Exists(TopListPath))
            {
                File.Create(TopListPath).Dispose();
            }*/
            //TopListIni = new Database(TopListPath);
            //TopListIni.ReadFile();
            MHandler = new Menu();
            MHandler.Watcher();
        }

        internal static void CreateNewMenu()
        {
            MHandler = new Menu();
            MHandler.Watcher();
        }

        /// <summary>
        /// A Menü osztályt dobja vissza
        /// </summary>
        internal static Menu Menu
        {
            get { return MHandler; }
        }
    }
}
