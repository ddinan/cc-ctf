using MCGalaxy;
using System;
using System.Collections.Generic;

namespace CTF
{
    internal class LobbyManager
    {
        public static List<Lobby> lobbies = new List<Lobby>();
        public static int nextLobbyId = 1; // Starting lobby ID.

        public static void CreateNewLobby(Player player)
        {
            Lobby newLobby = new Lobby(nextLobbyId);
            lobbies.Add(newLobby);

            string map = $"lobby{newLobby.LobbyId}_map1";
            if (!LevelActions.Copy(player, "map1", map)) return;
            LevelActions.Load(player, map, false);
            newLobby.Map = LevelInfo.FindExact(map); // Set the lobby's active map.
            newLobby.UpdateMapConfig(newLobby.Map);

            player.Message($"&SNew lobby created with ID &b{newLobby.LobbyId}&7.");

            // Immediately join the new lobby after creating it.
            if (player != Player.Console)
            {
                JoinLobby(player, newLobby.LobbyId);
            }
            else
            {
                Server.SetMainLevel(map); // Set the main level so new players automatically join the lobby.
            }

            newLobby.StartGame();

            nextLobbyId++;
        }

        public static void JoinLobby(Player player, int lobbyId, bool justConnected = false)
        {
            Lobby lobby = FindLobbyById(lobbyId);

            if (lobby == null)
            {
                player.Message($"&cLobby with ID {lobbyId} not found.");
                return;
            }

            if (lobby.Players.Contains(player))
            {
                player.Message("&cYou are already in this lobby.");
                return;
            }

            if (GetPlayerLobby(player) != null)
            {
                LeaveLobby(player);
            }

            if (lobby.Map != null && !justConnected)
            {
                PlayerActions.ChangeMap(player, lobby.Map);
            }

            lobby.AddPlayer(player);

            lobby.MessagePlayers($"&a+ &b{player.truename} &Sjoined the lobby &5({lobby.Players.Count} players)&S.");
        }

        private static Lobby FindLobbyById(int lobbyId)
        {
            foreach (var lobby in lobbies)
            {
                if (lobby.LobbyId == lobbyId)
                {
                    return lobby;
                }
            }
            return null; // Lobby with specified ID not found.
        }

        public static List<Lobby> GetLobbies()
        {
            return lobbies;
        }

        public static Lobby GetPlayerLobby(Player player)
        {
            foreach (var lobby in lobbies)
            {
                if (lobby.ContainsPlayer(player))
                {
                    return lobby;
                }
            }
            return null;
        }

        public static void LeaveLobby(Player player)
        {
            foreach (var lobby in lobbies)
            {
                if (lobby.ContainsPlayer(player))
                {
                    lobby.RemovePlayer(player);
                    lobby.MessagePlayers($"&c- &b{player.truename} &Sleft the lobby &5({lobby.Players.Count} players)&S.");
                    return;
                }
            }
        }

        public static void DeleteEmptyLobby(Lobby lobby)
        {
            if (lobby.Map != null)
            {
                if (!LevelActions.Delete(Player.Console, $"{lobby.Map.name.ToLower()}")) return;
            }

            lobbies.Remove(lobby);
            Console.WriteLine($"Lobby {lobby.LobbyId} has been deleted because it is empty.");
        }

    }
}
