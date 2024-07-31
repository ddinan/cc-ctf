using MCGalaxy;
using MCGalaxy.Commands;
using MCGalaxy.Maths;
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

        public MapConfig config = new MapConfig();

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

            UpdateMapConfig(newLevel); // Reposition things such as flag and spawn positions.

            foreach (Player p in Players)
            {
                PlayerActions.ChangeMap(p, newLevel); // Send players in the lobby to the new map.
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
            { // Time ran out.
                EndGame();
                return;
            }

            if (BlueTeam.Captures == 5 || RedTeam.Captures == 5)
            { // Either team achieved max captures.
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

        public void UpdateMapConfig(Level lvl)
        {
            MapConfig config = new MapConfig();
            config.SetDefaults(lvl);
            config.Load(lvl.name.Replace($"lobby{LobbyId}_", ""));
            this.config = config;

            SpawnFlags(lvl);
        }

        private void SpawnFlags(Level lvl)
        {
            Position blueFlagPosition = new Position(16 + config.BlueFlagPosition.X * 32, 48 + config.BlueFlagPosition.Y * 32, 16 + config.BlueFlagPosition.Z * 32);
            SpawnFlag(lvl, blueFlagPosition, "blue_flag");

            Position redFlagPosition = new Position(16 + config.RedFlagPosition.X * 32, 48 + config.RedFlagPosition.Y * 32, 16 + config.RedFlagPosition.Z * 32);
            SpawnFlag(lvl, redFlagPosition, "red_flag");
        }

        public void SpawnFlag(Level lvl, Position position, string name)
        {
            PlayerBot bot = new PlayerBot(name, lvl);
            bot.SetInitialPos(position);
            bot.SetYawPitch(0, 0);
            bot.UpdateModel("sheep");
            PlayerBot.Add(bot);
        }

        public void RespawnPlayer(Player p)
        {
            Position pos;
            Vec3U16 spawn = new Vec3U16(p.level.spawnx, p.level.spawny, p.level.spawnz);

            int yaw = p.level.rotx;
            int pitch = p.level.roty;

            if (BlueTeam.Players.Contains(p))
            {
                spawn = config.BlueSpawnPosition;
                yaw = config.BlueSpawnYaw;
                pitch = config.BlueSpawnPitch;
            }

            else if (RedTeam.Players.Contains(p))
            {
                spawn = config.RedSpawnPosition;
                yaw = config.RedSpawnYaw;
                pitch = config.RedSpawnPitch;
            }

            pos.X = 16 + spawn.X * 32;
            pos.Y = 32 + spawn.Y * 32;
            pos.Z = 16 + spawn.Z * 32;

            if (!CommandParser.GetInt(Player.Console, yaw.ToString(), "Yaw angle", ref yaw, -360, 360)) return;
            if (!CommandParser.GetInt(Player.Console, pitch.ToString(), "Pitch angle", ref yaw, -360, 360)) return;
            yaw = Orientation.DegreesToPacked(yaw);
            pitch = Orientation.DegreesToPacked(pitch);

            Position oldPos = p.Pos;

            PlayerActions.RespawnAt(p, pos, (byte)yaw, (byte)pitch);

            // If the player is holding a flag, spawn it at their death location.
            // TODO: Why does the player turn into the flag when respawning?
            if (p.Extras.GetBoolean("CTF_HAS_FLAG"))
            {
                string team = BlueTeam.ContainsPlayer(p) ? "red" : "blue";
                SpawnFlag(p.level, oldPos, $"{team}_flag");
                p.Extras["CTF_HAS_FLAG"] = false;
            }
        }

        public void ClickOnFlag(Player p, PlayerBot flag)
        {
            Team team = BlueTeam.Players.Contains(p) ? RedTeam : BlueTeam;

            MessagePlayers($"&S{p.truename} picked up the {team.Name} flag!");
            p.Extras["CTF_HAS_FLAG"] = true;

            PlayerBot.Remove(flag);
        }

        public void CaptureFlag(Player p)
        {
            Team team = BlueTeam.Players.Contains(p) ? RedTeam : BlueTeam;

            MessagePlayers($"&S{p.truename} captured the {team.Name} flag!");
            p.Extras["CTF_HAS_FLAG"] = false;

            if (team == BlueTeam)
            {
                Position blueFlagPosition = new Position(16 + config.BlueFlagPosition.X * 32, 48 + config.BlueFlagPosition.Y * 32, 16 + config.BlueFlagPosition.Z * 32);
                SpawnFlag(p.level, blueFlagPosition, "blue_flag");

                BlueTeam.SetCaptures(BlueTeam.Captures + 1);
            }

            else if (team == RedTeam)
            {
                Position redFlagPosition = new Position(16 + config.RedFlagPosition.X * 32, 48 + config.RedFlagPosition.Y * 32, 16 + config.RedFlagPosition.Z * 32);
                SpawnFlag(p.level, redFlagPosition, "red_flag");

                RedTeam.SetCaptures(RedTeam.Captures + 1);
            }
        }
    }
}
