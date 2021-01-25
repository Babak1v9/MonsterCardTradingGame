using System.Collections.Generic;
using System.Threading;
using MyServer.Classes.Battle_Stuff;
using System.Collections.Concurrent;
using MyServer.Classes.Data;
using System;

namespace MyServer.Classes {
     class GameHandler {

        //blocks thread until it can be taken out of the collection
        private BlockingCollection<User> usersWaitingForBattle = new BlockingCollection<User>();
        private List<User> usersWhoFinishedBattle = new List<User>();
        private string _gameLog;
        public string GameLog => _gameLog;

        private AutoResetEvent battleCanStart = new AutoResetEvent(false);

        static GameHandler() {
            var battle = new Thread(() => Instance.Battle());
            battle.Start();
        }
        private GameHandler() {}

        public static GameHandler Instance { get; } = new GameHandler();

        //todo: <returns> input player, including updated log </return>

        public User ConnectUserToBattle(User user) {
            usersWaitingForBattle.Add(user);

            while(!usersWhoFinishedBattle.Contains(user)) {

                Thread.Sleep(100);
            }

            usersWhoFinishedBattle.Remove(user);
            return user;
        }

        private void Battle() {
            var user1 = usersWaitingForBattle.Take();
            var user2 = usersWaitingForBattle.Take();
            var battle = new Battle(user1, user2);
            _gameLog = battle.StartBattle();

            usersWhoFinishedBattle.Add(user1);
            usersWhoFinishedBattle.Add(user2);
        }
    }
}
