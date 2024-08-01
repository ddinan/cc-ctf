using MCGalaxy;
using System;

namespace CTF
{
    public class Mine
    {
        public Position Location { get; private set; }
        public Player Owner { get; private set; }
        public bool IsActive = false;

        public Mine(Player p, Position location)
        {
            Location = location;
            Owner = p;
        }
    }

    public class Mines
    {
        public static void HandlePlayerMove(Player p, Position next, byte yaw, byte pitch, ref bool cancel)
        {
            int x = p.Pos.BlockX;
            int y = p.Pos.BlockY;
            int z = p.Pos.BlockZ;

            Lobby lobby = LobbyManager.GetPlayerLobby(p);
            if (lobby == null) return;

            Mine mine = null;

            foreach (Mine m in lobby.mines)
            {
                if (!lobby.BlueTeam.Players.Contains(p) && !lobby.RedTeam.Players.Contains(p)) continue;
                if (!m.IsActive) continue;
                if (m.Owner == p) continue;

                if (Math.Abs(x - m.Location.X) <= 2 &&
                    Math.Abs(y - m.Location.Y) <= 2 &&
                    Math.Abs(z - m.Location.Z) <= 2)
                {
                    mine = m;
                }
            }

            if (mine == null) return;

            p.Message($"There was a mine at {mine.Location.X} {mine.Location.Y} {mine.Location.Z}");
            p.level.UpdateBlock(p, (ushort)mine.Location.X, (ushort)mine.Location.Y, (ushort)mine.Location.Z, Block.Air);
            //Command.Find("Effect").Use(p, "explosion " + mine.Location.X + " " + mine.Location.Y + " " + mine.Location.Z + " 0 0 0 true");
            lobby.mines.Remove(mine);
            lobby.RespawnPlayer(p);
        }
    }
}
