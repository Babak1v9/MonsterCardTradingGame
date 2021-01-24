using MyServer.Classes.Cards.Monster;

namespace MyServer.Classes.Battle_Stuff {
    class WaterSpellCard : Card{

        public WaterSpellCard() {

            CardType = TypeSpell;
            ElementType = ElementTypeWater;
        }

        public override Card getLosingCard(Card defendingCard) {

            if (defendingCard.GetType() == typeof(KnightCard)) {
                return defendingCard;
            }

            if (defendingCard.GetType() == typeof(KrakenCard)) {
                return this;
            }

            return base.getLosingCard(defendingCard);
        }
    }
}
