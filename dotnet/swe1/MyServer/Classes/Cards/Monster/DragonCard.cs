using MyServer.Classes.Battle_Stuff;

namespace MyServer.Classes.Cards.Monster {
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
