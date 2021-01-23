using MTCGserver.Classes.Cards;
using MTCGserver.Classes.Cards.Monster;
using MTCGserver.Classes.Cards.Spells;
using Npgsql;
using System;
using System.Collections.Generic;

namespace MTCGserver.Classes.DB {
    class DeckDataBaseController {

        private UserDatabaseController _userDatabaseController = new UserDatabaseController();

        private const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=SWE;User ID=postgres;Password=swe;";

        public List<Card> GetByUserId(String userId) {

            var fetchedCards = new List<Card>();
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            using var command = new NpgsqlCommand("select * from \"DECK\" join \"DECK_CARD\" using (deck_id) join \"CARD\" using (card_id) where user_id::text = :userId ", connection);

            command.Parameters.AddWithValue("userId", userId);
            var reader = command.ExecuteReader();

            while (reader.Read()) {

                var cardClassToUse = reader["name"].ToString() ?? string.Empty;
                var elementTypeToUse = int.Parse(reader["element_type"].ToString() ?? string.Empty);

                cardClassToUse = elementTypeToUse switch {

                    Card.ElementTypeFire => cardClassToUse.Remove(0, 4),
                    Card.ElementTypeNormal => cardClassToUse.Remove(0, 6),
                    Card.ElementTypeWater => cardClassToUse.Remove(0, 5),
                    _ => cardClassToUse
                };

                var currentCard = SwitchCardClassFromName(cardClassToUse, elementTypeToUse);

                currentCard.Name = reader["name"].ToString();
                currentCard.CardType = int.Parse(reader["card_type"].ToString() ?? string.Empty);
                currentCard.ElementType = int.Parse(reader["element_type"].ToString() ?? string.Empty);
                currentCard.Damage = float.Parse(reader["damage"].ToString() ?? string.Empty);

                fetchedCards.Add(currentCard);
            }

            return fetchedCards;

        }

        private Card SwitchCardClassFromName(string name, int elementType) {

            Card returnCard = name switch {

                "Dragon" => new DragonCard(),
                "Elf" => new FireElveCard(),
                "Goblin" => new GoblinCard(),
                "Knight" => new KnightCard(),
                "Kraken" => new KrakenCard(),
                "Ork" => new OrkCard(),
                "Wizard" => new WizardCard(),
                "Spell" => elementType switch {

                    Card.ElementTypeFire => new FireSpellCard(),
                    Card.ElementTypeWater => new WaterSpellCard(),
                    Card.ElementTypeNormal => new NormalSpellCard(),
                    _ => null
                },
                _ => null
            };

            return returnCard;
        }

        public object createPackage() {

            using var connection = new NpgsqlConnection(ConnectionString);
            using var command = new NpgsqlCommand("insert into \"PACKAGES\" (price) values (:price) RETURNING pack_id", connection);

            connection.Open();

            var price = 5;
            command.Parameters.AddWithValue("price", price);

            var packageId = command.ExecuteScalar();
            return packageId;


        }

        public void createCard(string cardId, string cardName, string cardDamage, string cardType, string cardElement, object package) {

            var newCardId = cardId;
            var newCardName = cardName;
            var newCardDamage = cardDamage;
            var newCardType = cardType;
            var newCardElemetn = cardElement;
            var packageId = package;


            Console.WriteLine(newCardId);
            Console.WriteLine(cardName);
            Console.WriteLine(cardDamage);
            Console.WriteLine(packageId);


            string packageStr = packageId.ToString();
            Console.WriteLine(package);

            using var connection = new NpgsqlConnection(ConnectionString);
            using var command = new NpgsqlCommand("insert into \"CARDS\" (id, name, damage, card_type, element_type, pack_id) values (:id, :name, :damage, :card_type, :element_type, :pack_id) RETURNING card_id", connection);

            connection.Open();

            command.Parameters.AddWithValue("id", newCardId);
            command.Parameters.AddWithValue("name", newCardName);
            command.Parameters.AddWithValue("damage", newCardDamage);
            command.Parameters.AddWithValue("card_type", cardType);
            command.Parameters.AddWithValue("element_type", cardElement);
            command.Parameters.AddWithValue("pack_id", packageId);


            var cardIdObject = command.ExecuteScalar();

        }

        /*public void acquirePackage(string tok) {

            //string card_id;
            //string deck_id;
            //string user_id;
            string token = tok;

            var user = _userDatabaseController.GetByToken(token);
            //user_id = user.Id;
            //Console.WriteLine("user_id");

            using var connection = new NpgsqlConnection(ConnectionString);
            using var command = new NpgsqlCommand("select * from \"PACKAGES\" FETCH FIRST ROW ONLY", connection);

            connection.Open();
            var packageId = command.ExecuteScalar();

            string tempid = "93460d62 - b4a7 - 4035 - a342 - 9b4776ea5380";
            using var command2 = new NpgsqlCommand("select * from \"CARDS\" where pack_id = '" + tempid + "'", connection);

            // card id von CARDS > DECK_CARD, von dort hole ich DECK_ID -> DECK, IN DECK USERID hinzufügen


            //using var delCommand = new NpgsqlCommand("delete from \"PACKAGES\" where pack_id = '" + packageId + "'", connection);
            //delCommand.ExecuteNonQuery();


        }*/

    }
}