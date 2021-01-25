using MTCGserver.Classes.Battle_Logic;

namespace MTCGserver.Classes.Cards.Monster {
    class NormalSpellCard : Card {

        public NormalSpellCard() {

            CardType = TypeSpell;
            ElementType = ElementTypeNormal;
        }

        public override Card getLosingCard(Card defendingCard) {

            if (defendingCard.GetType() == typeof(KrakenCard)) {
                return this;
            }

            return base.getLosingCard(defendingCard);
        }
    }
}
