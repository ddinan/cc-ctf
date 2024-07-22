﻿using MCGalaxy;
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

        public List<Player> BluePlayers = new List<Player>();
        public List<Player> RedPlayers = new List<Player>();

        public SchedulerTask Task;
        public Level Map = null;

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

                // Check if lobby is now empty.
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

            ScheduleInitialGameStartCheck();

            MessagePlayers($"Game starting in Lobby {LobbyId} will commence once each team has at least one player.");
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
            if (BluePlayers.Count > 0 && RedPlayers.Count > 0)
            {
                // Ensure the initial check task is canceled once conditions are met.
                if (initialCheckScheduled)
                {
                    Server.MainScheduler.Cancel(task);
                    initialCheckScheduled = false;
                }

                // Start the game
                MessagePlayers($"Game started in Lobby {LobbyId}!");
                IsGameRunning = true;
                GameEndTime = DateTime.Now.AddMinutes(GameDurationMinutes);

                Server.MainScheduler.QueueRepeat(GameLoop, null, TimeSpan.FromSeconds(1));
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

            BluePlayers.Clear();
            RedPlayers.Clear();

            // Perform end of game tasks like displaying scores, cleanup, etc.
            Server.MainScheduler.Cancel(Task);

            if (shutdown)
            {
                LobbyManager.DeleteEmptyLobby(this);
            }
        }

        public void MessagePlayers(string message)
        {
            Map.Message(message);
        }
    }
}
