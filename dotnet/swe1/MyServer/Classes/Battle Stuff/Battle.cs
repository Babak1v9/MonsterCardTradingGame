using MyServer.Classes.Data;
using MyServer.Classes.DB_Stuff;
using System;
using System.Collections.Generic;

namespace MyServer.Classes.Battle_Stuff {
     class Battle {

        private User user1;
        private User user2;
        private Random random = new Random();
        private DeckDataBaseController deckDBController = new DeckDataBaseController();
        public string Gamelog;

        public Battle(User _user1, User _user2) {

            var (first, second) = SelectStartingUser(_user1, _user2);
            user1 = first;
            user2 = second;
        }

        public string StartBattle() {
            Console.WriteLine("in StartBattle");
            int counter = 0;
            user1.Deck = deckDBController.GetDeck(user1.Id);
            user2.Deck = deckDBController.GetDeck(user2.Id);
            Console.WriteLine(counter);
            while (counter < 100) {
                if (user1.Deck.Count == 0) {
                    Console.WriteLine("User"+ user2.Username + " won.");
                    break;
                }

                if (user2.Deck.Count == 0) {
                    Console.WriteLine("User" + user1.Username + " won.");
                    break;
                }

                var user1Card = SelectRandomCard(user1.Deck);
                var user2Card = SelectRandomCard(user2.Deck);
                Console.WriteLine("user1Card");
                Console.WriteLine(user1Card);
                Console.WriteLine("user2Card");
                Console.WriteLine(user2Card);

                var winner = WinnerOfRound(user1Card, user2Card);
                Console.WriteLine("winner");
                Console.WriteLine(winner.Username);

                if (winner == user1) {
                    Gamelog += Environment.NewLine + user1.Username+" won Round number" + counter + ". Card played: " + user1Card.Name + ". Deck Count: " + user1.Deck.Count + Environment.NewLine;

                } else if (winner == user2) {
                    Gamelog += Environment.NewLine + user2.Username + " won Round number" + counter + ". Card played: " + user2Card.Name + ". Deck Count: "+ user2.Deck.Count + Environment.NewLine;
                    }
                counter++;
                
            }

            Gamelog += user1.Deck.Count > user2.Deck.Count ? user1.Username : user2.Username;
            Gamelog += " won the battle!\n";
            return Gamelog;
        }

        private User WinnerOfRound(Card user1Card, Card user2Card) {
            Console.WriteLine("in WinnerOfRound");
            User winner = new User();

            var losingCard = user1Card.getLosingCard(user2Card);

            if (losingCard == user2Card) {

                user1.Deck.Add(losingCard);
                user2.Deck.Remove(losingCard);
                winner = user1;
            } else {

                user2.Deck.Add(losingCard);
                user1.Deck.Remove(losingCard);
                winner = user2;
            }

            User changeOrder = user1;
            user1 = user2;
            user2 = changeOrder;

            return winner;
        }

        private Card SelectRandomCard(List<Card> userDeck) {
            Console.WriteLine("in SelectRandomCard");

            return userDeck[random.Next(0, userDeck.Count)];
        }

        private (User, User) SelectStartingUser(User user1, User user2) {
            int startingUser = random.Next(0,1);
            Console.WriteLine(startingUser);
            if (startingUser == 0) {
                return (user1, user2);
            } else {
                return (user2, user1);
            }
        }
    }
}
