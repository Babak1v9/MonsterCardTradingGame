using System;
using System.Collections.Generic;
using System.Text;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.Cards.Monster;

namespace MyServer.Classes.Battle_Stuff {
    class FireSpellCard : Card {

        public FireSpellCard() {
            CardType = TypeSpell;
            ElementType = ElementTypeFire;
        }

        public override Card CalculateDamageAgainst(Card defendingCard) {

            if (defendingCard.GetType() == typeof(KrakenCard)) {
                return this;
                //kraken immune to spells
            }

            return base.CalculateDamageAgainst(defendingCard);
        }
    }
}
