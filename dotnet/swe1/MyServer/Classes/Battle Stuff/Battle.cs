using MyServer.Classes.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyServer.Classes.Battle_Stuff {
     class Battle {

        private User _player1;
        private User _player2;
        private string _log;
        private Random random = new Random();

        public Battle(User player1, User player2) {
            var (item1, item2) = SelectStartingPlayer(player1, player2);
            _player1 = item1;
            _player2 = item2;
        }

        public void StartBattle() {
            var roundCount = 0;
            while (roundCount < 100) {
                if (_player1.Deck.Count == 0) {
                    Console.WriteLine("Player 2 won. P1 has no more cards.");
                    break;
                }

                if (_player2.Deck.Count == 0) {
                    Console.WriteLine("Player 1 won. P2 has no more cards.");
                    break;
                }

                var player1Card = SelectRandomCard(_player1);
                var player2Card = SelectRandomCard(_player2);

                var winningPlayer = DetermineWinner(player1Card, player2Card, roundCount);

                if (winningPlayer == _player1) {
                    _log += $"{_player1.Username} won this round. He played {player1Card.Name}. he now has {_player1.Deck.Count} cards.\n";
                    
                } else if (winningPlayer == _player2) {
                        _log += $"{_player2.Username} won this round. He played {player2Card.Name}. he now has {_player2.Deck.Count} cards.\n";
                    }
                roundCount++;
                
            }

            _log += _player1.Deck.Count > _player2.Deck.Count ? _player1.Username : _player2.Username;
            _log += " won the battle!\n";
        }

        private User DetermineWinner(Card player1Card, Card player2Card, int roundCount) {

            User userToReturn = new User();

            //player 1 always starts? maybe change
            //first if = both cards type monster
            //if else useless -> calcdmgagainst method checks cardtype
            if (player1Card.CardType == Card.TypeMonster && player2Card.CardType == Card.TypeMonster) {

                var losingCard = player1Card.CalculateDamageAgainst(player2Card);

                if (losingCard == player2Card) {

                    _player1.Deck.Add(losingCard);
                    _player2.Deck.Remove(losingCard);
                    userToReturn = _player1;
                } else {

                    _player2.Deck.Add(losingCard);
                    _player1.Deck.Remove(losingCard);
                    userToReturn = _player2;
                }
            } else {

                var losingCard = player1Card.CalculateDamageAgainst(player2Card);

                if (losingCard == player2Card) {

                    _player1.Deck.Add(losingCard);
                    _player2.Deck.Remove(losingCard);
                    userToReturn = _player1;
                } else {

                    _player2.Deck.Add(losingCard);
                    _player1.Deck.Remove(losingCard);
                    userToReturn = _player2;
                }
            }


            User temp = _player1;
            _player1 = _player2;
            _player2 = temp;

            return userToReturn;
        }

        private Card SelectRandomCard(User user) {

            return user.Deck[random.Next(0, user.Deck.Count)];
        }

        private (User, User) SelectStartingPlayer(User player1, User player2) {

            var startingPlayer = random.Next(1, 3);
            if (startingPlayer == 1) {
                return (player1, player2);
            } else {
                return (player2, player1);
            }
        }

        public User Player1 => _player1;

        public User Player2 => _player2;

        public string Log => _log;


    }
}
