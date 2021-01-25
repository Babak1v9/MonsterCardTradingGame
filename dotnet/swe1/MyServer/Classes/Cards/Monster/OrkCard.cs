using MTCGserver.Classes.Battle_Logic;

namespace MTCGserver.Classes.Cards.Monster {
    class OrkCard : Card {

        public OrkCard() {

            CardType = TypeMonster;
        }

        public override Card getLosingCard(Card defendingCard) {

            if (defendingCard.GetType() == typeof(WizardCard)) {

                return this;
            }
            return base.getLosingCard(defendingCard);
        }
    }
}
