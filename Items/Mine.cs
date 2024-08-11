using MCGalaxy;
using System;
using System.Collections.Generic;

namespace CTF.Items
{
    public class Mine
    {
        public Position Location { get; private set; }
        public Player Owner { get; private set; }
        public bool IsActive = false;

        public Mine(Player player, Position location)
        {
            Location = location;
            Owner = player;
        }
    }

    public class Mines
    {
        public static void HandlePlayerMove(Player player, Position next, byte yaw, byte pitch, ref bool cancel)
        {
            int x = player.Pos.BlockX;
            int y = player.Pos.BlockY;
            int z = player.Pos.BlockZ;

            Lobby lobby = LobbyManager.GetPlayerLobby(player);
            if (lobby == null) return;

            if (!lobby.BlueTeam.Players.Contains(player) && !lobby.RedTeam.Players.Contains(player)) return;

            List<Mine> mines = new List<Mine>();

            foreach (Mine mine in lobby.mines)
            {
                if (!mine.IsActive) continue;
                if (mine.Owner == player) continue;

                if (Math.Abs(x - mine.Location.X) <= 2 &&
                    Math.Abs(y - mine.Location.Y) <= 2 &&
                    Math.Abs(z - mine.Location.Z) <= 2)
                {
                    mines.Add(mine);
                }
            }

            if (mines.Count > 0)
            {
                lobby.RespawnPlayer(player);
            }

            foreach (Mine mine in mines)
            {
                player.Message($"There was a mine at {mine.Location.X} {mine.Location.Y} {mine.Location.Z}.");
                RemoveMine(player, mine, lobby);
            }
        }

        public static void RemoveMine(Player player, Mine mine, Lobby lobby)
        {
            lobby.Map.UpdateBlock(player, (ushort)mine.Location.X, (ushort)mine.Location.Y, (ushort)mine.Location.Z, Block.Air);
            player.Message($"Removed a mine at {mine.Location.X} {mine.Location.Y} {mine.Location.Z}.");
            lobby.mines.Remove(mine);
        }

        public static Mine GetMineAtPosition(Lobby lobby, ushort x, ushort y, ushort z)
        {
            foreach (Mine mine in lobby.mines)
            {
                if (mine.Location.X == x && mine.Location.Y == y && mine.Location.Z == z)
                {
                    return mine;
                }
            }

            return null;
        }
    }
}
