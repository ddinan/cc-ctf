﻿using MCGalaxy;
using MCGalaxy.Maths;
using MCGalaxy.Tasks;
using System;
using System.Collections.Generic;

using BlockID = System.UInt16;

namespace CTF.Items
{
    public class Rocket
    {
        readonly Dictionary<Player, RocketData> activeRockets = new Dictionary<Player, RocketData>();

        public void LaunchRocket(Player p, ushort yaw, ushort pitch)
        {
            Vec3F32 dir = DirUtils.GetDirVectorExt(yaw, pitch);

            RocketData data = MakeArgs(p, dir);
            activeRockets[p] = data;

            SchedulerTask task = new SchedulerTask(RocketCallback, data, TimeSpan.FromMilliseconds(65), true);
            p.CriticalTasks.Add(task);
        }

        RocketData MakeArgs(Player p, Vec3F32 dir)
        {
            RocketData args = new RocketData();
            args.block = Block.Lime;

            args.drag = new Vec3F32(0.99f, 0.99f, 0.99f);
            args.gravity = 0.00f;
            args.thrust = new Vec3F32(dir.X * 0.5f, dir.Y * 0.5f, dir.Z * 0.5f);

            args.pos = p.Pos.BlockCoords;
            args.last = Round(args.pos);
            args.next = Round(args.pos);

            args.vel = new Vec3F32(dir.X * 1.5f, dir.Y * 1.5f, dir.Z * 1.5f);
            args.player = p;
            return args;
        }

        void RevertLast(Player p, RocketData data)
        {
            p.level.BroadcastRevert(data.last.X, data.last.Y, data.last.Z);
        }

        void UpdateNext(Player p, RocketData data)
        {
            p.level.BroadcastChange(data.next.X, data.next.Y, data.next.Z, data.block);
        }

        void OnHitBlock(RocketData data, Vec3U16 pos, BlockID block)
        {
            data.player.Message("hit a block");
        }

        void OnHitPlayer(RocketData args, Player pl)
        {
            pl.HandleDeath(Block.Cobblestone, "@p &Swas shot");
        }

        void RocketCallback(SchedulerTask task)
        {
            RocketData data = (RocketData)task.State;
            if (TickRocket(data)) return;

            RevertLast(data.player, data);
            task.Repeating = false;
            activeRockets.Remove(data.player);
        }

        static Vec3U16 Round(Vec3F32 v)
        {
            unchecked { return new Vec3U16((ushort)Math.Round(v.X), (ushort)Math.Round(v.Y), (ushort)Math.Round(v.Z)); }
        }

        bool TickRocket(RocketData data)
        {
            Player p = data.player;
            Vec3U16 pos = data.next;
            BlockID cur = p.level.GetBlock(pos.X, pos.Y, pos.Z);

            if (cur == Block.Invalid) return false;
            if (cur != Block.Air) { OnHitBlock(data, pos, cur); return false; }

            Player pl = PlayerAt(p, pos, true);
            if (pl != null) { OnHitPlayer(data, pl); return false; }

            // Apply physics.
            data.pos += data.vel;
            data.vel += data.thrust;
            data.vel.X *= data.drag.X; data.vel.Y *= data.drag.Y; data.vel.Z *= data.drag.Z;
            data.vel.Y -= data.gravity;

            data.next = Round(data.pos);
            if (data.last == data.next) return true;

            // Update projectile position.
            RevertLast(p, data);
            UpdateNext(p, data);
            data.last = data.next;
            return true;
        }

        // Stolen from MCGalaxy.
        private Player PlayerAt(Player p, Vec3U16 pos, bool skipSelf)
        {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players)
            {
                if (pl.level != p.level) continue;
                if (p == pl && skipSelf) continue;

                if (Math.Abs(pl.Pos.BlockX - pos.X) <= 1
                    && Math.Abs(pl.Pos.BlockY - pos.Y) <= 1
                    && Math.Abs(pl.Pos.BlockZ - pos.Z) <= 1)
                {
                    return pl;
                }
            }
            return null;
        }
    }

    public class RocketData
    {
        public Player player;
        public BlockID block;
        public Vec3F32 pos, vel, thrust;
        public Vec3U16 last, next;
        public Vec3F32 drag;
        public float gravity;
    }
}