// TODO: Add fuel depletion.

using MCGalaxy;
using MCGalaxy.Maths;
using MCGalaxy.Tasks;
using System;
using System.Collections.Generic;

using BlockID = System.UInt16;

namespace CTF.Items
{
    public class Flamethrower
    {
        Dictionary<Player, FlamethrowerData> activeFlamethrowers = new Dictionary<Player, FlamethrowerData>();

        public void ActivateFlamethrower(Player player, ushort yaw, ushort pitch)
        {
            Vec3F32 dir = DirUtils.GetDirVectorExt(yaw, pitch);

            FlamethrowerData data = MakeArgs(player, dir);
            activeFlamethrowers[player] = data;

            player.Extras["CTF_FLAMETHROWER_ACTIVATED"] = true;

            SchedulerTask task = new SchedulerTask(FlamethrowerCallback, data, TimeSpan.FromMilliseconds(50), true);
            player.CriticalTasks.Add(task);
        }

        FlamethrowerData MakeArgs(Player player, Vec3F32 dir)
        {
            FlamethrowerData args = new FlamethrowerData();
            args.block = Block.Lava;

            // Offset the starting position by 3 blocks in the direction of the flamethrower
            args.pos = player.Pos.BlockCoords + dir * 3;
            args.dir = dir;
            args.player = player;
            args.length = 3;

            return args;
        }

        void RevertFlame(FlamethrowerData data, bool revertCurrent = false)
        {
            Player player = data.player;

            foreach (Vec3U16 pos in data.previousFlamePositions)
            {
                if (!data.flamePositions.Contains(pos))
                {
                    player.level.UpdateBlock(player, pos.X, pos.Y, pos.Z, Block.Air);
                }
            }

            if (revertCurrent)
            {
                foreach (Vec3U16 pos in data.flamePositions)
                {
                    player.level.UpdateBlock(player, pos.X, pos.Y, pos.Z, Block.Air);
                }
            }
        }

        void UpdateFlame(FlamethrowerData data)
        {
            Player player = data.player;
            data.previousFlamePositions = new List<Vec3U16>(data.flamePositions);
            data.flamePositions.Clear();

            for (int i = 0; i < data.length; i++)
            {
                Vec3U16 pos = Round(data.pos + data.dir * i);
                BlockID cur = player.level.GetBlock(pos.X, pos.Y, pos.Z);

                // Stop if we hit a non-air block.
                if (cur != Block.Air && cur != Block.Lava)
                {
                    StopFlamethrower(data);
                    return;
                }

                // Check if we hit a player.
                Player victim = PlayerAt(player, pos, true);
                if (victim != null)
                {
                    OnHitPlayer(data, victim);
                    StopFlamethrower(data);
                    return;
                }

                data.flamePositions.Add(pos);
                if (!data.previousFlamePositions.Contains(pos))
                {
                    player.level.UpdateBlock(player, pos.X, pos.Y, pos.Z, data.block);
                }
            }
        }

        void StopFlamethrower(FlamethrowerData data)
        {
            data.player.Extras["CTF_FLAMETHROWER_ACTIVATED"] = false;
            RevertFlame(data, true);
        }

        void FlamethrowerCallback(SchedulerTask task)
        {
            FlamethrowerData data = (FlamethrowerData)task.State;

            if (!data.player.Extras.GetBoolean("CTF_FLAMETHROWER_ACTIVATED"))
            {
                RevertFlame(data, true);
                task.Repeating = false;
                activeFlamethrowers.Remove(data.player);
                return;
            }

            // Update position and flame.
            data.pos = data.player.Pos.BlockCoords + data.dir * 3; // Maintain the 3-block offset.
            data.dir = DirUtils.GetDirVector(data.player.Rot.RotY, data.player.Rot.HeadX);
            RevertFlame(data);
            UpdateFlame(data);
        }

        static Vec3U16 Round(Vec3F32 v)
        {
            unchecked { return new Vec3U16((ushort)Math.Round(v.X), (ushort)Math.Round(v.Y), (ushort)Math.Round(v.Z)); }
        }

        void OnHitPlayer(FlamethrowerData data, Player victim)
        {
            victim.HandleDeath(Block.Lava, "@p &Swas cooked by " + data.player.DisplayName);
        }

        // Stolen from MCGalaxy.
        private Player PlayerAt(Player player, Vec3U16 pos, bool skipSelf)
        {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players)
            {
                if (pl.level != player.level) continue;
                if (player == pl && skipSelf) continue;

                if (Math.Abs(pl.Pos.BlockX - pos.X) <= 1
                    && Math.Abs(pl.Pos.BlockY - pos.Y) <= 1
                    && Math.Abs(pl.Pos.BlockZ - pos.Z) <= 1)
                {
                    return pl;
                }
            }
            return null;
        }

        public void DeactivateFlamethrower(Player player)
        {
            if (activeFlamethrowers.TryGetValue(player, out FlamethrowerData data))
            {
                player.Extras["CTF_FLAMETHROWER_ACTIVATED"] = false;
            }
        }
    }

    public class FlamethrowerData
    {
        public Player player;
        public BlockID block;
        public Vec3F32 pos, dir;
        public List<Vec3U16> flamePositions = new List<Vec3U16>();
        public List<Vec3U16> previousFlamePositions = new List<Vec3U16>();
        public int length;
    }
}
