using BluffStopp_SU22;

namespace AI;

class MyPlayer2 : Player
{
    List<Card> SpeladeKort = new List<Card>();
    List<Card> potentionellaSpeladeKort = new List<Card>();
    Card kortJagSpelade;
    public MyPlayer2()
    {
        Name = "JakobHot2";
    }

    public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
    {

        if (SpeladeKort.Contains(new Card(cardValue, cardSuit)))
        {
            return true;
        }
        if (potentionellaSpeladeKort.Contains(new Card(cardValue, cardSuit)))
        {
            return true;
        }
        potentionellaSpeladeKort.Add(new Card(cardValue, cardSuit));
        return false; //Säger aldrig bluffstopp!
    }

    public override Card LäggEttKort(int cardValue, Suit cardSuit)
    {
        Hand = Game.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
        if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
        {
            Game.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
            kortJagSpelade = Hand[0]; //Sparar kortet som skall spelas 
            SpeladeKort.Add(kortJagSpelade);
            return Hand[0]; //Spelar ut det första kortet på handen
        }
        for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
        {
            if (Hand[i].Suit == cardSuit && Hand[i].Value > cardValue) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
            {

                Game.StateReason("Jag lägger det lägsta kortet jag kan");
                kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas 
                SpeladeKort.Add(kortJagSpelade);
                return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit

            }
            if (Hand[i].Suit != cardSuit && Hand[i].Value > cardValue)
            {
                if (Hand.Count == 1)
                {
                    return null;
                }

            }
        }
        kortJagSpelade = Hand[0];
        SpeladeKort.Add(kortJagSpelade);
        return kortJagSpelade;

    }

    public override Card SägEttKort(int cardValue, Suit cardSuit)
    {
        if (kortJagSpelade.Suit == cardSuit && kortJagSpelade.Value < cardValue)
        {
            SpeladeKort.Add(kortJagSpelade);
            return new Card(cardValue++, cardSuit);
        }
        SpeladeKort.Add(kortJagSpelade);
        return kortJagSpelade;

    }


    public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
    {
        SpeladeKort = new List<Card>();
        potentionellaSpeladeKort = new List<Card>();
    }
}