using MTCGserver.Classes.Battle_Logic;

namespace MTCGserver.Classes.Cards.Monster {
    class DragonCard : Card {

        public DragonCard() {
            CardType = TypeMonster;
        }

        public override Card getLosingCard(Card defendingCard) {

            if (defendingCard.GetType() == typeof(FireElveCard)) {

                return this;
            }
            return base.getLosingCard(defendingCard);
        }
    }
}
