using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using System.Collections.Generic;

namespace CTF
{
    public class CTF : Plugin
    {
        public override string name { get { return "CTF"; } }
        public override string MCGalaxy_Version { get { return "1.9.4.9"; } }
        public override string creator { get { return "Venk"; } }

        public override void Load(bool startup)
        {
            Command.Register(new CmdJoinTeam());
            Command.Register(new CmdLobby());

            OnBlockChangingEvent.Register(TNT.HandleBlockChanging, Priority.Low);
            OnPlayerClickEvent.Register(HandlePlayerClick, Priority.Low);
            OnPlayerFinishConnectingEvent.Register(HandlePlayerFinishConnecting, Priority.Low);
            

            LobbyManager.CreateNewLobby(Player.Console); // Create the default lobby.
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(Command.Find("JoinTeam"));
            Command.Unregister(Command.Find("Lobby"));

            OnBlockChangingEvent.Unregister(TNT.HandleBlockChanging);
            OnPlayerClickEvent.Unregister(HandlePlayerClick);
            OnPlayerFinishConnectingEvent.Unregister(HandlePlayerFinishConnecting);

            Server.SetMainLevel(Server.Config.MainLevel); // Revert the main level back to what it was before so we can delete the default lobby's level.

            List<Lobby> lobbiesCopy = new List<Lobby>(LobbyManager.GetLobbies()); // Create a copy of the lobbies list to avoid enumeration errors.

            foreach (Lobby lobby in lobbiesCopy)
            {
                lobby.EndGame(true);
            }
        }

        public override void Help(Player p)
        {
        }

        private void HandlePlayerClick(Player p, MouseButton button, MouseAction action, ushort yaw, ushort pitch, byte entity, ushort x, ushort y, ushort z, TargetBlockFace face)
        {
            if (action != MouseAction.Pressed) return;
            if (entity != Entities.SelfID && ClickOnBot(p, entity)) return;
        }

        private bool ClickOnBot(Player p, byte entity)
        {
            Lobby lobby = LobbyManager.GetPlayerLobby(p);
            if (lobby == null) return false;

            if (!lobby.BlueTeam.Players.Contains(p) && !lobby.RedTeam.Players.Contains(p)) return false;

            PlayerBot[] bots = p.level.Bots.Items;
            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i].EntityID != entity) continue;

                Vec3F32 delta = p.Pos.ToVec3F32() - bots[i].Pos.ToVec3F32();
                float reachSq = p.ReachDistance * p.ReachDistance;
                if (delta.LengthSquared > (reachSq + 1)) return false;

                if (bots[i].name.CaselessEq("blue_flag"))
                {
                    if (lobby.RedTeam.Players.Contains(p))
                    { // Take the flag if the player is on the enemy team.
                        lobby.ClickOnFlag(p, bots[i]);
                        return true;
                    }
                    
                    else if (lobby.BlueTeam.Players.Contains(p) && p.Extras.GetBoolean("CTF_HAS_FLAG"))
                    { // Capture the flag if the player has the enemy flag.
                        lobby.CaptureFlag(p);
                        return true;
                    }
                }

                else if (bots[i].name.CaselessEq("red_flag"))
                {
                    if (lobby.BlueTeam.Players.Contains(p))
                    { // Take the flag if the player is on the enemy team.
                        lobby.ClickOnFlag(p, bots[i]);
                        return true;
                    }

                    else if (lobby.RedTeam.Players.Contains(p) && p.Extras.GetBoolean("CTF_HAS_FLAG"))
                    { // Capture the flag if the player has the enemy flag.
                        lobby.CaptureFlag(p);
                        return true;
                    }
                }
                return true;
            }
            return false;
        }

        private void HandlePlayerFinishConnecting(Player p)
        {
            LobbyManager.JoinLobby(p, 1, true);
        }
    }
}