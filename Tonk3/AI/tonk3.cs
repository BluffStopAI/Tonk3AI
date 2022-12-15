using BluffStopp_SU22;

namespace AI
{
    internal sealed class opponentInfo
    {
        public Card? PossiblyPlayedCard { get; set; }
        public bool CalledBluff { get; set; }
        public bool CalledCorrectly { get; set; }

        public opponentInfo(Card card, bool cb, bool cc)
        {
            this.PossiblyPlayedCard = card;
            this.CalledBluff = cb;
            this.CalledCorrectly = cc;
        }
    }

    internal sealed class Tonk3 : Player
    {
        private Card? CardJustPlayed { get; set; }
        private bool Bluff { get; set; }
        private Random RNg { get; set; }

        private List<Card> PlayerPlayedCards { get; set; }
        private List<opponentInfo> PotentialOpponentPlayedCards { get; set; }

        internal Tonk3()
        {
            this.Name = "tonk3";
            this.CardJustPlayed = null;
            this.Bluff = false;
            this.RNg = new Random();
            this.PlayerPlayedCards = new List<Card>();
            this.PotentialOpponentPlayedCards = new List<opponentInfo>();
        }

        public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat) // Figure out when to call a bluff
        {
            this.Game.SortHandByValue(this.Hand);

            if (this.PotentialOpponentPlayedCards.Count != 0)
            {
                if (this.PotentialOpponentPlayedCards[^1].CalledBluff && this.WasLastCallCorrect)
                {
                    this.PotentialOpponentPlayedCards[^1].CalledCorrectly = true;
                }
            }

            this.PotentialOpponentPlayedCards.Add(new opponentInfo(new Card(cardValue, cardSuit), false, false));

            if (this.Hand.Contains(new Card(cardValue, cardSuit)))
            {
                this.PotentialOpponentPlayedCards[^1].CalledBluff = true;
                this.PotentialOpponentPlayedCards[^1].CalledCorrectly = true;

                return true;
            }

            if (this.Game.opponentHandSize(this) == 0)
            {
                this.PotentialOpponentPlayedCards[^1].CalledBluff = true;
                this.PotentialOpponentPlayedCards[^1].CalledCorrectly = false;

                return true;
            }

            if (this.PlayerPlayedCards.Contains(new Card(cardValue, cardSuit)))
            {
                this.PotentialOpponentPlayedCards[^1].CalledBluff = true;
                this.PotentialOpponentPlayedCards[^1].CalledCorrectly = false;

                return true;
            }

            return this.PotentialOpponentPlayedCards.Find((pMove) => (new Card(cardValue, cardSuit) == pMove.PossiblyPlayedCard!) &&
                                                                     pMove is { CalledBluff: true, CalledCorrectly: false }) != null;
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit) // Draw a card
        {
            this.Bluff = false;
            this.Game.SortHandByValue(this.Hand);

            if (cardSuit == Suit.Wild)
            {
                this.CardJustPlayed = this.Hand[0];
                this.Game.StateReason("I can lay whatever so I am choosing to get rid of the lower cards since they wont be necessary when the game has progressed");
                this.PlayerPlayedCards.Add(this.Hand[0]);

                return this.Hand[0];
            }

            for (int i = 0; i < this.Hand.Count; i++)
            {
                if ((this.Hand[i].Suit == cardSuit) && (this.Hand[i].Value > cardValue))
                {
                    this.CardJustPlayed = this.Hand[i];
                    this.PlayerPlayedCards.Add(this.Hand[i]);

                    return this.Hand[i];
                }
            }

            if (48 > this.RNg.Next(100))
            {
                this.Bluff = true;

                this.PlayerPlayedCards.Add(this.Hand[0]);
                return this.Hand[0];
            }
            else
            {
                this.Bluff = false;
                return null;
            }
        }

        public override Card SägEttKort(int cardValue, Suit cardSuit) // Tell the truth or bluff
        {
            if (!this.Bluff) return this.CardJustPlayed; // If player dont bluff

            int fakeCardValue = this.RNg.Next(cardValue + 1, 15); // Sets a value for a fake card to a random number in the limit
            this.Game.StateReason("I always bluff a random value in between the cardValue and the highest card available, this is to keep my opponent always guessing and keep them from spotting a pattern");
            return new Card(fakeCardValue, cardSuit); // Creates and returns the fake card
        }

        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            this.PlayerPlayedCards = new List<Card>();
        }
    }
}