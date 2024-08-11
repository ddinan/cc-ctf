// TODO: Bridges should cancel when hitting other bridges.

using MCGalaxy;
using MCGalaxy.Maths;
using System;
using System.Threading.Tasks;
using BlockID = System.UInt16;

namespace CTF.Items
{
    public class Bridge
    {
        public static async void BuildBridge(Player player, ushort yaw, ushort pitch)
        {
            Vec3F32 dir = DirUtils.GetDirVectorExt(yaw, pitch);
            Vec3F32 normalizedDir = Vec3F32.Normalise(dir);

            Vec3F32 currentPos = new Vec3F32(player.Pos.BlockX, player.Pos.BlockY, player.Pos.BlockZ);

            for (int i = 0; i <= Math.Max(player.level.Length, Math.Max(player.level.Width, player.level.Height)); i++)
            {
                BlockID cur = player.level.GetBlock((ushort)currentPos.X, (ushort)currentPos.Y, (ushort)currentPos.Z);
                if (cur != Block.Air && cur != Block.Wood && cur != Block.StillWater) return;

                // Leave a 1-block gap between the player and the bridge.
                if (i > 1)
                {
                    player.level.UpdateBlock(player, (ushort)currentPos.X, (ushort)currentPos.Y, (ushort)currentPos.Z, Block.Wood);
                }

                await Task.Delay(15);

                currentPos += normalizedDir;
            }
        }
    }
}
