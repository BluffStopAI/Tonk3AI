using BluffStopp_SU22;

internal class Rev2 : Player //Denna spelare skall du göra till din egen! Nu har den exakt samma strategi som HonestPlayer.
{
    private Card _kortJagSpelade; //Håller reda på vilket kort som spelaren lagt.
    private List<Card> _cardsPlayed = new List<Card>(); // Lista med alla kort som har spelats
    private bool Bluffing;

    public Rev2()
    {
        Name = "Rev2";
    }

    public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
    {
        Card cardPlayed = new Card(cardValue, cardSuit);
        bool isBluff = false;

        if (Hand.Contains(cardPlayed))
        {
            Game.StateReason("Det kortet finns i min hand");
            isBluff = true;
        }
        else if (_cardsPlayed.Contains(cardPlayed))
        {
            Game.StateReason("Det kortet har redan spelats");
            isBluff = true;
        }
                
        else if (Game.opponentHandSize(this) == 0) 
        {
            Game.StateReason("Det är motståndarens sista kort, finns inget bättre val");
            isBluff = true;
        }
        /*else if (cardValueToBeat > 10) Behövs inte användas förrens rev1 kan bluffa.
        {
            Game.StateReason("Det är ett väldigt högt värde och jag tror fan inte på det.");
            isBluff = false;
        }*/
        return isBluff; 
    }

    public override Card LäggEttKort(int cardValue, Suit cardSuit)
    {
        Bluffing = false;
        Hand = Game.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
        if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
        {
            Game.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
            _kortJagSpelade = Hand[0]; //Sparar kortet som skall spelas 
            _cardsPlayed.Add(_kortJagSpelade);
            return Hand[0]; //Spelar ut det första kortet på handen
        }
        for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
        {
            if (Hand[i].Suit == cardSuit && Hand[i].Value > cardValue) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
            {
                Game.StateReason("Jag lägger det lägsta kortet jag kan");
                _kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas 
                _cardsPlayed.Add(_kortJagSpelade);
                return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
            }
        }
        if (cardValue < 10)
        {
            Game.StateReason("Jag bluffar eftersom värdet inte är så högt");
            Bluffing = true;
            return Hand[0];
        }

        Game.StateReason("Jag kan inte lägga något kort och tänker inte bluffa");
        return null; //Om inget kort hittats säger spelaren pass
    }

    public override Card SägEttKort(int cardValue, Suit cardSuit)
    {
        if (Bluffing)
        {
            Game.StateReason("Jag säger att värdet är 1 högre.");
            return new Card(cardValue+1,cardSuit);
        }
        else
        {
            Game.StateReason("Jag bluffar inte");
            return _kortJagSpelade; // Passar
        }

    }


    public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
    {
        _cardsPlayed = new List<Card>();
        _kortJagSpelade = null;
    }
}