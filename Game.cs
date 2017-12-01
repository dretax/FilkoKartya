using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace FilkoKartya
{
    internal class Game
    {
        /// <summary>
        /// Random változó
        /// </summary>
        internal readonly Random r;

        /// <summary>
        /// Pakliban lévő kártya típusok.
        /// </summary>
        internal enum Cards
        {
            PirosAdu,
            PirosAsz,
            PirosTiz,
            PirosKiralyFelso,
            PirosKiralyAlso,
            PirosKilenc,
            PirosNyolc,
            PirosHet,

            TokAdu,
            TokAsz,
            TokTiz,
            TokKiralyFelso,
            TokKiralyAlso,
            TokKilenc,
            TokNyolc,
            TokHet,

            ZoldAdu,
            ZoldAsz,
            ZoldTiz,
            ZoldKiralyFelso,
            ZoldKiralyAlso,
            ZoldKilenc,
            ZoldNyolc,
            ZoldHet,

            MakkAdu,
            MakkAsz,
            MakkTiz,
            MakkKiralyFelso,
            MakkKiralyAlso,
            MakkKilenc,
            MakkNyolc,
            MakkHet,

            // Visszatérítendő érték amely a pakliban nem szerepel, és null helyett ezt használja a program ha a játékosnak nincsen egy bizonyos lapja.
            NincsLap,
        }

        /// <summary>
        /// A kártyáknak különböző szín típusai vannak. Ez felel azok deklarálásáért.
        /// </summary>
        internal enum Colors
        {
            Makk,
            Zold,
            Piros,
            Tok
        }

        /// <summary>
        /// Vége van a játéknak?
        /// </summary>
        internal bool Match;

        /// <summary>
        /// Pakliban lévő tényleges kártyák.
        /// </summary>
        internal List<Cards> CardsList;

        /// <summary>
        /// Már az asztalon lévő kártyák.
        /// </summary>
        internal List<Cards> CardsOnBoard;

        /// <summary>
        /// Eddig letett meg nem nyert ászok, illetve tizesek
        /// </summary>
        internal List<Cards> PlacedTensorAces; 

        /// <summary>
        /// Következő játékos aki a kártyát fogja lehelyezni.
        /// </summary>
        internal int NextPlayer;

        /// <summary>
        /// Az elején kiválasztott adu kártya.
        /// </summary>
        internal Cards AduKartya;

        /// <summary>
        /// Az elején kiválaszott adu kártyának a színe.
        /// </summary>
        internal Colors AduSzine;

        /// <summary>
        /// Jelen játékosok száma.
        /// </summary>
        internal int Players = 4;

        /// <summary>
        /// Játékosok kártyái.
        /// </summary>
        internal Dictionary<int, List<Cards>> PlayerCards;

        /// <summary>
        /// Első csapat pontjainak száma
        /// </summary>
        internal int TeamOnePoints = 0;

        /// <summary>
        /// Második csapat pontjainak száma
        /// </summary>
        internal int TeamTwoPoints = 0;

        /// <summary>
        /// Aki az utolsó ászt vagy adut tette le
        /// </summary>
        internal int LastAceorTenPlacer;

        /// <summary>
        /// Utolsó adu kártya letevője.
        /// </summary>
        internal int LastAduPosPlacer;

        /// <summary>
        /// Első csapat által lehyelezett adu kártyák száma
        /// </summary>
        internal int PlacedAdusTeamOne = 0;

        /// <summary>
        /// Második csapat által lehelyezett adu kártyák száma
        /// </summary>
        internal int PlacedAdusTeamTwo = 0;

        /// <summary>
        /// A Game osztály konstruktora. Az alap információk itt leszen kiírva
        /// a változók itt lesznek deklarálva, és a játékot magát is innen irányítjuk.
        /// </summary>
        internal Game(int players, int to = 0, int to2 = 0, int aduplacer = -1, int placer = -1, int nextp = 2, int placed1 = 0, int placed2 = 0, List<Cards> CardsOnBoard = null,
            Dictionary<int, List<Cards>> PlayerCards = null, List<Cards> PlacedTensorAces = null)
        {
            Match = false;
            LastAduPosPlacer = aduplacer;
            LastAceorTenPlacer = placer;
            TeamOnePoints = to;
            TeamTwoPoints = to2;
            PlacedAdusTeamOne = placed1;
            PlacedAdusTeamTwo = placed2;

            Players = players;
            NextPlayer = nextp;
            bool loaded = false;
            if (CardsOnBoard == null)
            {
                this.CardsOnBoard = new List<Cards>();
            }
            else
            {
                this.CardsOnBoard = CardsOnBoard;
                loaded = true;
            }
            if (PlayerCards == null)
            {
                this.PlayerCards = new Dictionary<int, List<Cards>>();
            }
            else
            {
                this.PlayerCards = PlayerCards;
            }
            if (PlacedTensorAces == null)
            {
                this.PlacedTensorAces = new List<Cards>();
            }
            else
            {
                this.PlacedTensorAces = PlacedTensorAces;
            }
            CardsList = new List<Cards>();
            r = new Random();
            if (!loaded)
            {
                RandomCards();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Pakli keverése megtörtént. Összesen " + CardsList.Count + "db kártya.");
                int id = 1;
                var currentcard = CardsList[CardsList.Count - id];
                while (currentcard.ToString().Contains("Adu"))
                {
                    id++;
                    currentcard = CardsList[CardsList.Count - id];
                }

                Console.WriteLine("Adu kártya: " + currentcard);


                AduKartya = currentcard;
                AduSzine = GetCardColor(AduKartya);
                Console.WriteLine("Adu Kártya Színe: " + AduSzine);

                int iterations = Players*2;
                if (Players == 6)
                {
                    iterations = Players;
                }
                for (int i = 1; i <= iterations; i++)
                {
                    if (Players == 4)
                    {
                        if (i < 5)
                        {
                            this.PlayerCards[i] = new List<Cards>();
                        }
                        int adder = i;
                        if (i > 4)
                        {
                            adder = adder - 4;
                        }
                        for (int j2 = 0; j2 < 4; j2++) // Osztunk 4 lapot.
                        {
                            this.PlayerCards[adder].Add(CardsList[CardsList.Count - 1]);
                            CardsList.RemoveAt(CardsList.Count - 1);
                        }
                    }
                    else
                    {
                        this.PlayerCards[i] = new List<Cards>();
                        for (int j2 = 0; j2 < 5; j2++)
                        {
                            this.PlayerCards[i].Add(CardsList[CardsList.Count - 1]);
                            CardsList.RemoveAt(CardsList.Count - 1);
                        }
                    }
                }
            }
            PrintPlayerCards(1);
            if (loaded)
            {
                Console.WriteLine();
                Console.WriteLine("==================================================================");
                Console.WriteLine("Utolsó kártya az asztalon: " + CardsOnBoard[CardsOnBoard.Count - 1]);
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            while (!Match)
            {
                ContinueMatch();
            }
            Console.WriteLine("=== Vége ===");
            Console.WriteLine("Első csapat pontszáma: " + TeamOnePoints);
            Console.WriteLine("Második csapat pontszáma: " + TeamTwoPoints);
            string s = "Senki";
            if ((TeamOnePoints >= 6 && Players == 4) || (TeamOnePoints >= 4 && Players == 6))
            {
                s = "Első csapat";
            }
            if ((TeamTwoPoints >= 6 && Players == 4) || (TeamTwoPoints >= 4 && Players == 6))
            {
                s = "Második csapat";
            }
            Console.WriteLine("Nyertes: " + s);
            Console.WriteLine();
            Console.WriteLine("Menübe lépéshez nyomjon meg egy gombot.");
            Console.ReadKey();
            Console.Clear();
            Program.CreateNewMenu();
        }

        /// <summary>
        /// Lekérdezi a kapott kártya értékét. Enum értékkel nem jó, hisz azonos enum értékek nem létezhetnek.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal int GetCardValue(Cards card)
        {
            string name = card.ToString().ToLower();
            int value = -1;
            if (name.Contains("adu"))
            {
                value = 8;
            }
            else if (name.Contains("asz"))
            {
                value = 7;
            }
            else if (name.Contains("tiz"))
            {
                value = 6;
            }
            else if (name.Contains("felso"))
            {
                value = 5;
            }
            else if (name.Contains("also"))
            {
                value = 4;
            }
            else if (name.Contains("kilenc"))
            {
                value = 3;
            }
            else if (name.Contains("nyolc"))
            {
                value = 2;
            }
            else if (name.Contains("het"))
            {
                value = 1;
            }
            return value;
        }

        /// <summary>
        /// Megadott játékosnak a kártyái.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        internal List<Cards> GetPlayerCards(int i)
        {
            return PlayerCards[i];
        }

        /// <summary>
        /// Megadott játékos kártyáinak kiiratása.
        /// </summary>
        /// <param name="i"></param>
        internal void PrintPlayerCards(int i)
        {
            Console.WriteLine("Kártyáid: " + string.Join(", ", PlayerCards[i].Select(e => e.ToString()).ToArray()));
        }

        /// <summary>
        /// Megadott kártya színének lekérése.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal Colors GetCardColor(Cards card)
        {
            if (card.ToString().Contains("Makk"))
            {
                return Colors.Makk;
            }
            if (card.ToString().Contains("Piros"))
            {
                return Colors.Piros;
            }
            if (card.ToString().Contains("Zold"))
            {
                return Colors.Zold;
            }
            return Colors.Tok;
        }

        /// <summary>
        /// Megadott kártya színének lekérése.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        internal Colors GetCardColor(string card)
        {
            if (card.Contains("Makk"))
            {
                return Colors.Makk;
            }
            if (card.Contains("Piros"))
            {
                return Colors.Piros;
            }
            if (card.Contains("Zold"))
            {
                return Colors.Zold;
            }
            return Colors.Tok;
        }

        /// <summary>
        /// Visszatérít egy azonos színű kártyát, ha van.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="card2"></param>
        /// <returns></returns>
        internal Cards GetPlayerMatchingColorCard(int player, Cards card2)
        {
            foreach (var card in PlayerCards[player])
            {
                if ((GetCardColor(card) == GetCardColor(card2)))
                {
                    return card;
                }
            }
            return Cards.NincsLap;
        }

        /// <summary>
        /// Visszatérít egy adu kártyát, ha van
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal Cards GetPlayerMatchingAduCardOnly(int player)
        {
            foreach (var card in PlayerCards[player])
            {
                if (GetCardValue(card) == 8)
                {
                    return card;
                }
            }
            return Cards.NincsLap;
        }

        /// <summary>
        /// Beírt szöveg egyezik-e akármelyik kártya nevével?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal bool IsTextMatchingAnyCard(string name)
        {
            // LINQBAN ez: return Enum.GetNames(typeof (Cards)).ToList().Any(x => string.Equals(x, name, StringComparison.CurrentCultureIgnoreCase));
            foreach (var x in Enum.GetNames(typeof (Cards)).ToList())
            {
                if (string.Equals(x, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Van olyan kártyája amit beírt a játékos?
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cardname"></param>
        /// <returns></returns>
        internal Cards HasSpecificMatchingCard(int player, string cardname)
        {
            foreach (var card in PlayerCards[player])
            {
                if (string.Equals(card.ToString(), cardname, StringComparison.CurrentCultureIgnoreCase))
                {
                    return card;
                }
            }
            return Cards.NincsLap;
        }

        /// <summary>
        /// Keressünk egy olyan kártyát, amely random válaszott, és nem adu.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal Cards GetRandomNormalCard(int player)
        {
            Cards card = PlayerCards[player][r.Next(0, PlayerCards[player].Count)];
            while (GetCardValue(card) == 8)
            {
                card = PlayerCards[player][r.Next(0, PlayerCards[player].Count)];
            }
            return card;
        }

        /// <summary>
        /// Egy random kártyát ad vissza a pakliból.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal Cards GetRandomChosenCard(int player)
        {
            Cards card = PlayerCards[player][r.Next(0, PlayerCards[player].Count)];
            return card;
        }

        /// <summary>
        /// Játékos utolsó kártyájának lekérése.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        internal Cards GetPlayerLastCard(int player)
        {
            if (PlayerCards[player].Count - 1 >= 0)
            {
                List<Cards> cdlist = PlayerCards[player];
                return cdlist[cdlist.Count - 1];
            }
            return Cards.NincsLap;
        }

        /// <summary>
        /// Pakli keverése
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        internal void ShuffleCards<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Van még olyan játékos akinek van kártyája?
        /// </summary>
        /// <returns></returns>
        internal bool DoWeHaveCards()
        {
            for (int i = 1; i <= Players; i++)
            {
                if (PlayerCards[i].Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A kártyajáték folytatásáért felelős void.
        /// </summary>
        internal void ContinueMatch()
        {
            bool sup = DoWeHaveCards();
            if (!sup)
            {
                Match = true;
                return;
            }
            if (NextPlayer != 1)
            {
                Thread.Sleep(1500);
                if (CardsOnBoard.Count == 0)
                {
                    Cards randomfirstcard = GetRandomNormalCard(NextPlayer);
                    Console.WriteLine(NextPlayer + " számú gép letett egy kártyát. (" +
                                      randomfirstcard + ")");
                    CardsOnBoard.Add(randomfirstcard);
                    PlayerCards[NextPlayer].Remove(randomfirstcard);
                    AssignNextPlayer();
                }
                else
                {
                    Cards randomcard = GetPlayerLastCard(NextPlayer);
                    if (randomcard == Cards.NincsLap)
                    {
                        AssignNextPlayer();
                        return;
                    }
                    var lastcardonboard = CardsOnBoard[CardsOnBoard.Count - 1];
                    Cards matchingcard = GetPlayerMatchingColorCard(NextPlayer, lastcardonboard); // Keresünk egy egyező színűt.
                    if (matchingcard != Cards.NincsLap && (PlacedTensorAces.Count == 0)) // Ha van egyező színünk, és nem tettek le ászt, illetve tizest, akkor nyugodtan tegyen színt.
                    {
                        Console.WriteLine(NextPlayer + " számú gép letett egy kártyát. (" +
                                          matchingcard + ")");
                        CardsOnBoard.Add(matchingcard);
                        PlayerCards[NextPlayer].Remove(matchingcard);
                        if (GetCardValue(matchingcard) == 7 || GetCardValue(matchingcard) == 6) // Ha 10-es vagy ász
                        {
                            PlacedTensorAces.Add(matchingcard);
                            LastAceorTenPlacer = NextPlayer;
                        }
                        else
                        {
                            if (LastAceorTenPlacer != -1) // Ha valaki tett már le ászt, vagy 10-est
                            {
                                if (GetCardValue(matchingcard) == 8) // Ha ütőkártyát tesznek akkor adjuk hozzá az eddigi letett ütőkártyák számához
                                {
                                    LastAduPosPlacer = NextPlayer;
                                    int team = GetPlayerTeam(NextPlayer);
                                    if (team == 1)
                                    {
                                        PlacedAdusTeamOne++;
                                    }
                                    else
                                    {
                                        PlacedAdusTeamTwo++;
                                    }
                                }
                                else
                                {
                                    if (LastAduPosPlacer != -1) // Ha van adu letételünk
                                    {
                                        int team = GetPlayerTeam(LastAduPosPlacer);
                                        if (GetOppositeTeamCards(team) < GetTeamCards(team)) // Több ütőkártyánk van letéve mint az ellenfélnek?
                                        {
                                            GivePoints(LastAduPosPlacer, PlacedTensorAces.Count);
                                            PlacedTensorAces.Clear();
                                            LastAduPosPlacer = -1;
                                            LastAceorTenPlacer = -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Cards adukartya = GetPlayerMatchingAduCardOnly(NextPlayer);
                        if (adukartya != Cards.NincsLap) // Ha van adu kártyája és tettek már le tizest vagy ászt, akkor tegyünk le adut.
                        {
                            Console.WriteLine(NextPlayer + " számú gép letett egy kártyát. (" +
                                          adukartya + ")");
                            CardsOnBoard.Add(adukartya);
                            PlayerCards[NextPlayer].Remove(adukartya);
                            LastAduPosPlacer = NextPlayer;
                            int team = GetPlayerTeam(NextPlayer);
                            if (team == 1)
                            {
                                PlacedAdusTeamOne++;
                            }
                            else
                            {
                                PlacedAdusTeamTwo++;
                            }
                        }
                        else
                        {
                            randomcard = GetRandomChosenCard(NextPlayer);
                            if (randomcard != Cards.NincsLap)
                            {
                                Console.WriteLine(NextPlayer + " számú gép letett egy kártyát. (" +
                                                  randomcard + ")");
                                CardsOnBoard.Add(randomcard);
                                PlayerCards[NextPlayer].Remove(randomcard);
                                if (GetCardValue(matchingcard) == 7 || GetCardValue(matchingcard) == 6) // Ha 10-es vagy ász
                                {
                                    PlacedTensorAces.Add(matchingcard);
                                    LastAceorTenPlacer = NextPlayer;
                                }
                                else
                                {
                                    if (LastAceorTenPlacer != -1) // Ha valaki tett már le ászt, vagy 10-est
                                    {
                                        if (GetCardValue(matchingcard) == 8) // Ha ütőkártyát tesznek akkor adjuk hozzá az eddigi letett ütőkártyák számához
                                        {
                                            LastAduPosPlacer = NextPlayer;
                                            int team = GetPlayerTeam(NextPlayer);
                                            if (team == 1)
                                            {
                                                PlacedAdusTeamOne++;
                                            }
                                            else
                                            {
                                                PlacedAdusTeamTwo++;
                                            }
                                        }
                                        else
                                        {
                                            if (LastAduPosPlacer != -1) // Ha van adu letételünk
                                            {
                                                int team = GetPlayerTeam(LastAduPosPlacer);
                                                if (GetOppositeTeamCards(team) < GetTeamCards(team)) // Több ütőkártyánk van letéve mint az ellenfélnek?
                                                {
                                                    GivePoints(LastAduPosPlacer, PlacedTensorAces.Count);
                                                    PlacedTensorAces.Clear();
                                                    LastAduPosPlacer = -1;
                                                    LastAceorTenPlacer = -1;
                                                    PlacedAdusTeamOne = 0;
                                                    PlacedAdusTeamTwo = 0;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(NextPlayer + " számú gépnek nincsen több lapja!");
                            }
                        }
                    }

                    AssignNextPlayer();
                }
            }
            else
            {
                Thread.Sleep(500);
                Console.WriteLine();
                Console.WriteLine("Te következel. Írj be egy letenni kívánt lapot. Színre-szint kell tenned, adura adut.");
                Console.WriteLine("Ha nincs színed akkor adut kell tenned. Ha az sincsen akkor bármit letehetsz.");
                Thread.Sleep(1000);
                VerifyCardPlacement();

                AssignNextPlayer();
            }
        }

        /// <summary>
        /// A játékos lépését ez a method kezeli le.
        /// Nem engedi szabályt szegni, és hasonlók, továbbá itt kezeljük a
        /// feladást illetve a mentést is.
        /// </summary>
        internal void VerifyCardPlacement()
        {
            Console.WriteLine();
            Console.WriteLine();
            PrintPlayerCards(1);
            string s = Console.ReadLine();
            if (s == "feladas")
            {
                Console.WriteLine("Feladtad a játékot..");
                Thread.Sleep(1500);
                Program.CreateNewMenu();
                return;
            }
            if (s == "mentes")
            {
                Console.WriteLine("Írj be egy filenevet.");
                string filenev = Console.ReadLine();
                while (string.IsNullOrEmpty(filenev) || File.Exists(filenev + ".ini"))
                {
                    Console.WriteLine("Írj be egy filenevet mert ez vagy hibás, vagy már létezik.");
                    filenev = Console.ReadLine();
                }
                File.Create(filenev + ".ini").Dispose();
                IniParser ini = new IniParser(filenev + ".ini");
                ini.AddSetting("GameState", "PlacedAdusTeamOne", PlacedAdusTeamOne.ToString());
                ini.AddSetting("GameState", "PlacedAdusTeamTwo", PlacedAdusTeamTwo.ToString());
                ini.AddSetting("GameState", "LastAceorTenPlacer", LastAceorTenPlacer.ToString());
                ini.AddSetting("GameState", "LastAduPosPlacer", LastAduPosPlacer.ToString());
                ini.AddSetting("GameState", "TeamOnePoints", TeamOnePoints.ToString());
                ini.AddSetting("GameState", "TeamTwoPoints", TeamTwoPoints.ToString());
                ini.AddSetting("GameState", "Players", Players.ToString());
                ini.AddSetting("GameState", "NextPlayer", NextPlayer.ToString());
                string cd = "";
                foreach (var x in CardsOnBoard)
                {
                    cd = cd + ((int) x) + ",";
                }
                ini.AddSetting("Cards", "CardsOnBoard", cd);

                string cd2 = "";
                foreach (var x in PlacedTensorAces)
                {
                    cd2 = cd2 + ((int)x) + ",";
                }
                ini.AddSetting("Cards", "PlacedTensorAces", cd2);
                
                /*string cd2 = "";
                foreach (var x in CardsList)
                {
                    cd2 = cd2 + ((int)x) + ",";
                }
                ini.AddSetting("Cards", "CardsList", cd2);*/

                foreach (var x in PlayerCards.Keys)
                {
                    string playercards = "";
                    foreach (var x2 in PlayerCards[x])
                    {
                        playercards = playercards + ((int)x2) + ",";
                    }
                    ini.AddSetting("PlayerCards", x.ToString(), playercards);
                }
                ini.Save();
                Console.WriteLine("Játék mentésre került.");
                VerifyCardPlacement();
                return;
            }

            Cards ReceivedCard = HasSpecificMatchingCard(1, s);
            while (ReceivedCard == Cards.NincsLap)
            {
                Console.WriteLine("Nincs ilyen kártyád!");
                PrintPlayerCards(1);
                Console.WriteLine();
                s = Console.ReadLine();
                ReceivedCard = HasSpecificMatchingCard(1, s);
            }
            var lastcardonboard = CardsOnBoard[CardsOnBoard.Count - 1];
            Colors color = GetCardColor(lastcardonboard);
            Colors color2 = GetCardColor(ReceivedCard);
            if (color != color2 && (GetCardValue(lastcardonboard) != 8 || GetCardValue(ReceivedCard) != 8) && ((GetCardValue(lastcardonboard) == 7 || GetCardValue(lastcardonboard) == 6) && GetCardValue(ReceivedCard) != 8))
            {
                Cards cd = GetPlayerMatchingColorCard(1, lastcardonboard);
                if (cd != Cards.NincsLap && GetCardValue(lastcardonboard) != 8) // Ha a játékos nem megyegyező színt akar letenni középre, de van nála egyező szín.
                {
                    Console.WriteLine("Nem tehetsz más színű kártyát le, úgy hogy van nálad egyező színű!");
                    VerifyCardPlacement();
                    return;
                }
            }
            /*if (GetCardValue(lastcardonboard) == 8 && GetCardValue(ReceivedCard) != 8 && GetPlayerMatchingAduCardOnly(1) != Cards.NincsLap)
            {
                Console.WriteLine("Adu kártyára csak Adu kártyát tehetsz, kivéve akkor ha nincs adu kártyád! (Felső típusú kártyák)");
                VerifyCardPlacement();
                return;
            }*/
            if (GetCardValue(ReceivedCard) == 7 || GetCardValue(ReceivedCard) == 6) // Ha 10-es vagy ász
            {
                PlacedTensorAces.Add(ReceivedCard);
                LastAceorTenPlacer = NextPlayer;
            }
            else
            {
                if (LastAceorTenPlacer != -1) // Ha valaki tett már le ászt, vagy 10-est
                {
                    if (GetCardValue(ReceivedCard) == 8) // Ha ütőkártyát tesznek akkor adjuk hozzá az eddigi letett ütőkártyák számához
                    {
                        LastAduPosPlacer = NextPlayer;
                        int team = GetPlayerTeam(NextPlayer);
                        if (team == 1)
                        {
                            PlacedAdusTeamOne++;
                        }
                        else
                        {
                            PlacedAdusTeamTwo++;
                        }
                    }
                    else
                    {
                        if (LastAduPosPlacer != -1) // Ha van adu letételünk
                        {
                            int team = GetPlayerTeam(LastAduPosPlacer);
                            if (GetOppositeTeamCards(team) < GetTeamCards(team)) // Több ütőkártyánk van letéve mint az ellenfélnek?
                            {
                                GivePoints(LastAduPosPlacer, PlacedTensorAces.Count);
                                PlacedTensorAces.Clear();
                                LastAduPosPlacer = -1;
                                LastAceorTenPlacer = -1;
                                PlacedAdusTeamOne = 0;
                                PlacedAdusTeamTwo = 0;
                            }
                        }
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("======================================");
            Console.WriteLine("Letettél egy kártyát. " + ReceivedCard);
            CardsOnBoard.Add(ReceivedCard);
            PlayerCards[NextPlayer].Remove(ReceivedCard);
        }

        /// <summary>
        /// Pakliból a fölösleges kártyák kivétele, majd megkeverése.
        /// </summary>
        internal void RandomCards()
        {
            CardsList = Enum.GetValues(typeof(Cards)).Cast<Cards>().ToList();
            Cards value = Cards.NincsLap;
            Cards Tok7 = Cards.NincsLap;
            Cards Zold7 = Cards.NincsLap;
            foreach (var x in CardsList)
            {
                if (x == Cards.NincsLap)
                {
                    value = x;
                    continue;
                }
                if (Players == 6)
                {
                    if (x == Cards.TokHet)
                    {
                        Tok7 = x;
                    }
                    else if (x == Cards.ZoldHet)
                    {
                        Zold7 = x;
                    }
                }
            }
            CardsList.Remove(value);
            if (Tok7 != Cards.NincsLap) { CardsList.Remove(Tok7);}
            if (Zold7 != Cards.NincsLap) { CardsList.Remove(Zold7);}
            Console.WriteLine();
            Console.WriteLine();
            ShuffleCards(CardsList);
        }

        /// <summary>
        /// Következő játékos a körben
        /// </summary>
        internal void AssignNextPlayer()
        {
            NextPlayer = NextPlayer + 1;
            if (NextPlayer > Players)
            {
                NextPlayer = 1;
            }
        }

        /// <summary>
        /// Egyszerű pont hozzáadó method.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cardamount"></param>
        internal void GivePoints(int player, int cardamount)
        {
            if (Players == 4)
            {
                if (player > 2)
                {
                    TeamTwoPoints = TeamTwoPoints + cardamount;
                    Console.WriteLine("A 2. csapat megszerzett " + cardamount + " kártyát. Összes pontjuk: " + TeamTwoPoints);
                }
                else
                {
                    TeamOnePoints = TeamOnePoints + cardamount;
                    Console.WriteLine("Az 1. csapat megszerzett " + cardamount + " kártyát. Összes pontjuk: " + TeamOnePoints);
                }
            }
            else
            {
                if (player > 3)
                {
                    TeamTwoPoints = TeamTwoPoints + cardamount;
                    Console.WriteLine("A 2. csapat megszerzett " + cardamount + " kártyát. Összes pontjuk: " + TeamTwoPoints);
                }
                else
                {
                    TeamOnePoints = TeamOnePoints + cardamount;
                    Console.WriteLine("Az 1. csapat megszerzett " + cardamount + " kártyát. Összes pontjuk: " + TeamOnePoints);
                }
            }
        }

        internal int GetPlayerTeam(int player)
        {
            if (Players == 4)
            {
                if (player > 2)
                {
                    return 2;
                }
                return 1;
            }
            if (player > 3)
            {
                return 2;
            }
            return 1;
        }

        internal int GetTeamCards(int team)
        {
            if (team == 1)
            {
                return PlacedAdusTeamOne;
            }
            return PlacedAdusTeamTwo;
        }

        internal int GetOppositeTeamCards(int team)
        {
            if (team == 1)
            {
                return PlacedAdusTeamTwo;
            }
            return PlacedAdusTeamOne;
        }
    }
}
