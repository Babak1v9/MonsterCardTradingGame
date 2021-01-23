using MTCGserver.Classes.Cards.Monster;

namespace MTCGserver.Classes.Cards.Spells {
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
