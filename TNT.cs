using MCGalaxy;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace CTF
{
    public class TNT
    {
        public static void HandleBlockChanging(Player p, ushort x, ushort y, ushort z, BlockID block, bool placing, ref bool cancel)
        {
            Lobby lobby = LobbyManager.GetPlayerLobby(p);

            if (lobby == null)
            {
                p.Message("&cYou need to be in a lobby to modify blocks.");
                cancel = true;
                p.RevertBlock(x, y, z);
                return;
            }

            if (!lobby.BlueTeam.Players.Contains(p) && !lobby.RedTeam.Players.Contains(p))
            {
                p.Message("&cYou need to be in a team to modify blocks.");
                cancel = true;
                p.RevertBlock(x, y, z);
                return;
            }

            if (placing && block == Block.TNT)
            {
                if (p.Extras.GetBoolean("CTF_HAS_TNT"))
                {
                    p.Extras["CTF_HAS_TNT"] = false;
                    string[] coords = p.Extras.GetString("CTF_TNT_COORDS").Split(';');

                    // Revert the last TNT.
                    ushort oldX = ushort.Parse(coords[0]);
                    ushort oldY = ushort.Parse(coords[1]);
                    ushort oldZ = ushort.Parse(coords[2]);

                    ModifyMap(p, oldX, oldY, oldZ);
                    KillPlayers(p, oldX, oldY, oldZ, lobby);
                    p.level.UpdateBlock(p, oldX, oldY, oldZ, Block.Air);

                    // Revert the current TNT.
                    cancel = true;
                    p.RevertBlock(x, y, z);
                }
                else
                {
                    p.Extras["CTF_HAS_TNT"] = true;
                    p.Extras["CTF_TNT_COORDS"] = $"{x};{y};{z}";
                }
            }
        }

        private static void ModifyMap(Player p, ushort x, ushort y, ushort z, int radius = 2)
        {
            BufferedBlockSender buffer = new BufferedBlockSender(p.level);

            for (int dx = x - radius; dx <= x + radius; dx++)
            {
                for (int dy = y - radius; dy <= y + radius; dy++)
                {
                    for (int dz = z - radius; dz <= z + radius; dz++)
                    {
                        BlockID block = p.level.GetBlock(x, y, z);

                        if (p.level.Props[block].OPBlock || block == Block.Bedrock) continue; // Don't explode OP blocks
                        if (block == Block.Invalid) continue;

                        p.level.SetBlock((ushort)dx, (ushort)dy, (ushort)dz, Block.Air);
                        int index = p.level.PosToInt((ushort)dx, (ushort)dy, (ushort)dz);
                        buffer.Add(index, Block.Air);
                    }
                }
            }

            buffer.Flush();
        }

        private static void KillPlayers(Player p, ushort x, ushort y, ushort z, Lobby lobby, int radius = 2)
        {
            foreach (Player pl in lobby.Players)
            {
                if (pl == p) continue;

                // Do not kill if the target is a spectator or they are on the same team as the player.
                if (lobby.BlueTeam.Players.Contains(pl) && lobby.BlueTeam.Players.Contains(p)) continue;
                if (lobby.RedTeam.Players.Contains(pl) && lobby.RedTeam.Players.Contains(p)) continue;
                if (!lobby.BlueTeam.Players.Contains(pl) && !lobby.RedTeam.Players.Contains(pl)) continue;

                int dx = pl.Pos.BlockX - x, dy = pl.Pos.FeetBlockCoords.Y - y, dz = pl.Pos.BlockZ - z;

                // If the target player is in range of the TNT.
                if ((dx * dx) + (dy * dy) + (dz * dz) <= radius * radius)
                {
                    pl.HandleDeath(Block.Cobblestone, $"{pl.truename} %Swas exploded by {p.truename}.");
                }
            }
        }
    }
}
