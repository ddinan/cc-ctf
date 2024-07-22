using MCGalaxy;
using MCGalaxy.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CTF
{
    public class Lobby
    {
        public int LobbyId { get; private set; }
        public List<Player> Players { get; private set; }
        public bool IsGameRunning { get; private set; }
        public int GameDurationMinutes { get; private set; }
        public DateTime GameEndTime { get; private set; }

        public SchedulerTask Task;

        public Lobby(int lobbyId, int gameDurationMinutes = 1)
        {
            LobbyId = lobbyId;
            Players = new List<Player>();
            IsGameRunning = false;
            GameDurationMinutes = gameDurationMinutes;
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

                // Check if lobby is now empty
                if (Players.Count == 0)
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

            if (Players.Count < 2)
            {
                MessagePlayers($"Cannot start game in Lobby {LobbyId}: Not enough players.");
                return;
            }

            MessagePlayers($"Game started in Lobby {LobbyId}!");
            IsGameRunning = true;
            GameEndTime = DateTime.Now.AddMinutes(GameDurationMinutes);

            Server.MainScheduler.QueueRepeat(GameLoop, null, TimeSpan.FromSeconds(1));
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

        public void EndGame()
        {
            MessagePlayers($"&SGame ended in Lobby {LobbyId}!");

            // Reset game state
            IsGameRunning = false;
            GameEndTime = DateTime.MinValue;

            // Perform end of game tasks like displaying scores, cleanup, etc.
            Server.MainScheduler.Cancel(Task);
        }

        public void MessagePlayers(string message)
        {
            foreach (Player p in Players)
            {
                p.Message(message);
            }
        }
    }
}
