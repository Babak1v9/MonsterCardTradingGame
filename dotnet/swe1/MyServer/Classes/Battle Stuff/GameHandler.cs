using System.Collections.Generic;
using System.Threading;
using MyServer.Classes.Battle_Stuff;
using System.Collections.Concurrent;
using MyServer.Classes.Data;

namespace MyServer.Classes {
     class GameHandler {

        //blocks thread until it can be taken out of the collection
        private BlockingCollection<User> _playersWaitingForBattle = new BlockingCollection<User>();
        private List<User> _PlayersWhoFinishedBattle = new List<User>();

        private AutoResetEvent _battleCanStart = new AutoResetEvent(false);

        static GameHandler() {
            var battleWorker = new Thread(() => Instance.Battle());
            battleWorker.Start();
        }
        //singleton class https://csharpindepth.com/articles/singleton
        private GameHandler() {

        }

        public static GameHandler Instance { get; } = new GameHandler();

        public static bool StopRequested { get; set; } = false;

        //todo: <returns> input player, including updated log </return>

        public User ConnectPlayerToBattle(User user) {

            _playersWaitingForBattle.Add(user);

            while(!_PlayersWhoFinishedBattle.Contains(user)) {

                Thread.Sleep(100);
            }

            _PlayersWhoFinishedBattle.Remove(user);
            return user;
        }

        private void Battle() {

            while (!StopRequested) {

                var player1 = _playersWaitingForBattle.Take();
                var player2 = _playersWaitingForBattle.Take();
                var battle = new Battle(player1, player2);
                battle.StartBattle();

                _PlayersWhoFinishedBattle.Add(player1);
                _PlayersWhoFinishedBattle.Add(player2);
                Thread.Sleep(100);
            }
        }



    }
}
