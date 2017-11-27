using System;
using System.Collections.Generic;
using System.Linq;

namespace FilkoKartya
{
    internal class Game
    {
        /// <summary>
        /// Random változó
        /// </summary>
        internal readonly Random r;

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



            NincsLap,
        }
  
        internal enum Colors
        {
            Makk,
            Zold,
            Piros,
            Tok
        }

        internal bool Match = false;

        internal List<Cards> CardsList;

        internal List<Cards> CardsOnBoard;
        internal int NextPlayer;

        internal Cards AduKartya;
        internal Colors AduSzine;

        internal int Players = 4;

        internal Dictionary<int, List<Cards>> PlayerCards;

        /// <summary>
        /// A Game osztály konstruktora. Az alap információk itt leszen kiírva
        /// a változók itt lesznek deklarálva, és a játékot magát is innen irányítjuk.
        /// </summary>
        internal Game(int players)
        {
            Players = players;
            NextPlayer = 2;
            CardsOnBoard = new List<Cards>();
            PlayerCards = new Dictionary<int, List<Cards>>();
            CardsList = new List<Cards>();
            r = new Random();
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
            
            for (int i = 1; i <= Players * 2; i++)
            {
                if (Players == 4)
                {
                    if (i < 5)
                    {
                        PlayerCards[i] = new List<Cards>();
                    }
                    int adder = i;
                    if (i > 4)
                    {
                        adder = adder - 4;
                    }
                    PlayerCards[adder].Add(CardsList[CardsList.Count - 1]);
                    CardsList.RemoveAt(CardsList.Count - 1);

                    PlayerCards[adder].Add(CardsList[CardsList.Count - 1]);
                    CardsList.RemoveAt(CardsList.Count - 1);

                    PlayerCards[adder].Add(CardsList[CardsList.Count - 1]);
                    CardsList.RemoveAt(CardsList.Count - 1);

                    PlayerCards[adder].Add(CardsList[CardsList.Count - 1]);
                    CardsList.RemoveAt(CardsList.Count - 1);
                }
                else
                {
                    if (i < 7)
                    {
                        PlayerCards[i] = new List<Cards>();
                    }
                    int adder = i;
                    if (i > 6)
                    {
                        adder = adder - 6;
                    }
                    PlayerCards[adder].Add(CardsList[CardsList.Count - 1]);
                    CardsList.RemoveAt(CardsList.Count - 1);

                    PlayerCards[adder].Add(CardsList[CardsList.Count - 1]);
                    CardsList.RemoveAt(CardsList.Count - 1);
                }
            }
            PrintPlayerCards(1);
            PrintPlayerCards(2);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            while (!Match)
            {
                ContinueMatch();
            }
            Console.WriteLine("ggggg");
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

        internal List<Cards> GetPlayerCards(int i)
        {
            return PlayerCards[i];
        }

        internal void PrintPlayerCards(int i)
        {
            Console.WriteLine("Kártyáid: " + string.Join(", ", PlayerCards[i].Select(e => e.ToString()).ToArray()));
        }

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

        internal Cards GetPlayerMatchingColorCard(int player, Cards card2)
        {
            foreach (var card in PlayerCards[player])
            {
                if ((GetCardColor(card) == GetCardColor(card2)) || (GetCardValue(card) == 8 && GetCardValue(card2) == 8)) // Ha egyezik a szín, vagy adu kártya mind2, mert akkor ugyanaz a szín megint.
                {
                    return card;
                }
            }
            if (PlayerCards.Count - 1 >= 0)
            {
                return PlayerCards[player][PlayerCards.Count - 1];
            }
            return Cards.NincsLap;
        }

        internal Cards GetPlayerAduCard(int player)
        {
            foreach (var card in PlayerCards[player])
            {
                if (GetCardValue(card) == 8) // Ha találtunk adu kártyát
                {
                    return card;
                }
            }
            return Cards.NincsLap;
        }

        internal Cards GetPlayerLastCard(int player)
        {
            if (PlayerCards[player].Count - 1 >= 0)
            {
                return PlayerCards[player][PlayerCards.Count - 1];
            }
            return Cards.NincsLap;
        }

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

        internal void ContinueMatch()
        {
            bool sup = DoWeHaveCards();
            if (NextPlayer != 1)
            {
                if (CardsOnBoard.Count == 0)
                {
                    Console.WriteLine(NextPlayer + " számú gép letett egy kártyát. (" +
                                      PlayerCards[NextPlayer][PlayerCards.Count - 1] + ")");
                    CardsOnBoard.Add(PlayerCards[NextPlayer][PlayerCards.Count - 1]);
                    PlayerCards[NextPlayer].RemoveAt(PlayerCards.Count - 1);
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
                    Cards matchingcard = GetPlayerMatchingColorCard(NextPlayer, lastcardonboard); // Keresünk egy egyező színűt, vagy adut.
                    if (matchingcard != Cards.NincsLap)
                    {
                        Console.WriteLine(NextPlayer + " számú gép letett egy kártyát. (" +
                                          matchingcard + ")");
                        CardsOnBoard.Add(matchingcard);
                        PlayerCards[NextPlayer].Remove(matchingcard);
                    }
                    else
                    {
                        Cards adukartya = GetPlayerAduCard(NextPlayer);
                        if (adukartya != Cards.NincsLap)
                        {
                            Console.WriteLine(NextPlayer + " számú gép letett egy kártyát. (" +
                                          matchingcard + ")");
                            CardsOnBoard.Add(matchingcard);
                            PlayerCards[NextPlayer].Remove(matchingcard);
                        }
                        else
                        {
                            if (randomcard != Cards.NincsLap)
                            {
                                Console.WriteLine(NextPlayer + " számú gép letett egy kártyát. (" +
                                                  matchingcard + ")");
                                CardsOnBoard.Add(matchingcard);
                                PlayerCards[NextPlayer].Remove(matchingcard);
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
                Console.WriteLine();
                Console.WriteLine("Te következel. Írj be egy lapot.");
                PrintPlayerCards(1);
                string s = Console.ReadLine();
                //TODO

                AssignNextPlayer();
            }
        }

        internal void RandomCards()
        {
            CardsList = Enum.GetValues(typeof(Cards)).Cast<Cards>().ToList();
            Cards value = (Cards) 0;
            foreach (var x in CardsList)
            {
                if (x == Cards.NincsLap)
                {
                    value = x;
                    break;
                }
            }
            CardsList.Remove(value);
            ShuffleCards(CardsList);
        }

        internal void AssignNextPlayer()
        {
            NextPlayer = NextPlayer + 1;
            if (NextPlayer > Players)
            {
                NextPlayer = 1;
            }
        }
    }
}
