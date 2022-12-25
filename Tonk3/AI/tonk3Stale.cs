using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BluffStopp_SU22;

namespace AI
{
    public sealed class Tonk3Stale : Player
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

        internal Tonk3Stale()
        {
            this.Name = "tonk3Stale";
            this.LaidCard = null;
            this.Bluff = false;
            this.Rng = new Random();
            this.CalledBluff = false;
            this.WasCorrect = false;

            this.KnownPlayedCards = new List<Card>();
            this.PotentialPlayedCards = new List<Card>();
        }

        public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat) // Figure out when to call a bluff
        {
            if (this.CalledBluff && this.WasLastCallCorrect) this.WasCorrect = true;

            if (this.PotentialPlayedCards.Count >= 1)
            {
                Card playedCard = this.PotentialPlayedCards[^1];

                this.KnownPlayedCards.Add((this.WasCorrect) ? this.OpponentLatestCard : playedCard);

                this.PotentialPlayedCards.RemoveAt(this.PotentialPlayedCards.Count - 1);
            }

            this.PotentialPlayedCards.Add(new Card(cardValue, cardSuit));

            this.CalledBluff = false;

            this.Game.SortHandByValue(this.Hand);

            Card opponentCard = new(cardValue, cardSuit);

            if (this.Hand.Contains(opponentCard))
            {
                this.CalledBluff = true;
                return true;
            }

            if (this.Game.opponentHandSize(this) == 0)
            {
                this.CalledBluff = true;
                return true;
            }

            if (this.KnownPlayedCards.Contains(opponentCard))
            {
                this.CalledBluff = true;
                return true;
            }

            return false;
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit) // Draw a card
        {
            this.Game.SortHandByValue(this.Hand);
            this.Bluff = false;

            // If other player passes
            if (cardSuit == Suit.Wild)
            {
                // Play the lowest card
                this.LaidCard = this.Hand[0];
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

            this.Bluff = true;
            this.LaidCard = this.Hand[0];

            this.KnownPlayedCards.Add(this.LaidCard);

            return this.LaidCard;
        }

        public override Card SägEttKort(int cardValue, Suit cardSuit) // Tell the truth or bluff
        {
            if (!this.Bluff) return this.LaidCard!; // If player dont bluff

            List<Card> possible = new();

            foreach (Card card in Tonk3Stale.FullDeck.Where((c) => (c.Suit == cardSuit) && (c.Value > cardValue)))
            {
                if (!this.KnownPlayedCards.Contains(card) && !this.PotentialPlayedCards.Contains(card)) possible.Add(card);
            }

            if (possible.Count == 0)
            {
                foreach (Card card in Tonk3Stale.FullDeck.Where((c) => (c.Suit == cardSuit) && (c.Value > cardValue)))
                {
                    if (!this.KnownPlayedCards.Contains(card)) possible.Add(card);
                }
            }

            if (possible.Count == 0)
            {
                possible.Add(new Card(this.Rng.Next(cardValue + 1, 15), cardSuit));
            }

            return possible[this.Rng.Next(0, possible.Count)];
        }

        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            this.KnownPlayedCards = new List<Card>();
            this.PotentialPlayedCards = new List<Card>();
        }
    }
}