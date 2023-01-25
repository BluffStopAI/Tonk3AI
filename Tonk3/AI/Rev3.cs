using BluffStopp_SU22;

namespace AI;

internal class Rev3 : Player
{
    private List<Card> _cardsPlayed = new();
    private int _roundsPlayed;
    private Card _lastCardSaid; // last card we said we played (could be lie)
    private int _lastHandSize;  // hand size last round

    private bool _bluffing;
    private bool _calledBluff;  // if this player called bluff last round (or this round depending on when accessing)
    
    private Card _prevOppCard;
    private int  _prevOppHandSize = 7;

    private readonly int[]
        _bluffValues        = new int[15],
        _handSizeValues     = new int[20],
        _callValues         = new int[15],
        _handSizeCallValues = new int[20],
        _myBluffValues      = new int[15];

    private double[]
        _bluffWeight        = new double[15],
        _handSizeWeight     = new double[20],
        _callWeight         = new double[15],
        _handSizeCallWeight = new double[20];

    public Rev3()
    {
        Name = "Rev3";
    }
    
    public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
    {
        Card card = new(cardValue, cardSuit);
        int oppHandSize = Game.opponentHandSize(this);

        // collect opponent bluff statistics
        if (_calledBluff && WasLastCallCorrect)
        {
            //if (_prevOppCard != null)
            // ^ is necessary?
            {
                _cardsPlayed.Add(_prevOppCard);
                _bluffValues[_prevOppCard.Value]++;
                _handSizeValues[_prevOppHandSize]++;
            }
        }
        
        // collect opponent call statistics
        if (WasLastCardCalled && _lastCardSaid != null!)
        {
            _callValues[_lastCardSaid.Value]++;
            _handSizeCallValues[_lastHandSize]++;
        }

        _prevOppCard = card;
        _prevOppHandSize = Game.opponentHandSize(this);
        _calledBluff = false;

        // binary criteria that guarantee a return statement
        if (Hand.Count == 1 && Game.opponentHandSize(this) >= 3)
        {
            return false;
        }
        
        if (_cardsPlayed.Contains(card))
        {
            return true;
        }

        if (Hand.Contains(card))
        {
            return true;
        }

        if (Game.opponentHandSize(this) == 1)
        {
            return true;
        }
        
        // if none of the binary criteria are met, calculate suspicion
        _bluffWeight        = Normalise(_bluffValues);
        _handSizeWeight     = Normalise(_handSizeValues);
        _callWeight         = Normalise(_callValues, _myBluffValues);
        _handSizeCallWeight = Normalise(_handSizeValues);

        double sus = 0;

        if (_bluffWeight[card.Value] > 0.2)
        {
            sus += _bluffWeight[card.Value];
        }

        if (_handSizeWeight[oppHandSize] > 0.2)
        {
            sus += _handSizeWeight[oppHandSize] / 2;
        }

        sus += card.Value / 40d;

        return sus > 0.8;
    }

    public override Card LäggEttKort(int cardValue, Suit cardSuit)
    {
        _bluffing = false;
        Hand = Game.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
        if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
        {
            Game.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
            _lastCardSaid = Hand[0]; //Sparar kortet som skall spelas 
            _cardsPlayed.Add(_lastCardSaid);
            return Hand[0]; //Spelar ut det första kortet på handen
        }
        for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
        {
            if (Hand[i].Suit == cardSuit && Hand[i].Value > cardValue) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
            {
                Game.StateReason("Jag lägger det lägsta kortet jag kan");
                _lastCardSaid = Hand[i]; //Sparar kortet som skall spelas 
                _cardsPlayed.Add(_lastCardSaid);
                return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
            }
        }
        if (cardValue < 10)
        {
            Game.StateReason("Jag bluffar eftersom värdet inte är så högt");
            _bluffing = true;
            return Hand[0];
        }

        Game.StateReason("Jag kan inte lägga något kort och tänker inte bluffa");
        return null; //Om inget kort hittats säger spelaren pass
    }

    public override Card SägEttKort(int cardValue, Suit cardSuit)
    {
        Card cardPlayed = new(cardValue, cardSuit);

        // bluff baiting
        // only bait if card is low value and there is another card in hand
        if (cardPlayed.Value < 5 && Hand.Count > 1)
        {
            Random rnd = new();
            
            if (rnd.NextDouble() > 0.5)
            {
                // pick a low value card from your deck that isn't the one you played
                Card cardPicked = (Game.FirstRound || DidOpponentPass
                    ? Hand.Find(card =>
                        !card.Equals(cardPlayed)
                    )
                    : Hand.Find(card =>
                        !card.Equals(cardPlayed) &&
                        cardValue > _prevOppCard.Value &&
                        cardSuit == _prevOppCard.Suit
                    )) ?? new Card(0, Suit.Wild);

                if (!cardPicked.Equals(new Card(0, Suit.Wild)))
                {
                    Game.StateReason("Jag bluffar med ett kort jag har för att lura motståndaren");
                    return cardPicked;
                }
            }
        }
        
        // if bluffing was decided in the LäggEttKort method
        if (_bluffing)
        {
            Game.StateReason("Jag säger att värdet är 1 högre");
            _myBluffValues[cardValue + 1]++;
            return new Card(cardValue + 1, cardSuit);
        }
        
        Game.StateReason("Jag bluffar inte");
        return new Card(cardValue, cardSuit); // Passar
    }

    public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
    {
        _prevOppCard = new Card(0, Suit.Wild);
        _prevOppHandSize = 7;
        _calledBluff = false;
        _cardsPlayed = new List<Card>();
    }

    private static double[] Normalise(IReadOnlyList<int> values1, IReadOnlyList<int> values2 = null!)
    {
        double[] weights = new double[values1.Count];
        
        int total = values1.Sum();

        if (total == 0)
        {
            for (int i = 0; i < weights.Length; i++)
                weights[i] = 0;
            return weights;
        }
        
        /*int minValue = values.Min();
        for (int i = 0; i < weights.Length; i++)
            weights[i] = (values[i] - minValue) / (float)total;*/
        
        for (int i = 0; i < weights.Length; i++)
            weights[i] = values1[i] / (values2 != null ? (double)Math.Max(values2[i], 1) : 1) / (float)total;

        return weights;
    }
}