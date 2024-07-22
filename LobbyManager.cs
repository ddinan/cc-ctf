using MCGalaxy;
using System;
using System.Collections.Generic;

namespace CTF
{
    internal class LobbyManager
    {
        public static List<Lobby> lobbies = new List<Lobby>();
        public static int nextLobbyId = 1; // Starting lobby ID.

        public static void CreateNewLobby(Player p)
        {
            Lobby newLobby = new Lobby(nextLobbyId);
            lobbies.Add(newLobby);

            string map = $"lobby{newLobby.LobbyId}_map1";
            if (!LevelActions.Copy(p, "map1", map)) return;
            LevelActions.Load(p, map, false);
            newLobby.Map = LevelInfo.FindExact(map); // Set the lobby's active map.

            p.Message($"&SNew lobby created with ID &b{newLobby.LobbyId}&7.");

            // Immediately join the new lobby after creating it.
            JoinLobby(p, newLobby.LobbyId);
            newLobby.StartGame();

            nextLobbyId++;
        }

        public static void JoinLobby(Player p, int lobbyId)
        {
            Lobby lobby = FindLobbyById(lobbyId);

            if (lobby == null)
            {
                p.Message($"&cLobby with ID {lobbyId} not found.");
                return;
            }

            if (lobby.Players.Contains(p))
            {
                p.Message("&cYou are already in this lobby.");
                return;
            }

            if (GetPlayerLobby(p) != null)
            {
                LeaveLobby(p);
            }

            if (lobby.Map != null)
            {
                PlayerActions.ChangeMap(p, lobby.Map);
            }

            lobby.AddPlayer(p);

            lobby.MessagePlayers($"&a+ &b{p.truename} &Sjoined the lobby &5({lobby.Players.Count} players)&S.");
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

        public static void LeaveLobby(Player p)
        {
            foreach (var lobby in lobbies)
            {
                if (lobby.ContainsPlayer(p))
                {
                    lobby.RemovePlayer(p);
                    lobby.MessagePlayers($"&c- &b{p.truename} &Sleft the lobby &5({lobby.Players.Count} players)&S.");
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
