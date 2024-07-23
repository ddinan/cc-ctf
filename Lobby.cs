using MCGalaxy;
using MCGalaxy.Tasks;
using System;
using System.Collections.Generic;

namespace CTF
{
    public class Lobby
    {
        public int LobbyId { get; private set; }
        public List<Player> Players { get; private set; }
        public bool IsGameRunning { get; private set; }
        public int GameDurationMinutes { get; private set; }
        public DateTime GameEndTime { get; private set; }

        public Team BlueTeam { get; private set; }
        public Team RedTeam { get; private set; }

        public SchedulerTask Task;
        public Level Map = null;

        private MapVoting mapVoting;

        public Lobby(int lobbyId, int gameDurationMinutes = 1)
        {
            LobbyId = lobbyId;
            LobbyId = lobbyId;
            Players = new List<Player>();
            IsGameRunning = false;
            GameDurationMinutes = gameDurationMinutes;

            BlueTeam = new Team("Blue");
            RedTeam = new Team("Red");

            List<string> availableMaps = new List<string> { "map1", "map2", "map3", "map4" };
            mapVoting = new MapVoting(this, availableMaps);
        }

        public void AddPlayer(Player p)
        {
            Players.Add(p);
        }

        public void RemovePlayer(Player p)
        {
            if (Players.Contains(p))
            {
                Players.Remove(p);

                // Check if lobby is now empty.
                if (Players.Count == 0 && LobbyId != 1)
                {
                    LobbyManager.DeleteEmptyLobby(this);
                }
            }
        }

        public bool ContainsPlayer(Player player)
        {
            return Players.Contains(player);
        }

        public void StartGame()
        {
            if (IsGameRunning) return;

            ScheduleInitialGameStartCheck();
        }

        public void StartNextMap(string lvl)
        {
            string map = $"lobby{LobbyId}_" + lvl;
            if (!LevelActions.Copy(Player.Console, lvl, map)) return;
            LevelActions.Load(Player.Console, map, false);
            Level newLevel = LevelInfo.FindExact(map);

            // Send players in the lobby to the new map.

            foreach (Player p in Players)
            {
                PlayerActions.ChangeMap(p, newLevel);
            }

            if (Map != null)
            {
                if (LobbyId == 1)
                {
                    Server.SetMainLevel(map);
                }

                if (!LevelActions.Delete(Player.Console, $"{Map.name.ToLower()}")) return; // Delete the old map.
            }

            Map = newLevel; // Set the lobby's active map.

            ScheduleInitialGameStartCheck();
        }

        private bool initialCheckScheduled = false;

        private void ScheduleInitialGameStartCheck()
        {
            Server.MainScheduler.QueueRepeat(CheckTeamsAndStartGame, null, TimeSpan.FromSeconds(10));
            initialCheckScheduled = true;
        }

        private void CheckTeamsAndStartGame(SchedulerTask task)
        {
            // Check if there is at least one player on each team.
            if (BlueTeam.Players.Count > 0 && RedTeam.Players.Count > 0)
            {
                // Ensure the initial check task is canceled once conditions are met.
                if (initialCheckScheduled)
                {
                    Server.MainScheduler.Cancel(task);
                    initialCheckScheduled = false;
                }

                // Start the game
                MessagePlayers($"&SGame started in Lobby {LobbyId}!");
                IsGameRunning = true;
                GameEndTime = DateTime.Now.AddMinutes(GameDurationMinutes);

                Server.MainScheduler.QueueRepeat(GameLoop, null, TimeSpan.FromSeconds(1));
            }

            else
            {
                MessagePlayers($"&SThe game will commence once each team has at least one player.");
            }
        }

        private void GameLoop(SchedulerTask task)
        {
            Task = task;

            TimeSpan timeLeft = GameEndTime - DateTime.Now;

            if (timeLeft.TotalSeconds <= 0)
            {
                EndGame();
                return;
            }

            MessagePlayers($"&SLobby {LobbyId} - Time left: {timeLeft.Minutes}:{timeLeft.Seconds}");

            // Update game state, check conditions, etc.
            // Example: Check if any team has won, update player states, etc.
        }

        public void EndGame(bool shutdown = false)
        {
            MessagePlayers($"&SGame ended in Lobby {LobbyId}!");

            // Reset game state
            IsGameRunning = false;
            GameEndTime = DateTime.MinValue;

            if (BlueTeam.Players.Count > 0)
            {
                BlueTeam.Players.Clear();
            }


            if (RedTeam.Players.Count > 0)
            {
                RedTeam.Players.Clear();
            }

            // Perform end of game tasks like displaying scores, cleanup, etc.
            Server.MainScheduler.Cancel(Task);

            if (shutdown)
            {
                LobbyManager.DeleteEmptyLobby(this);
                return;
            }

            mapVoting.StartMapVoting();
        }

        public void MessagePlayers(string message)
        {
            if (Map == null) return;

            Map.Message(message);
        }
    }
}
