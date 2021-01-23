using MyServer.Classes.Battle_Stuff;
using System.Collections.Generic;

namespace MyServer.Classes.Data {
     class User {

        private List<Card> _deck = new List<Card>(5);

        public string Id { get; set; }

        public string Username { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public string Salt { get; set; }
        public string Token { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public int Coins { get; set; }
        public string BattleLog { get; set; }
        public int GamesPlayed { get; set; } = 0;

        private List<Card> Stack { get; set; }

        public List<Card> Deck {
            get => _deck;
            set => _deck = value;
        }
    }
}
