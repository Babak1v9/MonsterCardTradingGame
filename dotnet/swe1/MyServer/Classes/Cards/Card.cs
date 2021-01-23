

namespace MyServer.Classes.Battle_Stuff {
    class Card {

        public const int ElementTypeNormal = 0;
        public const int ElementTypeWater = 1;
        public const int ElementTypeFire = 2;

        public const int TypeMonster = 0;
        public const int TypeSpell = 1;

        public string Name { get; set; }
        public float Damage { get; set; }
        public int CardType { get; set; }
        public int ElementType { get; set; }

        public virtual Card CalculateDamageAgainst(Card defendingCard) {

            if (CardType == TypeMonster && defendingCard.CardType == TypeMonster) {
                return CalcPlainDamage(defendingCard);
            }

            return CalcElementDamage(defendingCard);
        }

        protected Card CalcPlainDamage(Card defendingCard) {

            return Damage > defendingCard.Damage ? defendingCard : this;
        }

        protected Card CalcElementDamage(Card defendingCard) {

            var attackingCardDamage = Damage;
            var defendingCardDamage = defendingCard.Damage;
            attackingCardDamage = ElementType switch {

                ElementTypeNormal => defendingCard.ElementType switch {

                    ElementTypeNormal => Damage,
                    ElementTypeFire => Damage / 2,
                    ElementTypeWater => Damage * 2,
                    _ => attackingCardDamage
                },

                ElementTypeFire => defendingCard.ElementType switch {

                    ElementTypeNormal => Damage * 2,
                    ElementTypeFire => Damage,
                    ElementTypeWater => Damage / 2,
                    _ => attackingCardDamage
                },

                ElementTypeWater => defendingCard.ElementType switch {

                    ElementTypeNormal => Damage / 2,
                    ElementTypeFire => Damage * 2,
                    ElementTypeWater => Damage,
                    _ => attackingCardDamage
                },
                _ => attackingCardDamage
            };

            return attackingCardDamage > defendingCardDamage ? defendingCard: this;
        }


    }
}
