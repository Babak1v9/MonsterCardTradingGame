using System.Collections.Generic;
using System.Text;
using Npgsql;
using System.Security.Cryptography;
using System.Linq;
using MTCGserver.Classes;

namespace MTCGserver.Classes.DB {
    class UserDatabaseController {

        private const string connectionString = "Server=127.0.0.1;Port=5432;Database=SWE;User ID=postgres;Password=swe;";
        private const string pepper = "NotHackable69420";

        public List<User> GetUsers() {

            var fetchedUser = new List<User>();

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
 
            using var command = new NpgsqlCommand("select user_id, username, name, token, bio, image, coins, games_played from \"USER\"", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read()) {

                var currentUser = new User {
                    Id = reader["user_id"].ToString(),
                    Username = reader["username"].ToString(),
                    Name = reader["name"].ToString(),
                    Token = reader["token"].ToString(),
                    Bio = reader["bio"].ToString(),
                    Image = reader["image"].ToString(),
                    Coins = int.Parse(reader["coins"].ToString() ?? string.Empty),
                    GamesPlayed = int.Parse(reader["games_played"].ToString() ?? string.Empty)
                };

                fetchedUser.Add(currentUser);
            }

            return fetchedUser;
        }

        public bool AuthenticateUser(IDictionary<string, string> headers) {

            if (headers.ContainsKey("authorization") == false) {
                return false;
            }

            string token = headers["authorization"];
            var tokenWithoutBasic = token.Remove(0, 6);
            var tokenExists = VerifyUserToken(tokenWithoutBasic);

            if (tokenExists != true) {
                return false;
            }
            return true;
        }

        private string GenerateSalt() {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var saltString = System.Text.Encoding.UTF8.GetString(salt);

            return saltString;
        }

        public string CalcSHA256(string password, string salt) {

            SHA256 sha256 = SHA256.Create();
            byte[] hashValue;
            UTF8Encoding objUtf8 = new UTF8Encoding();

            var passwordBuilder = new StringBuilder();
            passwordBuilder.Append(password);
            passwordBuilder.Append(pepper);
            passwordBuilder.Append(salt);
            var passwordForHashing = passwordBuilder.ToString();
            
            hashValue = sha256.ComputeHash(objUtf8.GetBytes(passwordForHashing));

            var hashedPassword = string.Concat(hashValue.Select(b => b.ToString("x2")));

            return hashedPassword;
        }

        public User GetByToken(string Token) { 

            var fetchedUser = new User();
            var tokenWithoutBasic = Token.Remove(0, 6);

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var command = new NpgsqlCommand("select user_id, username, name, token, bio, image, coins, games_played, elo, wins from \"USER\" where token=:userToken", connection);

            //adds the token as parameter, removing the basic poart form the strong
            command.Parameters.AddWithValue("userToken", tokenWithoutBasic);
            var reader = command.ExecuteReader();

            if (!reader.HasRows) {
                return null; //no data found
            }

            reader.Read();

            fetchedUser.Id = reader["user_id"].ToString();
            fetchedUser.Username = reader["username"].ToString();
            fetchedUser.Name = reader["name"].ToString();
            fetchedUser.Token = reader["token"].ToString();
            fetchedUser.Bio = reader["bio"].ToString();
            fetchedUser.Image = reader["image"].ToString();
            fetchedUser.Coins = int.Parse(reader["coins"].ToString() ?? string.Empty);
            fetchedUser.GamesPlayed = int.Parse(reader["games_played"].ToString() ?? string.Empty);

            return fetchedUser;
        }

        public User GetByUserName(string userName, bool deckFlag = false) {

            var returnedUser = new User();

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("select user_id, username, name, password, salt, token, bio, image, coins, games_played from \"USER\" where username=:userName", connection);

            command.Parameters.AddWithValue("userName", userName);
            var reader = command.ExecuteReader();

            if (!reader.HasRows) {
                return null;
            }

            reader.Read();

            returnedUser.Id = reader["user_id"].ToString();
            returnedUser.Username = reader["username"].ToString();
            returnedUser.Name = reader["name"].ToString();
            returnedUser.Password = reader["password"].ToString();
            returnedUser.Salt = reader["salt"].ToString();
            returnedUser.Token = reader["token"].ToString();
            returnedUser.Bio = reader["bio"].ToString();
            returnedUser.Image = reader["image"].ToString();
            returnedUser.Coins = int.Parse(reader["coins"].ToString() ?? string.Empty);
            returnedUser.GamesPlayed = int.Parse(reader["games_played"].ToString() ?? string.Empty);

            return returnedUser;
        }

        public void Insert(string username, string password) {

            using var connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand("insert into \"USER\" (username, password, token, salt) values (:userName, :password, :token, :salt)", connection);
            connection.Open();

            //hash and salt pw
            string salt = GenerateSalt();
            var hashedPassword = CalcSHA256(password, salt);
            
            //token
            var tokenBuilder = new StringBuilder();
            tokenBuilder.Append(username);
            tokenBuilder.Append("-mtcgToken");
            var token = tokenBuilder.ToString();

            command.Parameters.AddWithValue("userName", username);
            command.Parameters.AddWithValue("password", hashedPassword);
            command.Parameters.AddWithValue("token", token);
            command.Parameters.AddWithValue("salt", salt);

            command.ExecuteNonQuery();
        }

        public void Update(User user, string newUsername) {

            using var connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand("update \"USER\" set name = :name, bio = :bio, image = :image where username=:userName", connection);
            connection.Open();

            command.Parameters.AddWithValue("name", user.Name);
            command.Parameters.AddWithValue("bio", user.Bio);
            command.Parameters.AddWithValue("image", user.Image);
            command.Parameters.AddWithValue("userName", newUsername);

            command.ExecuteNonQuery();
        }
        
        public bool VerifyUserToken(string token) {

            using var connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand("select 1 from \"USER\" where token = '" + token + "' limit 1", connection);
            connection.Open();

            var doesExist = command.ExecuteScalar();
            var myVerifier = doesExist != null ? true : false;

            return myVerifier;
        }

        public int GetCoins(string token) {

            using var connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand("select coins from \"USER\" where token =:token", connection);
            connection.Open();

            command.Parameters.AddWithValue("token", token);
            var tmpObject = command.ExecuteScalar();
            int userCoins = (int)tmpObject;

            return userCoins;
        }

        public void PackageAcquired(string token) {

            using var connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand("update \"USER\" set coins = coins -5 where token =:token", connection);
            connection.Open();

            command.Parameters.AddWithValue("token", token);
            command.ExecuteNonQuery();
           
        }

        public List<int> ShowUserStats(string token) {

            using var connection = new NpgsqlConnection(connectionString);
            using var getElo = new NpgsqlCommand("select elo from \"USER\" where token =:token", connection);
            connection.Open();

            getElo.Parameters.AddWithValue("token", token);
            int userElo = (int)getElo.ExecuteScalar();

            using var getGamesPlayed = new NpgsqlCommand("select games_played from \"USER\" where token =:token", connection);
            getGamesPlayed.Parameters.AddWithValue("token", token);
            int userGamesPlayed = (int)getGamesPlayed.ExecuteScalar();

            using var getWins = new NpgsqlCommand("select wins from \"USER\" where token =:token", connection);
            getWins.Parameters.AddWithValue("token", token);
            int userWins = (int)getWins.ExecuteScalar();

            List<int> userStats = new List<int>();
            userStats.Add(userElo);
            userStats.Add(userGamesPlayed);
            userStats.Add(userWins);

            return userStats;
        }

        public void UpdateStats (User user, bool winOrLose) {
            using var connection = new NpgsqlConnection(connectionString);
            using var addGamePlayed = new NpgsqlCommand("update \"USER\" set games_played = games_played +1 where username =:username", connection);
            connection.Open();
            addGamePlayed.Parameters.AddWithValue("username", user.Username);
            addGamePlayed.ExecuteNonQuery();

            if (winOrLose) {
                using var changeElo = new NpgsqlCommand("update \"USER\" set elo = elo +3, wins = wins +1 where username =:username", connection);
                changeElo.Parameters.AddWithValue("username", user.Username);
                changeElo.ExecuteNonQuery();
            } else {
                using var changeElo = new NpgsqlCommand("update \"USER\" set elo = elo -5 where username =:username", connection);
                changeElo.Parameters.AddWithValue("username", user.Username);
                changeElo.ExecuteNonQuery();
            }
        }
    }
}
