
using MTCGserver.Classes.Battle_Logic;

namespace MTCGserver.Classes.Cards.Monster {
    class FireSpellCard : Card {

        public FireSpellCard() {
            CardType = TypeSpell;
            ElementType = ElementTypeFire;
        }

        public override Card getLosingCard(Card defendingCard) {

            if (defendingCard.GetType() == typeof(KrakenCard)) {
                return this;
            }

            return base.getLosingCard(defendingCard);
        }
    }
}
