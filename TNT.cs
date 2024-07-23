using MCGalaxy;
using MCGalaxy.Network;
using System.Reflection.Emit;
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

            if (!lobby.BluePlayers.Contains(p) && !lobby.RedPlayers.Contains(p))
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
                    p.Message("explode");
                    p.Extras["CTF_HAS_TNT"] = false;
                    string[] coords = p.Extras.GetString("CTF_TNT_COORDS").Split(';');

                    // Revert the last TNT.
                    ushort oldX = ushort.Parse(coords[0]);
                    ushort oldY = ushort.Parse(coords[1]);
                    ushort oldZ = ushort.Parse(coords[2]);

                    DoExplode(p, oldX, oldY, oldZ);
                    p.level.UpdateBlock(p, oldX, oldY, oldZ, Block.Air);

                    // Revert the current TNT.
                    cancel = true;
                    p.RevertBlock(x, y, z);
                }
                else
                {
                    p.Message("awaiting another tnt");
                    p.Extras["CTF_HAS_TNT"] = true;
                    p.Extras["CTF_TNT_COORDS"] = $"{x};{y};{z}";
                }
            }
        }

        private static void DoExplode(Player p, ushort x, ushort y, ushort z, int radius = 2)
        {
            BufferedBlockSender buffer = new BufferedBlockSender(p.level);

            for (int dx = x - radius; dx <= x + radius; dx++)
            {
                for (int dy = y - radius; dy <= y + radius; dy++)
                {
                    for (int dz = z - radius; dz <= z + radius; dz++)
                    {
                        BlockID block = p.level.GetBlock(x, y, z);

                        if (p.level.Props[block].OPBlock) continue; // Don't explode OP blocks
                        if (block == Block.Invalid) continue;

                        p.level.SetBlock((ushort)dx, (ushort)dy, (ushort)dz, Block.Air);
                        int index = p.level.PosToInt((ushort)dx, (ushort)dy, (ushort)dz);
                        buffer.Add(index, Block.Air);
                    }
                }
            }

            buffer.Flush();
        }
    }
}
