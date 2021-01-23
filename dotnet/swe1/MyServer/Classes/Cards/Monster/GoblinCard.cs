using MyServer.Classes.Battle_Stuff;

namespace MyServer.Classes.Cards.Monster {
    class GoblinCard : Card {

        public GoblinCard() {

            CardType = TypeMonster;
        }

        public override Card CalculateDamageAgainst(Card defendingCard) {

            if (defendingCard.GetType() == typeof(DragonCard)) {

                return this; //instant lose
            }
            return base.CalculateDamageAgainst(defendingCard);
        }
    }
}
