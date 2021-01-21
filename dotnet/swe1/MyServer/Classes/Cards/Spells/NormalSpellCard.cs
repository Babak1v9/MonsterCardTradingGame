using System;
using System.Collections.Generic;
using System.Text;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.Cards.Monster;

namespace MyServer.Classes.Battle_Stuff {
    class NormalSpellCard : Card {

        public NormalSpellCard() {

            CardType = TypeSpell;
            ElementType = ElementTypeNormal;
        }

        public override Card CalculateDamageAgainst(Card defendingCard) {

            if (defendingCard.GetType() == typeof(KrakenCard)) {
                return this;
            }

            return base.CalculateDamageAgainst(defendingCard);
        }
    }
}
