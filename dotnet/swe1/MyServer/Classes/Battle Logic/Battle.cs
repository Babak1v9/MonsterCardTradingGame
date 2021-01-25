using MTCGserver.Classes;
using MTCGserver.Classes.DB;
using System;

namespace MTCGserver.Classes.Battle_Logic {
     class Battle {

        private User user1;
        private User user2;
        private Random random = new Random();
        private DeckDataBaseController deckDBController = new DeckDataBaseController();
        private UserDatabaseController userDBController = new UserDatabaseController();
        public string Gamelog;

        public Battle(User _user1, User _user2) {

            var (first, second) = SelectStartingUser(_user1, _user2);
            user1 = first;
            user2 = second;
        }

        public string StartBattle() {
            User winnerOfBattle = new User();
            int counter = 1;
            user1.Deck = deckDBController.GetDeck(user1.Id);
            user2.Deck = deckDBController.GetDeck(user2.Id);
            

            while (counter < 101) {
                if (user1.Deck.Count == 0) {
                    winnerOfBattle = user2;
                    break;
                }

                if (user2.Deck.Count == 0) {
                    winnerOfBattle = user1;
                    break;
                }

                var user1Card = user1.Deck[random.Next(0, user1.Deck.Count)];
                var user2Card = user2.Deck[random.Next(0, user2.Deck.Count)];

                var winnerofRound = WinnerOfRound(user1Card, user2Card);

                if (winnerofRound == null) {
                    Gamelog += Environment.NewLine + "Round number " + counter + " was a Tie. User 1 Card: " + user1Card.Name + " vs. User 2 Card: " + user2Card.Name + Environment.NewLine;
                }

                if (winnerofRound == user1) {
                    Gamelog += Environment.NewLine + "User " + user1.Username+ " won Round number " + counter + ". Card played: " + user1Card.Name + " vs. " + user2Card.Name + ". New Deck Count: " + user1.Deck.Count + Environment.NewLine;

                } else if (winnerofRound == user2) {
                    Gamelog += Environment.NewLine + "User " + user2.Username + " won Round number " + counter + ". Card played: " + user2Card.Name + " vs. " + user1Card.Name + ". Deck Count: " + user2.Deck.Count + Environment.NewLine;
                    }
                counter++;
                
            }

            if (winnerOfBattle == user1) {
                userDBController.UpdateStats(user1, true);
                userDBController.UpdateStats(user2, false);
            } else {
                userDBController.UpdateStats(user1, false);
                userDBController.UpdateStats(user2, true);
            }

            Gamelog += winnerOfBattle.Username + " won the battle!\n";
            return Gamelog;
        }

        private User WinnerOfRound(Card user1Card, Card user2Card) {

            User winner = new User();

            var losingCard = user1Card.getLosingCard(user2Card);

            if (losingCard == null) {
                return null;
            }

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
