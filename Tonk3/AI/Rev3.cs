using System.ComponentModel.Design;
using BluffStopp_SU22;

namespace AI;

internal class Rev3 : Player
{
    private List<Card> _cardsPlayed;
    private int _roundsPlayed;

    private int[]
        _oppBluffValues,
        _myBluffValues;

    private double[]
        _oppBluffWeight,
        _myBluffWeight;

    public Rev3()
    {
        Name = "Rev3";
    }
    
    public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
    {
        Card card = new(cardValue, cardSuit);

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
        throw new NotImplementedException();
    }
}