using MyServer.Classes.Battle_Stuff;
using MyServer.Classes.Cards.Monster;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyServer.Classes.DB_Stuff {
    class DeckDataBaseController {

        private const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=SWE;User ID=postgres;Password=swe;";

        public List<Card> ShowAllCards(String userId) {

            var fetchedCards = new List<Card>();
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            using var command = new NpgsqlCommand("select * from \"STACK\" join \"CARDS\" using (id) where user_id::text = :userId ", connection);

            command.Parameters.AddWithValue("userId", userId);
            var reader = command.ExecuteReader();

            while (reader.Read()) {

                var cardClassToUse = reader["name"].ToString() ?? string.Empty;
                var elementTypeToUse = int.Parse(reader["element_type"].ToString() ?? string.Empty);

                if (!cardClassToUse.Contains("Spell") && elementTypeToUse == 0) {
                    cardClassToUse = cardClassToUse.Insert(0, "Regular");
                }


                cardClassToUse = elementTypeToUse switch {

                    Card.ElementTypeFire => cardClassToUse.Remove(0, 4),
                    Card.ElementTypeNormal => cardClassToUse.Remove(0, 7),
                    Card.ElementTypeWater => cardClassToUse.Remove(0, 5),
                    _ => cardClassToUse
                };

                var currentCard = SwitchCardClassFromName(cardClassToUse, elementTypeToUse);


                currentCard.Name = reader["name"].ToString() ?? string.Empty;
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


            string packageStr = package.ToString();

            using var connection = new NpgsqlConnection(ConnectionString);
            using var command = new NpgsqlCommand("insert into \"CARDS\" (id, name, damage, card_type, element_type, pack_id) values (:id, :name, :damage, :card_type, :element_type, :pack_id) RETURNING card_id", connection);

            connection.Open();

            command.Parameters.AddWithValue("id", cardId);
            command.Parameters.AddWithValue("name", cardName);
            command.Parameters.AddWithValue("damage", cardDamage);
            command.Parameters.AddWithValue("card_type", cardType);
            command.Parameters.AddWithValue("element_type", cardElement);
            command.Parameters.AddWithValue("pack_id", packageStr);

            var cardIdObject = command.ExecuteScalar();
        }
        
        public bool acquirePackage(string userId) {

            using var connection = new NpgsqlConnection(ConnectionString);
            using var command = new NpgsqlCommand("select * from \"PACKAGES\" FETCH FIRST ROW ONLY", connection);

            connection.Open();
            var packageId = command.ExecuteScalar();
            
            if (packageId != null) {
                //get each card which is part of the pack
                List<string> Cards = new List<string>();

                for (int i = 0; i < 5; i++) {
                    using var command2 = new NpgsqlCommand("select id from \"CARDS\" where pack_id = '" + packageId + "' offset " + i + "", connection);
                    var cardId = command2.ExecuteScalar();
                    string tmp = cardId.ToString();
                    Cards.Add(tmp);
                }

                for (int i = 0; i < 5; i++) {

                    string cardId ="";
                    switch (i) {
                        case 0: cardId = Cards.ElementAt(0); break;
                        case 1: cardId = Cards.ElementAt(1); break;
                        case 2: cardId = Cards.ElementAt(2); break;
                        case 3: cardId = Cards.ElementAt(3); break;
                        case 4: cardId = Cards.ElementAt(4); break;
                        default: Console.WriteLine("Invalid CardId"); break;
                    };

                    using var command3 = new NpgsqlCommand("insert into \"STACK\" (user_id, card_id) values ('" + userId + "', '" + cardId + "')", connection);
                    command3.ExecuteNonQuery();
                }

                using var delCommand = new NpgsqlCommand("delete from \"PACKAGES\" where pack_id = '" + packageId + "'", connection);
                delCommand.ExecuteNonQuery();
                return true;

            } else {
                return false;
            }

        }

        public List<Card> GetDeck(String userId) {
            Console.WriteLine("in getdck");
            var fetchedCards = new List<Card>();
            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();

            using var command = new NpgsqlCommand("select * from \"DECKS\" join \"CARDS\" using (id) where user_id::text = :userId ", connection);

            command.Parameters.AddWithValue("userId", userId);
            var reader = command.ExecuteReader();

            while (reader.Read()) {

                var cardClassToUse = reader["name"].ToString() ?? string.Empty;
                var elementTypeToUse = int.Parse(reader["element_type"].ToString() ?? string.Empty);

                if (!cardClassToUse.Contains("Spell") && elementTypeToUse == 0) {
                    cardClassToUse = cardClassToUse.Insert(0, "Regular");
                }


                cardClassToUse = elementTypeToUse switch {

                    Card.ElementTypeFire => cardClassToUse.Remove(0, 4),
                    Card.ElementTypeNormal => cardClassToUse.Remove(0, 7),
                    Card.ElementTypeWater => cardClassToUse.Remove(0, 5),
                    _ => cardClassToUse
                };

                var currentCard = SwitchCardClassFromName(cardClassToUse, elementTypeToUse);


                currentCard.Name = reader["name"].ToString() ?? string.Empty;
                currentCard.CardType = int.Parse(reader["card_type"].ToString() ?? string.Empty);
                currentCard.ElementType = int.Parse(reader["element_type"].ToString() ?? string.Empty);
                currentCard.Damage = float.Parse(reader["damage"].ToString() ?? string.Empty);

                fetchedCards.Add(currentCard);
            }
            Console.WriteLine("fetched CaRDS in function");
            Console.WriteLine(fetchedCards);
            Console.WriteLine(fetchedCards[0]);
            return fetchedCards;
        }

        public bool checkUserDeck(string userId) {

            using var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            using var checkUserDeck = new NpgsqlCommand("select exists(select * from \"DECKS\" where user_id='" + userId + "')", connection);

            var userHasDeck = (bool)checkUserDeck.ExecuteScalar();

            if (userHasDeck == true) {
                return true;
            } else {
                return false;
            }
        }
        public void CardsToDeck(string[] cards, string userId) {

            using var connection = new NpgsqlConnection(ConnectionString);
            string deckId = Guid.NewGuid().ToString();
            connection.Open();

            for (int i = 0; i < 4; i++) {

                string cardId = "";

                switch (i) {
                case 0: cardId = cards[0]; break;
                case 1: cardId = cards[1]; break;
                case 2: cardId = cards[2]; break;
                case 3: cardId = cards[3]; break;
                default: Console.WriteLine("Invalid CardId"); break;
                };

                using var command = new NpgsqlCommand("insert into \"DECKS\" (deck_id, user_id, id) values ('" + deckId + "','" + userId + "', '" + cardId + "')", connection);
                command.ExecuteNonQuery();
            }

        }

    }
}