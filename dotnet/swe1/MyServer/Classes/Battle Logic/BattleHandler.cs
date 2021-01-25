using System.Collections.Generic;
using System.Threading;
using MTCGserver.Classes.Battle_Logic;
using System.Collections.Concurrent;
using MTCGserver.Classes;

namespace MTCGserver.Classes {
     class BattleHandler {

        private BlockingCollection<User> usersWaitingForBattle = new BlockingCollection<User>();
        private List<User> usersWhoFinishedBattle = new List<User>();
        private string _gameLog;
        public string GameLog => _gameLog;

        private AutoResetEvent battleCanStart = new AutoResetEvent(false);

        static BattleHandler() {
            var battle = new Thread(() => Instance.Battle());
            battle.Start();
        }
        private BattleHandler() {}

        public static BattleHandler Instance { get; } = new BattleHandler();

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
