using MCGalaxy;
using MCGalaxy.Network;
using System.Threading.Tasks;
using BlockID = System.UInt16;

namespace CTF.Items
{
    public class TNT
    {
        // TODO: Move HandleBlockChanging out of TNT class.
        public static void HandleBlockChanging(Player player, ushort x, ushort y, ushort z, BlockID block, bool placing, ref bool cancel)
        {
            Lobby lobby = LobbyManager.GetPlayerLobby(player);

            if (lobby == null)
            {
                player.Message("&cYou need to be in a lobby to modify blocks.");
                cancel = true;
                player.RevertBlock(x, y, z);
                return;
            }

            if (!lobby.BlueTeam.Players.Contains(player) && !lobby.RedTeam.Players.Contains(player))
            {
                player.Message("&cYou need to be in a team to modify blocks.");
                cancel = true;
                player.RevertBlock(x, y, z);
                return;
            }

            if (placing && block == Block.Gray)
            {
                Position position = new Position(x, y, z);
                Mine mine = new Mine(player, position);
                lobby.mines.Add(mine);

                player.Message($"&SAdded a mine at {mine.Location.X} {mine.Location.Y} {mine.Location.Z}.");

                // Wait for 3 seconds before activating the mine.
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    mine.IsActive = true;
                    player.Message("&SMine is now active!");
                });

                return;
            }

            if (placing && block == Block.TNT)
            {
                if (player.Extras.GetBoolean("CTF_HAS_TNT"))
                {
                    player.Extras["CTF_HAS_TNT"] = false;
                    string[] coords = player.Extras.GetString("CTF_TNT_COORDS").Split(';');

                    // Revert the last TNT.
                    ushort oldX = ushort.Parse(coords[0]);
                    ushort oldY = ushort.Parse(coords[1]);
                    ushort oldZ = ushort.Parse(coords[2]);

                    ModifyMap(player, lobby, oldX, oldY, oldZ);
                    KillPlayers(player, oldX, oldY, oldZ, lobby);
                    player.level.UpdateBlock(player, oldX, oldY, oldZ, Block.Air);


                    // Revert the current TNT.
                    cancel = true;
                    player.RevertBlock(x, y, z);
                }

                else
                {
                    player.Extras["CTF_HAS_TNT"] = true;
                    player.Extras["CTF_TNT_COORDS"] = $"{x};{y};{z}";
                }

                return;
            }
        }

        private static void ModifyMap(Player player, Lobby lobby, ushort x, ushort y, ushort z, int radius = 2)
        {
            BufferedBlockSender buffer = new BufferedBlockSender(player.level);

            for (int dx = x - radius; dx <= x + radius; dx++)
            {
                for (int dy = y - radius; dy <= y + radius; dy++)
                {
                    for (int dz = z - radius; dz <= z + radius; dz++)
                    {
                        BlockID block = player.level.GetBlock(x, y, z);

                        if (player.level.Props[block].OPBlock || block == Block.Bedrock) continue; // Don't explode OP blocks.
                        if (block == Block.Invalid) continue;

                        Mine mine = Mines.GetMineAtPosition(lobby, (ushort)dx, (ushort)dy, (ushort)dz);
                        if (mine != null)
                        {
                            if (mine.Owner == player) continue;
                            Mines.RemoveMine(player, mine, lobby);
                        }

                        player.level.SetBlock((ushort)dx, (ushort)dy, (ushort)dz, Block.Air);
                        int index = player.level.PosToInt((ushort)dx, (ushort)dy, (ushort)dz);
                        buffer.Add(index, Block.Air);
                    }
                }
            }

            buffer.Flush();
        }

        private static void KillPlayers(Player player, ushort x, ushort y, ushort z, Lobby lobby, int radius = 2)
        {
            foreach (Player pl in lobby.Players)
            {
                if (pl == player) continue;

                // Do not kill if the target is a spectator or they are on the same team as the player.
                if (lobby.BlueTeam.Players.Contains(pl) && lobby.BlueTeam.Players.Contains(player)) continue;
                if (lobby.RedTeam.Players.Contains(pl) && lobby.RedTeam.Players.Contains(player)) continue;
                if (!lobby.BlueTeam.Players.Contains(pl) && !lobby.RedTeam.Players.Contains(pl)) continue;

                int dx = pl.Pos.BlockX - x, dy = pl.Pos.FeetBlockCoords.Y - y, dz = pl.Pos.BlockZ - z;

                // If the target player is in range of the TNT.
                if (dx * dx + dy * dy + dz * dz <= radius * radius)
                {
                    lobby.MessagePlayers($"{pl.truename} %Swas exploded by {player.truename}.");
                    lobby.RespawnPlayer(pl);
                }
            }
        }
    }
}
