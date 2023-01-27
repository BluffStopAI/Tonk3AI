using BluffStopp_SU22;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluffstop
{
    class Tobias : Player
    //Denna spelare bluffar ibland och synar(säger bluffstopp) ibland beroende på vilka värden som skrivs in i konstruktorn
    {
        Card kortJagSpelade; //Håller reda på vilket kort som spelaren lagt.
        int BluffProcent;  //Sannolikheten för bluff(i %) om spelaren inte har ett ok kort att spela
        int SynProcent;  //Sannolikheten för Bluffstopp (i %)
        bool Bluff;  //Håller koll på om spelaren bluffat
        Random RNG;
        List<Card> AllaKortViSpelat = new List<Card>();
        List<Card> AllaKortMotståndarenSagt = new List<Card>();
        Suit VaranSenasteFarg;
        bool MotståndarenPassade = false;
        int AntalExaktEnMer = 0;
        bool SenasteExaktEnMer = false;
        int LyckadeExaktEnMer = 0;
        bool ForstättaMedExaktEnMer = true;
        bool ViKanLägga = false;
        int MotstandarenÄrPåSistaMenViHarHögt = 0;
        int AntalKungOchEss = 0;
        bool SenasteKungOchEss = false;
        int LyckadeKungOchEss = 0;
        bool FortsättMedKungOchEss = true;


        public Tobias(int bluffProcent, int synProcent)  //Konstruktor
        {
            Name = "Tobias";
            BluffProcent = bluffProcent;
            SynProcent = synProcent;
            RNG = new Random();
        }

        public override bool BluffStopp(int cardValue, Suit cardSuit, int cardValueToBeat)
        {

            if (OpponentLatestCard != null && !AllaKortViSpelat.Contains(OpponentLatestCard))
            {
                AllaKortViSpelat.Add(OpponentLatestCard);
            }

            AllaKortMotståndarenSagt.Add(new Card(cardValueToBeat, cardSuit));

            if (DidOpponentPass == true)
            {
                MotståndarenPassade = true;
            }

            if (SenasteExaktEnMer == true)
            {
                if (WasLastCallCorrect == true)
                {
                    LyckadeExaktEnMer++;
                }
                else
                {
                    LyckadeExaktEnMer--;
                }
                SenasteExaktEnMer = false;
            }

            if (SenasteKungOchEss == true)
            {
                if (WasLastCallCorrect == true)
                {
                    LyckadeKungOchEss++;
                }
                else
                {
                    LyckadeKungOchEss--;
                }
                SenasteExaktEnMer = false;
            }

            if (AntalExaktEnMer >= 2000001)
            {
                if (LyckadeExaktEnMer < 0)
                {
                    ForstättaMedExaktEnMer = false;
                }
                //else
                //{
                //    ForstättaMedExaktEnMer = true;
                //}
            }

            if (AntalKungOchEss >= 2000001)
            {
                if (LyckadeKungOchEss < 0)
                {
                    FortsättMedKungOchEss = false;
                }
                //else
                //{
                //    FortsättMedKungOchEss = true;
                //}
            }

            AllaKortViSpelat.Add(kortJagSpelade);
            Card Kortet = new Card(cardValue, cardSuit);
            Suit spelarensFarg = cardSuit;

            if (Hand.Contains(Kortet))
            {
                Game.StateReason("Kortet som spelats är på hand, det är en säker bluff");
                return true;
            }

            else if (Game.opponentHandSize(this) <= 1)
            {
                for (int i = Hand.Count; i < 0; i--)
                {
                    if (Hand[i].Value > 10 && Hand[i].Suit == spelarensFarg)
                    {
                        ViKanLägga = true;
                        MotstandarenÄrPåSistaMenViHarHögt = i;
                        return false;
                    }
                }

                Game.StateReason("Motståndaren är på sina sista kort, bäst att bluffstoppa.");
                return true;
            }

            else if (AllaKortViSpelat.Contains(Kortet))
            {
                Game.StateReason("Kortet har redan spelats så det måste vara un bluff");
                return true;
            }

            else if (MotståndarenPassade == true)
            {
                Game.StateReason("Motståndaren passde denna färg tidigare");
                return true;
            }

            else if (AllaKortViSpelat.Count > 0 && (cardValue == cardValueToBeat + 1) && ForstättaMedExaktEnMer == true)  //|| AllaKortMotståndarenSagt.Contains(new Card(cardValue, cardSuit)
            {
                Game.StateReason("Motståndaren lägger ett kort som är exakt en mer");
                AntalExaktEnMer++;
                SenasteExaktEnMer = true;
                return true;
            }

            else if (FortsättMedKungOchEss == true && (cardValue == 13 || cardValue == 14))
            {
                Game.StateReason("Motståndaren lägger högt kort");
                AntalKungOchEss++;
                SenasteKungOchEss = true;
                return true;
            }

            else
            {
                return false;
            }

        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit)
        {

            Bluff = false;
            Hand = Game.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
            if (ViKanLägga == true)
            {
                Game.StateReason("Motståndaren har få kort men vi har högt");
                kortJagSpelade = Hand[MotstandarenÄrPåSistaMenViHarHögt];
                return Hand[MotstandarenÄrPåSistaMenViHarHögt];
            }

            if (cardSuit == Suit.Wild && DidOpponentPass == true)
            {
                for (int i = 0; i < Hand.Count; i++)
                {
                    if (Hand[i].Suit == VaranSenasteFarg)
                    {
                        Game.StateReason("Motståndaren passade på denna färg tidigare så jag lägger lägsta av samma färg");
                        VaranSenasteFarg = Hand[i].Suit;
                        kortJagSpelade = Hand[i];
                        return Hand[i];
                    }
                }

                Game.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                VaranSenasteFarg = Hand[0].Suit;
                kortJagSpelade = Hand[0];
                MotståndarenPassade = false;
                return Hand[0];

            }

            else if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
            {
                Game.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                VaranSenasteFarg = Hand[0].Suit;
                kortJagSpelade = Hand[0]; //Sparar kortet som skall spelas
                MotståndarenPassade = false;
                return Hand[0]; //Spelar ut det första kortet på handen
            }

            for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
            {
                if (Hand[i].Suit == cardSuit && Hand[i].Value > cardValue) //Om den hittar ett kort på handen som är högre och i smma suit som det som ligger
                {
                    Game.StateReason("Jag lägger det lägsta kortet jag kan.");
                    VaranSenasteFarg = Hand[i].Suit;
                    kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas 
                    MotståndarenPassade = false;
                    return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }

            if (BluffProcent > RNG.Next(100)) //Om inget Ok kort hittats slumpar spelaren om den tänker bluffa 
            {
                if (Hand.Count == 1)
                {
                    Game.StateReason("Vi är på vårat sista kort så vi vill inte bluffa");
                    Bluff = false;
                    MotståndarenPassade = false;
                    return null;
                }

                double dålig = 0.6;
                if (totalNumberOfBluffs > 0 && dålig > badBluffs / totalNumberOfBluffs)
                {
                    for (int i = cardValue + 1; i < 13; i++)
                    {

                        if (!AllaKortViSpelat.Contains(new Card(i, cardSuit)) || !AllaKortMotståndarenSagt.Contains(new Card(i, cardSuit)))
                        {
                            Game.StateReason("Jag slumpar fram en bluff för det finns en bra");
                            Bluff = true;
                            MotståndarenPassade = false;
                            return Hand[0];
                        }

                    }
                    Game.StateReason("Det finns inga bra kort att bluffa");
                    return null;
                }

                for (int i = cardValue + 1; i < 15; i++)
                {

                    if (!AllaKortViSpelat.Contains(new Card(i, cardSuit)) || !AllaKortMotståndarenSagt.Contains(new Card(i, cardSuit)))
                    {
                        Game.StateReason("Jag slumpar fram en bluff för det finns en bra");
                        Bluff = true;
                        MotståndarenPassade = false;
                        return Hand[0];
                    }

                }
                Game.StateReason("Det finns inga bra kort att bluffa");
                return null;
            }

            else
            {
                Game.StateReason("Jag kan inte lägga något kort, och säger pass");
                Bluff = false;
                MotståndarenPassade = false;
                return null;  //Spelaren väljer att inte bluffa och säger pass
            }
        }

        public override Card SägEttKort(int cardValue, Suit cardSuit)
        {
            if (ViKanLägga == true)
            {
                Game.StateReason("Vi har ett högt kort mot motståndarens få kort");
                ViKanLägga = false;
                return Hand[MotstandarenÄrPåSistaMenViHarHögt];
            }
            if (Bluff == true) //Om spelaren bestämt sig för att bluffa
            {

                int fakeCardValue = RNG.Next(14 - cardValue + 2, 15);       //Om det går fel, fixa detta
                int ändra = -1;
                fakeCardValue += ändra;


                while (true)
                {

                    if (fakeCardValue <= cardValue)
                    {
                        ändra = 1;
                    }

                    if (AllaKortViSpelat.Contains(new Card(fakeCardValue, cardSuit)) || AllaKortMotståndarenSagt.Contains(new Card(fakeCardValue, cardSuit)))
                    {
                        fakeCardValue += ändra;
                    }

                    else
                    {
                        AllaKortMotståndarenSagt.Add(new Card(fakeCardValue, cardSuit));
                        VaranSenasteFarg = cardSuit;
                        return new Card(fakeCardValue, cardSuit);
                    }
                }

            }
            else
            {
                Game.StateReason("Jag bluffar inte");
                return kortJagSpelade; //Om spelaren valt att inte bluffa, säger samma kort som den spelat.
            }
        }

        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            AllaKortMotståndarenSagt.Clear();
            VaranSenasteFarg = Suit.Wild;
            MotståndarenPassade = false;
            AllaKortViSpelat.Clear();
            SenasteExaktEnMer = false;
        }
    }
}