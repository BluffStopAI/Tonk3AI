using BluffStopp_SU22;

namespace AI;

internal class Rev3 : Player
{
    private List<Card> _cardsPlayed;
    private int _roundsPlayed;
    private Card _lastCardSaid; // last card we said we played (could be lie)
    private int _lastHandSize;  // hand size last round
    
    private bool _calledBluff;  // if this player called bluff last round (or this round depending on when accessing)
    
    private Card _prevOppCard;
    private int  _prevOppHandSize = 7;

    private readonly int[]
        _bluffValues         = new int[15],
        _handSizeValues      = new int[20],
        _callValues          = new int[15],
        _handSizeCallValues  = new int[20];

    private double[]
        _bluffWeight         = new double[15],
        _handSizeWeight      = new double[20],
        _callWeight          = new double[15],
        _handSizeCallWeight  = new double[20];

    public Rev3()
    {
        Name = "Rev3";
    }
    
    public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
    {
        Card card = new(cardValue, cardSuit);

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
        if (WasLastCardCalled)
        {
            _callValues[_lastCardSaid.Value]++;
            _handSizeCallValues[_lastHandSize]++;
        }

        _prevOppCard = card;
        _prevOppHandSize = Game.opponentHandSize(this);
        _calledBluff = false;

        _bluffWeight        = Normalise(_bluffValues);
        _handSizeWeight     = Normalise(_handSizeValues);
        _callWeight         = Normalise(_callValues);
        _handSizeCallWeight = Normalise(_handSizeValues);

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

        return false;
    }

    public override Card LäggEttKort(int cardValue, Suit cardSuit)
    {
        if (Hand.Count == 1 && Game.opponentHandSize(this) >= 3)
        {
            return null;
        }

        return null;
    }

    public override Card SägEttKort(int cardValue, Suit cardSuit)
    {
        throw new NotImplementedException();
    }

    public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
    {
        _prevOppCard = null!;
        _prevOppHandSize = 7;
        _calledBluff = false;
    }

    private static double[] Normalise(IReadOnlyList<int> values)
    {
        double[] weights = new double[values.Count];
        
        int total = values.Sum();

        if (total == 0)
        {
            for (int i = 0; i < weights.Length; i++)
                weights[i] = 0;
            return weights;
        }
        
        int minValue = values.Min();
        for (int i = 0; i < weights.Length; i++)
            weights[i] = (values[i] - minValue) / (float)total;

        return weights;
    }
}