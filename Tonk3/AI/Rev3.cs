using BluffStopp_SU22;

namespace AI;

internal class Rev3 : Player
{
    public Rev3()
    {
        Name = "Rev3";
    }
    
    public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
    {
        throw new NotImplementedException();
    }

    public override Card LäggEttKort(int cardValue, Suit cardSuit)
    {
        throw new NotImplementedException();
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