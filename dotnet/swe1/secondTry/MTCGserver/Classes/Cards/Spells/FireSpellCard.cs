﻿using MTCGserver.Classes.Cards.Monster;

namespace MTCGserver.Classes.Cards.Spells {
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