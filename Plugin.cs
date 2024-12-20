﻿using CTF.Items;
using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using System;
using System.Collections.Generic;
using System.IO;
using BlockID = System.UInt16;

namespace CTF
{
    public class CTF : Plugin
    {
        public override string name { get { return "CTF"; } }
        public override string MCGalaxy_Version { get { return "1.9.4.9"; } }
        public override string creator { get { return "Venk"; } }

        public override void Load(bool startup)
        {
            Command.Register(new CmdClass());
            Command.Register(new CmdJoinTeam());
            Command.Register(new CmdLobby());

            OnBlockChangingEvent.Register(TNT.HandleBlockChanging, Priority.Low);
            OnPlayerClickEvent.Register(HandlePlayerClick, Priority.Low);
            OnPlayerFinishConnectingEvent.Register(HandlePlayerFinishConnecting, Priority.Low);
            OnPlayerMoveEvent.Register(Mines.HandlePlayerMove, Priority.Low);

            if (Directory.Exists("./levels/"))
            { // Delete any ghost lobby levels.
                string[] files = Directory.GetFiles("./levels/", "lobby*.*");

                foreach (string file in files)
                {
                    if (file.EndsWith(".lvl", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".backup", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            File.Delete(file);
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                        }
                    }
                }
            }

            LobbyManager.CreateNewLobby(Player.Console); // Create the default lobby.
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(Command.Find("Class"));
            Command.Unregister(Command.Find("JoinTeam"));
            Command.Unregister(Command.Find("Lobby"));

            OnBlockChangingEvent.Unregister(TNT.HandleBlockChanging);
            OnPlayerClickEvent.Unregister(HandlePlayerClick);
            OnPlayerFinishConnectingEvent.Unregister(HandlePlayerFinishConnecting);
            OnPlayerMoveEvent.Unregister(Mines.HandlePlayerMove);

            Server.SetMainLevel(Server.Config.MainLevel); // Revert the main level back to what it was before so we can delete the default lobby's level.

            List<Lobby> lobbiesCopy = new List<Lobby>(LobbyManager.GetLobbies()); // Create a copy of the lobbies list to avoid enumeration errors.

            foreach (Lobby lobby in lobbiesCopy)
            {
                lobby.EndGame(true);
            }
        }

        public override void Help(Player player)
        {
        }

        private void HandlePlayerClick(Player player, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            if (action != MouseAction.Pressed) return;
            if (entity != Entities.SelfID && ClickOnBot(player, entity)) return;

            BlockID heldBlock = player.GetHeldBlock();

            // Weapons testing.

            if (heldBlock == Block.Red)
            {
                player.Message("grenade");
                Grenade grenade = new Grenade();
                grenade.ThrowGrenade(player);
                return;
            }

            if (heldBlock == Block.Orange)
            {
                if (player.Extras.GetBoolean("CTF_FLAMETHROWER_ACTIVATED"))
                {
                    player.Message("flamethrower off");
                    player.Extras["CTF_FLAMETHROWER_ACTIVATED"] = false;
                    return;
                }

                else
                {
                    player.Message("flamethrower on");
                    Flamethrower flamethrower = new Flamethrower();
                    flamethrower.ActivateFlamethrower(player);
                    return;
                }
            }

            if (heldBlock == Block.Yellow)
            {
                player.Message("bridge");
                Bridge.BuildBridge(player);
                return;
            }

            if (heldBlock == Block.Lime)
            {
                player.Message("rocket");
                Rocket rocket = new Rocket();
                rocket.LaunchRocket(player);
                return;
            }

            if (heldBlock == Block.Green)
            {
                player.Message("shield");
                return;
            }
        }

        private bool ClickOnBot(Player player, byte entity)
        {
            Lobby lobby = LobbyManager.GetPlayerLobby(player);
            if (lobby == null) return false;

            if (!lobby.BlueTeam.Players.Contains(player) && !lobby.RedTeam.Players.Contains(player)) return false;

            PlayerBot[] bots = player.level.Bots.Items;
            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i].EntityID != entity) continue;

                Vec3F32 delta = player.Pos.ToVec3F32() - bots[i].Pos.ToVec3F32();
                float reachSq = player.ReachDistance * player.ReachDistance;
                if (delta.LengthSquared > (reachSq + 1)) return false;

                if (bots[i].name.CaselessEq("blue_flag"))
                {
                    if (lobby.RedTeam.Players.Contains(player))
                    { // Take the flag if the player is on the enemy team.
                        lobby.ClickOnFlag(player, bots[i]);
                        return true;
                    }

                    else if (lobby.BlueTeam.Players.Contains(player) && player.Extras.GetBoolean("CTF_HAS_FLAG"))
                    { // Capture the flag if the player has the enemy flag.
                        lobby.CaptureFlag(player);
                        return true;
                    }
                }

                else if (bots[i].name.CaselessEq("red_flag"))
                {
                    if (lobby.BlueTeam.Players.Contains(player))
                    { // Take the flag if the player is on the enemy team.
                        lobby.ClickOnFlag(player, bots[i]);
                        return true;
                    }

                    else if (lobby.RedTeam.Players.Contains(player) && player.Extras.GetBoolean("CTF_HAS_FLAG"))
                    { // Capture the flag if the player has the enemy flag.
                        lobby.CaptureFlag(player);
                        return true;
                    }
                }
                return true;
            }
            return false;
        }

        private void HandlePlayerFinishConnecting(Player player)
        {
            if (player.Supports(CpeExt.TextHotkey) && player.hasCP437)
            { // Initialize hotkeys if the player's client allows them.
                player.Send(Packet.TextHotKey("UsePowerUp", "/Class usePowerUp◙", 18, 0, true)); // E.
            }

            LobbyManager.JoinLobby(player, 1, true);
        }
    }
}