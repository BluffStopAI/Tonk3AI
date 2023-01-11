using AI;

namespace BluffStopp_SU22
{
    public enum Suit { Hjärter, Ruter, Spader, Klöver, Wild };
    class Program
    {
        static void Main(string[] args)
        {
            List<Player> players = new List<Player>();

            players.Add(new RandomPlayer(80, 15));
            players.Add(new HonestPlayer());
            players.Add(new Tonk3());
            players.Add(new Tonk3Stale());
            players.Add(new Rev2());
            players.Add(new Rev3());
            players.Add(new MyPlayer());

            Console.WriteLine("Vilka två spelare skall mötas?");
            for (int i = 1; i <= players.Count; i++)
            {
                Console.WriteLine(i + ": {0}", players[i - 1].Name);
            }
            int p1 = int.Parse(Console.ReadLine());
            int p2 = int.Parse(Console.ReadLine());
            Player player1 = players[p1 - 1];
            Player player2 = players[p2 - 1];
            Game game = new Game();
            player1.Game = game;
            player1.PrintPosition = 0;
            player2.Game = game;
            player2.PrintPosition = 19;
            game.Player1 = player1;
            game.Player2 = player2;
            player1.OpponentName = player2.Name;
            player2.OpponentName = player1.Name;
            Console.WriteLine("Hur många spel skall spelas?");
            int numberOfGames = int.Parse(Console.ReadLine());
            Console.WriteLine("Vilket spel skall skrivas ut? (0 = Inget spel)");
            int gameToPrint = int.Parse(Console.ReadLine());
            //Console.WriteLine("Skriva ut första spelet? (y/n)");
            //string print = Console.ReadLine();
            Console.Clear();
            //if (print == "y")
            //    game.Printlevel = 2;
            //else
            //    game.Printlevel = 0;
            //game.initialize(true);
            //int result = game.PlayAGame(true);
            int result;
            //Console.Clear();
            bool player1starts = true;
            int player1Wins = 0;
            int player2Wins = 0;
            int totalRounds = game.NbrOfRounds;

            //if (result == 1)
            //{
            //    player1Wins++;
            //}
            //else if (result == 2)
            //{
            //    player2Wins++;
            //}
            bool fillout = false;
            for (int i = 1; i <= numberOfGames; i++)
            {
                player1starts = !player1starts;
                bool fair = game.initialize(false);
                if (game.Printlevel == 2)
                {
                    Console.Clear();
                    game.Printlevel = 0;
                    gameToPrint = numberOfGames + 1;
                    fillout = true;
                }
                else if (gameToPrint <= i && gameToPrint != 0 && fair)
                {
                    Console.ReadKey();
                    Console.Clear();
                    game.Printlevel = 2;
                    Console.CursorVisible = true;
                }
                else
                {
                    Console.CursorVisible = false;
                    game.Printlevel = 0;
                    fillout = false;
                }

                result = game.PlayAGame(player1starts);
                totalRounds += game.NbrOfRounds;
                if (result == 1)
                {
                    player1Wins++;
                }
                else if (result == 2)
                {
                    player2Wins++;
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 3);
                Console.Write(player1.Name + ":");
                Console.ForegroundColor = ConsoleColor.Green;
                if (fillout)
                {
                    Console.SetCursorPosition(15, 3);
                    int x = 0;
                    while (x < player1Wins * 100 / numberOfGames)
                    {
                        Console.Write("█");
                        x++;
                    }
                }
                Console.SetCursorPosition((player1Wins * 100 / numberOfGames) + 15, 3);
                Console.Write("█");
                Console.SetCursorPosition((player1Wins * 100 / numberOfGames) + 16, 3);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(player1Wins);

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(0, 5);
                Console.Write(player2.Name + ":");
                Console.ForegroundColor = ConsoleColor.Red;
                if (fillout)
                {
                    Console.SetCursorPosition(15, 5);
                    int x = 0;
                    while (x < player2Wins * 100 / numberOfGames)
                    {
                        Console.Write("█");
                        x++;
                    }
                }
                Console.SetCursorPosition((player2Wins * 100 / numberOfGames) + 15, 5);
                Console.Write("█");
                Console.SetCursorPosition((player2Wins * 100 / numberOfGames) + 16, 5);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(player2Wins);
                Console.SetCursorPosition(25, 7);
                Console.Write(player1.Name);
                Console.SetCursorPosition(45, 7);
                Console.WriteLine(player2.Name);
                Console.WriteLine("          Vunna spel:");
                Console.WriteLine("            Skillnad:                                        ");
                Console.WriteLine("Antal ronder i snitt:");
                Console.WriteLine("       Andel bluffar:");
                Console.WriteLine("   Varav misslyckade:");
                Console.WriteLine("    Andel bluffstopp:");
                Console.WriteLine("      Varav korrekta:");
                Console.SetCursorPosition(25 + player1.Name.Length / 2, 8);
                Console.Write(player1Wins);
                Console.SetCursorPosition(25 + player1.Name.Length / 2, 9);
                int diff = player1Wins - player2Wins;
                if (diff > 0)
                {
                    Console.Write("+" + diff);
                }
                else
                {
                    Console.Write("      ");
                }
                Console.SetCursorPosition(30 + player1.Name.Length, 10);
                double avgRounds = Math.Round((double)totalRounds / i, 2);
                Console.Write(avgRounds.ToString("F"));
                Console.SetCursorPosition(25 + player1.Name.Length / 2, 11);
                double bluffPercent1 = Math.Round((double)player1.totalNumberOfBluffs * 100 / player1.totalNumberOfBluffChances, 1);
                Console.Write(bluffPercent1.ToString("F") + " %");
                Console.SetCursorPosition(25 + player1.Name.Length / 2, 12);
                double badBluffPercent1 = Math.Round((double)player1.badBluffs * 100 / player1.totalNumberOfBluffs, 1);
                Console.Write(badBluffPercent1.ToString("F") + " %");
                Console.SetCursorPosition(25 + player1.Name.Length / 2, 13);
                double callPercent1 = Math.Round((double)player1.totalNumberOfCalls * 100 / player1.totalNumberOfCallChances, 1);
                Console.Write(callPercent1.ToString("F") + " %");
                Console.SetCursorPosition(25 + player1.Name.Length / 2, 14);
                double goodCallPercent1 = Math.Round((double)player1.goodCalls * 100 / player1.totalNumberOfCalls, 1);
                Console.Write(goodCallPercent1.ToString("F") + " %");

                Console.SetCursorPosition(45 + player2.Name.Length / 2, 8);
                Console.Write(player2Wins);
                Console.SetCursorPosition(45 + player2.Name.Length / 2, 9);
                diff = player2Wins - player1Wins;
                if (diff > 0)
                {
                    Console.Write("+" + diff);
                }
                else
                {
                    Console.Write("      ");
                }

                Console.SetCursorPosition(45 + player2.Name.Length / 2, 11);
                double bluffPercent2 = Math.Round((double)player2.totalNumberOfBluffs * 100 / player2.totalNumberOfBluffChances, 1);
                Console.Write(bluffPercent2.ToString("F") + " %");
                Console.SetCursorPosition(45 + player2.Name.Length / 2, 12);
                double badBluffPercent2 = Math.Round((double)(player2.badBluffs) * 100 / player2.totalNumberOfBluffs, 1);
                Console.Write(badBluffPercent2.ToString("F") + " %");
                Console.SetCursorPosition(45 + player2.Name.Length / 2, 13);
                double callPercent2 = Math.Round((double)player2.totalNumberOfCalls * 100 / player2.totalNumberOfCallChances, 1);
                Console.Write(callPercent2.ToString("F") + " %");
                Console.SetCursorPosition(45 + player2.Name.Length / 2, 14);
                double goodCallPercent2 = Math.Round((double)player2.goodCalls * 100 / player2.totalNumberOfCalls, 1);
                Console.Write(goodCallPercent2.ToString("F") + " %");


            }

            Console.ReadKey();
            Console.ReadKey();
        }
    }

    public class Card
    {
        public int Value { get; private set; } //Kortets värde enligt reglerna i Bluffstopp, t.ex. dam = 12
        public Suit Suit { get; private set; }

        //public int Id { get; private set; } //Typ av kort, t.ex dam = 12

        public Card(int value, Suit suit)
        {
            Suit = suit;
            Value = value;

        }

        public void PrintCard()
        {
            string cardname = "";
            if (Value == 14)
            {
                cardname = "ess    ";
            }
            else if (Value == 11)
            {
                cardname = "knekt  ";
            }
            else if (Value == 12)
            {
                cardname = "dam     ";
            }
            else if (Value == 13)
            {
                cardname = "kung   ";
            }
            else
            {
                cardname = Value + "      ";
            }
            if (Suit == Suit.Hjärter)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (Suit == Suit.Ruter)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (Suit == Suit.Spader)
                Console.ForegroundColor = ConsoleColor.Gray;
            else if (Suit == Suit.Klöver)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(" " + Suit + " " + cardname);
            if (cardname == "")
            {
                Console.Write(Value + " ");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Card card = (Card)obj;
            if (card.Value == Value && card.Suit == Suit)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    public class Game
    {
        public List<Card> CardDeck = new List<Card>();
        List<Card> DiscardPile = new List<Card>();
        public bool FirstRound = true;
        public int CurrentValue;
        public Suit CurrentSuit;
        public int LastCurrentValue;
        private Card RealCard;
        int Cardnumber;
        public int Printlevel;
        public int Discardnumber;
        public Player Player1 { private get; set; }
        public Player Player2 { private get; set; }
        Random RNG = new Random();
        public int NbrOfRounds;
        private string Reason;

        public Game()
        {

        }
        public bool initialize(bool firstGame)
        {
            Cardnumber = -1;
            Discardnumber = 52;
            CardDeck = new List<Card>();
            DiscardPile = new List<Card>();
            Player1.Hand = new List<Card>();
            Player2.Hand = new List<Card>();
            Player1.OpponentLatestCard = null;
            Player2.OpponentLatestCard = null;
            int value;
            int suit;
            for (int i = 0; i < 52; i++)
            {
                value = i % 13 + 2;
                suit = i % 4;
                CardDeck.Add(new Card(value, (Suit)suit));
            }
            Shuffle();
            int p1Sum = 0;
            int p2Sum = 0;
            for (int i = 0; i < 7; i++)
            {
                Player1.Hand.Add(DrawCard());
                Player2.Hand.Add(DrawCard());
                p1Sum += Player1.Hand[i].Value;
                p2Sum += Player2.Hand[i].Value;
            }
            Card first = DrawCard();
            Discard(first);
            Player1.OpponentLatestCard = first;
            Player2.OpponentLatestCard = first;
            if (Math.Abs(p1Sum - p2Sum) <= 5)
                return true;
            else
                return false;
        }
        public int PlayAGame(bool player1starts)
        {
            NbrOfRounds = 0;
            FirstRound = true;
            Player playerInTurn, playerNotInTurn, temp;
            if (player1starts)
            {
                playerInTurn = Player1;
                playerNotInTurn = Player2;
            }
            else
            {
                playerInTurn = Player2;
                playerNotInTurn = Player1;
            }
            Player1.OpponentLatestCard = null;
            Player2.OpponentLatestCard = null;
            while (Cardnumber < 48 && NbrOfRounds < 100)
            {
                NbrOfRounds++;
                bool result = PlayARound(playerInTurn, playerNotInTurn);
                FirstRound = false;
                if (result)
                {
                    if (Printlevel > 1)
                        printHand(playerNotInTurn);
                    playerNotInTurn.SpelSlut(playerNotInTurn.Hand.Count, playerInTurn.Hand.Count);
                    playerInTurn.SpelSlut(playerInTurn.Hand.Count, playerNotInTurn.Hand.Count);
                    if (Printlevel > 0)
                    {
                        Console.SetCursorPosition(15, playerNotInTurn.PrintPosition + 7);
                        Console.Write(playerNotInTurn.Name + " fick slut på kort och vann spelet!");
                        Console.ReadLine();
                    }
                    if (Player1 == playerNotInTurn)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else
                {
                    temp = playerNotInTurn;
                    playerNotInTurn = playerInTurn;
                    playerInTurn = temp;
                }
            }

            if (Printlevel > 0)
            {
                Console.SetCursorPosition(15, 20);
                Console.WriteLine("Korten tog slut utan att någon spelare vann.");
                Console.ReadLine();
            }
            playerInTurn.SpelSlut(playerInTurn.Hand.Count, playerNotInTurn.Hand.Count);
            playerNotInTurn.SpelSlut(playerNotInTurn.Hand.Count, playerInTurn.Hand.Count);

            return 0;
        }

        public void StateReason(string reason)
        {
            if (Reason.Length + reason.Length < 100)
            {
                Reason += reason + ". ";
            }
            else
            {
                Reason += " Långt!";
            }

        }

        private void ShowReason(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(x, y);
            Console.Write(Reason);
            for (int i = Reason.Length; i < 100; i++)
            {
                Console.Write(" ");
            }
            Reason = "";
            Console.ResetColor();
        }
        private void printHand(Player player)
        {
            Console.SetCursorPosition(0, player.PrintPosition);
            Console.WriteLine(player.Name + " har ");
            for (int i = 0; i < player.Hand.Count; i++)
            {
                player.Hand[i].PrintCard();
                Console.WriteLine();
            }
            Console.Write("              ");
        }

        private bool PlayARound(Player player, Player otherPlayer)
        {
            Reason = "";
            if (FirstRound)
            {
                CurrentSuit = DiscardPile.Last().Suit;
                CurrentValue = DiscardPile.Last().Value;
            }
            if (Printlevel > 1)
            {
                printHand(player);
                Console.SetCursorPosition(7, 13);
                if (FirstRound)
                {
                    Console.Write("Första rundan. På skräphögen ligger ");
                    DiscardPile.Last().PrintCard();
                }
                else
                {
                    Console.Write("                                                          ");
                }
            }
            if (!FirstRound && CurrentValue != 0)
            {
                player.totalNumberOfCallChances++;
            }
            otherPlayer.WasLastCardCalled = false;
            if (!FirstRound && CurrentValue != 0 && player.BluffStopp(CurrentValue, CurrentSuit, LastCurrentValue))
            {
                otherPlayer.WasLastCardCalled = true;
                otherPlayer.DidOpponentPass = false;
                player.totalNumberOfCalls++;
                player.OpponentLatestCard = new Card(CurrentValue, CurrentSuit);
                if (Printlevel > 1)
                {
                    Console.SetCursorPosition(20, player.PrintPosition + 4);
                    Console.Write(player.Name + " säger Bluffstopp!                                      ");
                    ShowReason(20, player.PrintPosition + 5);

                }
                if (TrueCard())
                {
                    player.Hand.Add(DrawCard());
                    player.Hand.Add(DrawCard());
                    CurrentValue = 0;
                    LastCurrentValue = 0;
                    CurrentSuit = Suit.Wild;
                    player.WasLastCallCorrect = false;
                    if (Printlevel > 1)
                    {
                        Console.SetCursorPosition(20, player.PrintPosition + 6);
                        Console.Write(player.Name + " hade fel och får ta upp två straffkort.                  ");
                        Console.SetCursorPosition(20, player.PrintPosition + 7);
                        Console.Write(player.Name + " tog ");
                        player.Hand[player.Hand.Count - 1].PrintCard();
                        player.Hand[player.Hand.Count - 2].PrintCard();
                        printHand(player);
                        Console.ReadKey();
                        Console.SetCursorPosition(20, player.PrintPosition + 7);
                        Console.Write("                                                                          ");

                    }
                    if (otherPlayer.Hand.Count() == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    otherPlayer.Hand.Add(DrawCard());
                    otherPlayer.Hand.Add(DrawCard());
                    otherPlayer.Hand.Add(DrawCard());
                    player.WasLastCallCorrect = true;
                    player.goodCalls++;
                    otherPlayer.badBluffs++;

                    CurrentValue = 0;
                    CurrentSuit = Suit.Wild;
                    LastCurrentValue = 0;
                    if (Printlevel > 1)
                    {
                        Console.SetCursorPosition(20, player.PrintPosition + 6);
                        Console.Write(player.Name + " hade rätt! " + otherPlayer.Name + " får ta upp tre straffkort.                 ");
                        Console.SetCursorPosition(20, player.PrintPosition + 7);
                        Console.Write(otherPlayer.Name + " tog ");
                        otherPlayer.Hand[otherPlayer.Hand.Count - 1].PrintCard();
                        otherPlayer.Hand[otherPlayer.Hand.Count - 2].PrintCard();
                        otherPlayer.Hand[otherPlayer.Hand.Count - 3].PrintCard();
                        printHand(otherPlayer);
                        Console.ReadKey();
                        Console.SetCursorPosition(20, player.PrintPosition + 7);
                        Console.Write("                                                                          ");
                    }
                }
            }
            if (otherPlayer.Hand.Count() == 0)
            {
                return true;
            }
            RealCard = null;
            if (CurrentValue < 14)
            {
                RealCard = player.LäggEttKort(CurrentValue, CurrentSuit);
                player.totalNumberOfBluffChances++;
                if (RealCard != null && !player.Hand.Contains(RealCard))
                {
                    Console.SetCursorPosition(60, 0);
                    Console.WriteLine("Fusk! " + player.Name + " försöker spela ett felaktigt kort");
                    Console.ReadKey();
                    return true;
                }
                player.Hand.Remove(RealCard);
            }
            if (RealCard != null)
            {
                Card fakeCard = player.SägEttKort(CurrentValue, CurrentSuit);
                fakeCard = FixFakeValue(fakeCard);
                LastCurrentValue = CurrentValue;
                otherPlayer.DidOpponentPass = false;
                CurrentValue = fakeCard.Value;
                CurrentSuit = fakeCard.Suit;
                if (!TrueCard())
                {
                    player.totalNumberOfBluffs++;
                }
                if (Printlevel > 1)
                {
                    Console.SetCursorPosition(20, player.PrintPosition + 4);
                    Console.Write(player.Name + " spelar ");
                    RealCard.PrintCard();
                    Console.Write("                        ");
                    Console.SetCursorPosition(20, player.PrintPosition + 5);
                    Console.Write(player.Name + " säger att kortet är ");
                    fakeCard.PrintCard();
                    Console.Write("                        ");
                    ShowReason(20, player.PrintPosition + 6);
                    printHand(player);

                    ClearMessage(otherPlayer.PrintPosition + 4);

                    if (LastCurrentValue != 0 && !FirstRound)
                    {
                        Console.SetCursorPosition(20, 13);
                        Console.Write("På skräphögen låg:                                        ");
                        Console.SetCursorPosition(38, 13);
                        new Card(LastCurrentValue, CurrentSuit).PrintCard();
                    }

                    Console.ReadKey();
                }
            }
            else
            {
                //player.Hand.Add(DrawCard());
                otherPlayer.DidOpponentPass = true;
                CurrentValue = 0;
                CurrentSuit = Suit.Wild;
                LastCurrentValue = 0;
                if (Printlevel > 1)
                {
                    Console.SetCursorPosition(20, player.PrintPosition + 4);
                    //Console.Write(player.Name + " säger pass och får ta ett straffkort");
                    Console.Write(player.Name + " säger pass");
                    ShowReason(20, player.PrintPosition + 5);
                    //Console.SetCursorPosition(20, player.PrintPosition + 6);
                    //Console.Write(player.Name + " tog ");
                    //player.Hand.Last().PrintCard();
                    Console.Write("                           ");
                    printHand(player);
                    Console.ReadKey();
                }
            }
            return false;
        }

        private void ClearMessage(int yOffset)
        {
            for (int y = 0; y < 6; y++)
            {
                Console.SetCursorPosition(20, yOffset + y);
                for (int x = 0; x < 100; x++)
                {
                    Console.Write(" ");
                }
            }

        }

        private Card FixFakeValue(Card fakeCard)
        {
            if (CurrentValue == 0)
            {
                if (fakeCard.Value > 14 || fakeCard.Value < 2 || (int)fakeCard.Suit > 3)
                {
                    return new Card(2, Suit.Hjärter);
                }
            }
            else
            {
                if (fakeCard.Suit != CurrentSuit || fakeCard.Value <= CurrentValue || fakeCard.Value > 14)
                {
                    return new Card(CurrentValue + 1, CurrentSuit);
                }
            }
            return fakeCard;
        }

        private bool TrueCard()
        {
            if (RealCard.Value == CurrentValue && RealCard.Suit == CurrentSuit)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void Discard(Card card)
        {
            Discardnumber--;
            DiscardPile.Add(card);
        }

        private Card DrawCard()
        {
            Cardnumber++;
            Card card = CardDeck.First();
            CardDeck.RemoveAt(0);
            return card;
        }

        private Card PickDiscarded()
        {
            Card card = DiscardPile.Last();
            Discardnumber++;
            return card;
        }



        private void Shuffle()
        {
            for (int i = 0; i < 200; i++)
            {
                switchCards();
            }
        }

        private void switchCards()
        {
            int card1 = RNG.Next(CardDeck.Count);
            int card2 = RNG.Next(CardDeck.Count);
            Card temp = CardDeck[card1];
            CardDeck[card1] = CardDeck[card2];
            CardDeck[card2] = temp;
        }

        public int opponentHandSize(Player askingPlayer) //Returnerar antalet kort motståndaren har. Anropa med (this) som parameter.
        {
            if (askingPlayer == Player1)
            {
                return Player2.Hand.Count;
            }
            else
            {
                return Player1.Hand.Count;
            }
        }
        public List<Card> SortHandBySuit(List<Card> hand) //Sorterar handen efter suit. Hjärter->ruter->spader->klöver.
        {
            return hand.OrderBy(o => o.Suit).ToList();
        }
        public List<Card> SortHandByValue(List<Card> hand) //Sorterar handen efter värde, lägst först.
        {
            return hand.OrderBy(o => o.Value).ToList();
        }

        public double ProbabilityOfCardBetterThan(int value, int handSize) //Returnerar sannolikheten för att en spelare med handSize antal kort på handen har ett kort i rätt suit med värde över value
        {
            double prob = (double)(14 - value) / 52;
            prob = 1.0 - Math.Pow(1.0 - prob, handSize);
            return prob;
        }

        public List<Card> MostCommonSuitCards(List<Card> hand) //Returnerar en lista med korten i den vanligaste suiten på handen
        {
            List<Card>[] SuitLists = new List<Card>[4];
            for (int i = 0; i < 4; i++)
            {
                SuitLists[i] = new List<Card>();
            }
            for (int i = 0; i < hand.Count; i++)
            {
                SuitLists[(int)hand[i].Suit].Add(hand[i]);
            }
            int bestNumberOfCards = 0;
            int bestSuit = 0;
            for (int i = 0; i < 4; i++)
            {
                if (SuitLists[i].Count > bestNumberOfCards)
                {
                    bestNumberOfCards = SuitLists[i].Count;
                    bestSuit = i;
                }
            }
            return SuitLists[bestSuit];
        }
    }

    public abstract class Player
    {
        public string Name;

        // Dessa variabler får ej ändras
        public List<Card> Hand = new List<Card>();  // Lista med alla kort i handen.
        public int PrintPosition;
        public Game Game;
        public bool DidOpponentPass; //True om motståndaren sa "pass" på ditt förra kort
        public bool WasLastCardCalled; //True om motstånadren sa "bluffstopp" på förra kortet
        public bool WasLastCallCorrect; //True om spelarens senaste "bluffstopp" var korrekt
        public Card OpponentLatestCard; //Det senaste av motståndarens kort som spelaren fick se. Null om spelaren inte fått se något kort.
        public string OpponentName;  //Motståndarens namn

        // Dessa variabler används för att samla statistik. Får ej ändras, men får användas.
        public int totalNumberOfBluffChances; //Antalet chanser spelaren haft att bluffa
        public int totalNumberOfBluffs;  //Antalet gånger spelaren bluffat
        public int totalNumberOfCallChances;  //Antalet chanser spelaren haft att säga bluffstopp
        public int totalNumberOfCalls;   //Antalet gånger spelaren sagt bluffstopp
        public int badBluffs;  //Antalet misslyckade bluffar spelaren gjort
        public int goodCalls;  //Antalet gånger som spelaren sagt bluffstopp och haft rätt


        public abstract bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat);
        //Skall returnera true om spelaren vill säga bluffstopp.
        //cardValue och cardSuit är värden på kortet motståndaren sa. cardValueToBeat är värdet på förra kortet.
        public abstract Card LäggEttKort(int cardValue, Suit cardSuit);
        //Skall returnera det kort spelaren vill lägga. Kortet måste finnas på spelarens hand.
        //Om spelaren väljer att säga pass, returnera null.
        //cardValue och cardSuit är värden på kortet motståndaren sa. cardSuit = Suit.Wild om valfritt kort kan läggas.
        public abstract Card SägEttKort(int cardValue, Suit cardSuit);
        //Skall returnera det kort spelaren säger att den lägger. Kortet måste vara högre och i samma Suit som det som ligger.
        //cardValue och cardSuit är värden på kortet motståndaren sa. cardSuit = Suit.Wild om valfritt kort kan läggas.

        public abstract void SpelSlut(int cardsLeft, int opponentCardsLeft);
        //Anropas varje gång ett spel tar slut. Kan vara bra att använda för att nollställa listor etc.
    }



    class HonestPlayer : Player //Denna spelare bluffar aldrig, och tror aldrig att moståndaren bluffar
    {
        Card kortJagSpelade; //Håller reda på vilket kort som spelaren lagt.
        public HonestPlayer()
        {
            Name = "HonestPlayer";
        }

        public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
        {
            return false; //Säger aldrig bluffstopp!
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit)
        {
            Hand = Game.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
            if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
            {
                Game.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                kortJagSpelade = Hand[0]; //Sparar kortet som skall spelas
                return Hand[0]; //Spelar ut det första kortet på handen
            }
            for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
            {
                if (Hand[i].Suit == cardSuit && Hand[i].Value > cardValue) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
                {
                    Game.StateReason("Jag lägger det lägsta kortet jag kan");
                    kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas
                    return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            Game.StateReason("Jag kan inte lägga något kort och tänker inte bluffa");
            return null; //Om inget kort hittats säger spelaren pass
        }

        public override Card SägEttKort(int cardValue, Suit cardSuit)
        {
            Game.StateReason("Jag bluffar aldrig");
            return kortJagSpelade; //Talar alltid sanning!
        }


        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            //Behöver inte användas.
        }
    }

    class MyPlayer : Player //Denna spelare skall du göra till din egen! Nu har den exakt samma strategi som HonestPlayer.
    {
        Card kortJagSpelade; //Håller reda på vilket kort som spelaren lagt.
        public MyPlayer()
        {
            Name = "MyPlayer";
        }

        public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
        {
            return true; //Säger aldrig bluffstopp!
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit)
        {
            Hand = Game.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
            if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
            {
                Game.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                kortJagSpelade = Hand[0]; //Sparar kortet som skall spelas
                return Hand[0]; //Spelar ut det första kortet på handen
            }
            for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
            {
                if (Hand[i].Suit == cardSuit && Hand[i].Value > cardValue) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
                {
                    Game.StateReason("Jag lägger det lägsta kortet jag kan");
                    kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas
                    return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            Game.StateReason("Jag kan inte lägga något kort och tänker inte bluffa");
            return null; //Om inget kort hittats säger spelaren pass
        }

        public override Card SägEttKort(int cardValue, Suit cardSuit)
        {
            Game.StateReason("Jag bluffar aldrig");
            return kortJagSpelade; //Talar alltid sanning!
        }


        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            //Behöver inte användas.
        }
    }

    class RandomPlayer : Player
    //Denna spelare bluffar ibland och synar(säger bluffstopp) ibland beroende på vilka värden som skrivs in i konstruktorn
    {
        Card kortJagSpelade; //Håller reda på vilket kort som spelaren lagt.
        int BluffProcent;  //Sannolikheten för bluff(i %) om spelaren inte har ett ok kort att spela
        int SynProcent;  //Sannolikheten för Bluffstopp (i %)
        bool Bluff;  //Håller koll på om spelaren bluffat
        Random RNG;

        public RandomPlayer(int bluffProcent, int synProcent)  //Konstruktor
        {
            Name = "Random " + bluffProcent + " " + synProcent;
            BluffProcent = bluffProcent;
            SynProcent = synProcent;
            RNG = new Random();
        }

        public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
        {
            if (SynProcent > RNG.Next(100)) //Slumpar fram om den skall säga bluffstopp
            {
                Game.StateReason("Jag slumpar fram ett bluffstopp. Sannolikhet " + SynProcent + " %");
                return true;
            }
            else
            {
                //Man behövet inte ange en anledning till att inte säga bluffstopp
                return false;
            }
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit)
        {
            Bluff = false;
            Hand = Game.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
            if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
            {
                Game.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                kortJagSpelade = Hand[0]; //Sparar kortet som skall spelas
                return Hand[0]; //Spelar ut det första kortet på handen
            }
            for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
            {
                if (Hand[i].Suit == cardSuit && Hand[i].Value > cardValue) //Om den hittar ett kort på handen som är högre och i smma suit som det som ligger
                {
                    Game.StateReason("Jag lägger det lägsta kortet jag kan.");
                    kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas
                    return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            if (BluffProcent > RNG.Next(100)) //Om inget Ok kort hittats slumpar spelaren om den tänker bluffa
            {
                Game.StateReason("Jag slumpar fram en bluff, sannolikhet " + BluffProcent + " %");
                Bluff = true;
                return Hand[0]; //Här tänker spelaren bluffa och spelar ut sitt lägsta kort
            }
            else
            {
                Game.StateReason("Jag kan inte lägga något kort, och säger pass");
                Bluff = false;
                return null;  //Spelaren väljer att inte bluffa och säger pass
            }
        }

        public override Card SägEttKort(int cardValue, Suit cardSuit)
        {
            if (Bluff == true) //Om spelaren bestämt sig för att bluffa
            {
                int fakeCardValue = cardValue + 1;  //Sätter värdet på fake-kortet till ett högre än det som ligger
                Game.StateReason("Jag säger alltid att kortet är 1 högre");
                return new Card(fakeCardValue, cardSuit);  //Skapar fake-kortet och returnerar det
            }
            else
            {
                Game.StateReason("Jag bluffar inte");
                return kortJagSpelade; //Om spelaren valt att inte bluffa, säger samma kort som den spelat.
            }
        }

        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            //Behöver inte användas.
        }
    }
}