using BluffStopp_SU22;

namespace AI;

internal class Rev3 : Player
{
    private List<Card> _cardsPlayed;
    private int _roundsPlayed;
    
    private bool _calledBluff; // if this player called bluff last round (or this round depending on when accessing)
    
    private Card _prevOppCard;
    private int  _prevOppHandSize = 7;

    private readonly int[]
        _oppBluffValues     = new int[15],
        _oppHandSizeValues  = new int[20],
        _myBluffValues      = new int[15],
        _myHandSizeValues   = new int[20];

    private double[]
        _oppBluffWeight     = new double[15],
        _oppHandSizeWeight  = new double[20],
        _myBluffWeight      = new double[15],
        _myHandSizeWeight   = new double[20];

    public Rev3()
    {
        Name = "Rev3";
    }
    
    public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
    {
        Card card = new(cardValue, cardSuit);

        if (_calledBluff && WasLastCallCorrect)
        {
            //if (_prevOppCard != null)
            // ^ is necessary?
            {
                _cardsPlayed.Add(_prevOppCard);
                _oppBluffValues[_prevOppCard.Value - 1]++;
                _oppHandSizeWeight[_prevOppHandSize]++;
            }
        }

        _prevOppCard = card;
        _prevOppHandSize = Game.opponentHandSize(this);
        _calledBluff = false;

        _oppBluffWeight     = Normalise(_oppBluffValues);
        _oppHandSizeWeight  = Normalise(_oppHandSizeValues);

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