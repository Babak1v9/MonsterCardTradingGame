﻿namespace MTCGserver.Classes.Battle_Logic {
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

        public virtual Card getLosingCard(Card defendingCard) {

            if (CardType == TypeMonster && defendingCard.CardType == TypeMonster) {
                return calcWithoutElementDmg(defendingCard);
            }

            return calcWithElementDmg(defendingCard);
        }

        protected Card calcWithoutElementDmg(Card defendingCard) {

            /*if (Damage == defendingCard.Damage) {
                return null;
            } */

            return Damage > defendingCard.Damage ? defendingCard : this;
        }

        protected Card calcWithElementDmg(Card defendingCard) {

            var defendingCardDamage = defendingCard.Damage;
            var attackingCardDamage = ElementType switch {

                ElementTypeNormal => defendingCard.ElementType switch {

                    ElementTypeNormal => Damage,
                    ElementTypeFire => Damage / 2,
                    ElementTypeWater => Damage * 2,
                    _ => Damage 
                },

                ElementTypeFire => defendingCard.ElementType switch {

                    ElementTypeNormal => Damage * 2,
                    ElementTypeFire => Damage,
                    ElementTypeWater => Damage / 2,
                    _ => Damage
                },

                ElementTypeWater => defendingCard.ElementType switch {

                    ElementTypeNormal => Damage / 2,
                    ElementTypeFire => Damage * 2,
                    ElementTypeWater => Damage,
                    _ => Damage
                },
                _ => Damage
            };

            /*if (attackingCardDamage == defendingCardDamage) {
                return null;
            }*/

            return attackingCardDamage > defendingCardDamage ? defendingCard: this;
        }


    }
}
