using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using BluffStopp_SU22;

namespace AI
{
    public sealed class Tonk3 : Player
    {
        private static readonly Card[] FullDeck =
        {
            new(2, Suit.Hjärter),
            new(3, Suit.Hjärter),
            new(4, Suit.Hjärter),
            new(5, Suit.Hjärter),
            new(6, Suit.Hjärter),
            new(7, Suit.Hjärter),
            new(8, Suit.Hjärter),
            new(9, Suit.Hjärter),
            new(10, Suit.Hjärter),
            new(11, Suit.Hjärter),
            new(12, Suit.Hjärter),
            new(13, Suit.Hjärter),
            new(14, Suit.Hjärter),

            new(2, Suit.Ruter),
            new(3, Suit.Ruter),
            new(4, Suit.Ruter),
            new(5, Suit.Ruter),
            new(6, Suit.Ruter),
            new(7, Suit.Ruter),
            new(8, Suit.Ruter),
            new(9, Suit.Ruter),
            new(10, Suit.Ruter),
            new(11, Suit.Ruter),
            new(12, Suit.Ruter),
            new(13, Suit.Ruter),
            new(14, Suit.Ruter),

            new(2, Suit.Klöver),
            new(3, Suit.Klöver),
            new(4, Suit.Klöver),
            new(5, Suit.Klöver),
            new(6, Suit.Klöver),
            new(7, Suit.Klöver),
            new(8, Suit.Klöver),
            new(9, Suit.Klöver),
            new(10, Suit.Klöver),
            new(11, Suit.Klöver),
            new(12, Suit.Klöver),
            new(13, Suit.Klöver),
            new(14, Suit.Klöver),

            new(2, Suit.Spader),
            new(3, Suit.Spader),
            new(4, Suit.Spader),
            new(5, Suit.Spader),
            new(6, Suit.Spader),
            new(7, Suit.Spader),
            new(8, Suit.Spader),
            new(9, Suit.Spader),
            new(10, Suit.Spader),
            new(11, Suit.Spader),
            new(12, Suit.Spader),
            new(13, Suit.Spader),
            new(14, Suit.Spader)
        };

        private Card? LaidCard { get; set; }
        private bool Bluff { get; set; }
        private Random Rng { get; set; }
        private bool CalledBluff { get; set; }
        private bool WasCorrect { get; set; }

        private List<Card> KnownPlayedCards { get; set; }
        private List<Card> PotentialPlayedCards { get; set; }

        // Learning
        private List<Card> BluffedCards { get; set; }
        private List<Card> OpponentPlayedCards { get; set; }
        private int NumberOfOpponentBluffs { get; set; }
        private int NumberOfOpponentCorrectCalls { get; set; }
        private readonly int[] _heatMap = new int[15];
        private readonly double[] _heatMapNormalized = new double[15];

        private readonly int[] _heatMapOppCalls = new int[15];
        private readonly double[] _heatMapOppCallsNormalized = new double[15];

        private int PlayedRounds { get; set; }

        internal Tonk3()
        {
            this.Name = "tonk3";
            this.LaidCard = null;
            this.Bluff = false;
            this.Rng = new Random();
            this.CalledBluff = false;
            this.WasCorrect = false;

            this.KnownPlayedCards = new List<Card>();
            this.PotentialPlayedCards = new List<Card>();

            // Learning
            this.BluffedCards = new List<Card>();
            this.OpponentPlayedCards = new List<Card>();
            this.NumberOfOpponentBluffs = 0;
            this.NumberOfOpponentCorrectCalls = 0;
            this.PlayedRounds = 0;
        }

        public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat) // Figure out when to call a bluff
        {
            this.PlayedRounds++;

            this.OpponentPlayedCards.Add(new Card(cardValue, cardSuit));

            if (this.Bluff && !this.WasLastCardCalled)
            {
                this.NumberOfOpponentCorrectCalls++;

                this._heatMapOppCalls[this.BluffedCards[^1].Value]++;

                int total = this._heatMapOppCalls.Sum();

                if (total != 0)
                {
                    int minVal = this._heatMapOppCalls.Min();
                    for (int i = 0; i < this._heatMapOppCalls.Length; i++)
                    {
                        this._heatMapOppCallsNormalized[i] = (this._heatMapOppCalls[i] - minVal) / (double)total;
                    }
                }
            }

            if (this.CalledBluff && this.WasLastCallCorrect)
            {
                this.NumberOfOpponentBluffs++;

                this._heatMap[this.OpponentPlayedCards[^1].Value]++;

                int total = this._heatMap.Sum();

                if (total != 0)
                {
                    int minVal = this._heatMap.Min();
                    for (int i = 0; i < this._heatMap.Length; i++)
                    {
                        this._heatMapNormalized[i] = (this._heatMap[i] - minVal) / (double)total;
                    }
                }
            }

            if (this.CalledBluff && this.WasLastCallCorrect) this.WasCorrect = true;

            if (this.PotentialPlayedCards.Count >= 1)
            {
                Card playedCard = this.PotentialPlayedCards[^1];

                if (this.WasCorrect)
                {
                    this.KnownPlayedCards.Add(playedCard);

                    this.PotentialPlayedCards.RemoveAt(this.PotentialPlayedCards.Count - 1);
                }

                Card[] commonCards = this.Hand.Intersect(this.PotentialPlayedCards).ToArray();

                if (commonCards.Length != 0)
                {
                    foreach (Card card in commonCards)
                    {
                        this.KnownPlayedCards.Add(card);

                        this.PotentialPlayedCards.RemoveAt(this.PotentialPlayedCards.FindIndex((c) => c == card));
                    }
                }
            }

            this.PotentialPlayedCards.Add(new Card(cardValue, cardSuit));

            this.CalledBluff = false;

            this.Game.SortHandByValue(this.Hand);

            Card opponentCard = new(cardValue, cardSuit);

            if (this.Hand.Contains(opponentCard))
            {
                this.CalledBluff = true;

                this.Game.StateReason("My hand contains the card and thus I know that it is a bluff, I call the bluff");

                return true;
            }

            if (this.Game.opponentHandSize(this) == 0)
            {
                this.CalledBluff = true;

                this.Game.StateReason("My opponent has no cards left so if I dont call I will lose instantly, but If I call and I am correct I might prolong my suffering some additional rounds yes");

                return true;
            }

            if (this.KnownPlayedCards.Contains(opponentCard))
            {
                this.CalledBluff = true;

                this.Game.StateReason("");

                return true;
            }

            double chanceOfOpBluff = (this.NumberOfOpponentBluffs) / (double)this.PlayedRounds; // Decimal form
            double chanceOfHeatMapAgreeing = this._heatMapNormalized[cardValue] * 10;

            double chanceOfBluff = chanceOfOpBluff * chanceOfHeatMapAgreeing;

            /*if (chanceOfBluff < ((this.NumberOfOpponentBluffs / (float)this.PlayedRounds) < 0.5 ? 0.26 : 0.13))
            {
                return false;
            }

            this.CalledBluff = true;
            return true;
            */

            if (chanceOfBluff >= 0.26)
            {
                this.CalledBluff = true;
                return true;
            }


            return false;
        }

        private static int Magic(int[] array, int n)
        {
            return Array.IndexOf(array, array.OrderByDescending(static x => x).Take(n).First());
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit) // Draw a card
        {
            this.Game.SortHandByValue(this.Hand);
            this.Bluff = false;

            int[] suitCount = new int[4];

            for (int i = 0; i < 4; i++)
            {
                suitCount[i] = this.Hand.Count(card => card.Suit == (Suit)i);
            }

            int n = 1;
            int suitWithLoLowestAmountOfCardsIndex = Tonk3.Magic(suitCount, n);
            while (suitCount[suitWithLoLowestAmountOfCardsIndex] == 0)
            {
                n++;
                suitWithLoLowestAmountOfCardsIndex = Tonk3.Magic(suitCount, n);
            }

            Card lowestPossible = this.Hand.Where(card => card.Suit == (Suit)suitWithLoLowestAmountOfCardsIndex)
                .OrderByDescending(static card => card.Value).Last();

            // If other player passes
            if (cardSuit == Suit.Wild)
            {
                // Play the lowest card
                this.LaidCard = lowestPossible;
                this.KnownPlayedCards.Add(this.LaidCard);
                return this.LaidCard;
            }

            for (int i = 0; i < this.Hand.Count; i++)
            {
                if ((this.Hand[i].Suit == cardSuit) && (this.Hand[i].Value > cardValue))
                {
                    this.LaidCard = this.Hand[i];
                    this.KnownPlayedCards.Add(this.LaidCard);

                    return this.LaidCard;
                }
            }

            if (cardValue < 6)
            {
                this.Game.StateReason("Jag bluffar eftersom värdet inte är så högt");
                this.Bluff = true;
                this.LaidCard = lowestPossible;
                this.KnownPlayedCards.Add(this.LaidCard);
                return this.LaidCard;
            }

            double lol = (this._heatMap.Sum() / (double)this.PlayedRounds);

            switch (lol)
            {
                case 0.0f:
                    break;
                case < 1.0f / 8:
                    this.Bluff = true;
                    this.LaidCard = this.Hand[0];
                    break;
                case > 0.8f:
                    break;
            }

            this.Bluff = true;
            this.LaidCard = this.Hand[0];

            this.KnownPlayedCards.Add(this.LaidCard);

            return this.LaidCard;
        }

        public override Card SägEttKort(int cardValue, Suit cardSuit) // Tell the truth or bluff
        {
            if (!this.Bluff) return this.LaidCard!; // If player dont bluff

            var toldCard = new Card(cardValue + 1, cardSuit);
            BluffedCards.Add(toldCard);
            return toldCard;
/*
 
            List<Card> possible = new();

            foreach (Card card in Tonk3.FullDeck.Where((c) => (c.Suit == cardSuit) && (c.Value > cardValue)))
            {
                if (!this.KnownPlayedCards.Contains(card) && !this.PotentialPlayedCards.Contains(card) && (card != this.LaidCard)) possible.Add(card);
            }

            if (possible.Count == 0)
            {
                foreach (Card card in Tonk3.FullDeck.Where((c) => (c.Suit == cardSuit) && (c.Value > cardValue)))
                {
                    if (!this.KnownPlayedCards.Contains(card) && (card != this.LaidCard)) possible.Add(card);
                }
            }

            if (possible.Count == 0)
            {
                possible.Add(new Card(this.Rng.Next(cardValue + 1, 15), cardSuit));
            }

            possible = possible.OrderByDescending(card => card.Value).Reverse().Take(3).ToList();

            Card toldCard = possible[this.Rng.Next(0, possible.Count)];

            this.BluffedCards.Add(toldCard);

            return toldCard;
            */
        }

        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            this.KnownPlayedCards = new List<Card>();
            this.PotentialPlayedCards = new List<Card>();
            this.OpponentPlayedCards = new List<Card>();
        }
    }
}