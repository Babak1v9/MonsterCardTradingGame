using MTCGserver.Classes.Battle_Logic;

namespace MTCGserver.Classes.Cards.Monster {
    class GoblinCard : Card {

        public GoblinCard() {

            CardType = TypeMonster;
        }

        public override Card getLosingCard(Card defendingCard) {

            if (defendingCard.GetType() == typeof(DragonCard)) {

                return this; 
            }
            return base.getLosingCard(defendingCard);
        }
    }
}
