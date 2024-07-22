using MCGalaxy;
using MCGalaxy.DB;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace CTF
{
    internal class LobbyManager
    {
        public static List<Lobby> lobbies = new List<Lobby>();
        public static int nextLobbyId = 1; // Starting lobby ID

        public static void CreateNewLobby(Player p)
        {
            Lobby newLobby = new Lobby(nextLobbyId);
            lobbies.Add(newLobby);

            p.Message($"&SNew lobby created with ID &b{nextLobbyId}&7.");

            // Immediately join the new lobby after creating it
            JoinLobby(p, nextLobbyId);

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

            if (IsPlayerInLobby(p))
            {
                LeaveLobby(p);
            }

            lobby.AddPlayer(p);

            lobby.MessagePlayers($"&a+ &b{p.truename} &Sjoined the lobby &5({lobby.Players.Count} players)&S.");

            // If enough players have joined, start the game
            if (lobby.Players.Count >= 2)
            {
                lobby.StartGame();
            }

            else
            {
                Console.WriteLine("Waiting for more players to join...");
            }
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
            return null; // Lobby with specified ID not found
        }

        public static List<Lobby> GetLobbies()
        {
            return lobbies;
        }

        private static bool IsPlayerInLobby(Player player)
        {
            foreach (var lobby in lobbies)
            {
                if (lobby.ContainsPlayer(player))
                {
                    return true;
                }
            }
            return false;
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
            lobbies.Remove(lobby);
            Console.WriteLine($"Lobby {lobby.LobbyId} has been deleted because it is empty.");
        }

    }
}
