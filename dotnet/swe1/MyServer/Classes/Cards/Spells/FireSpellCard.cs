
using MyServer.Classes.Cards.Monster;

namespace MyServer.Classes.Battle_Stuff {
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
