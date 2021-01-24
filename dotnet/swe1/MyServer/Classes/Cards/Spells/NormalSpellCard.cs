using MyServer.Classes.Cards.Monster;

namespace MyServer.Classes.Battle_Stuff {
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
