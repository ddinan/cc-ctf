using MCGalaxy;
using MCGalaxy.Events.PlayerEvents;
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

            OnPlayerFinishConnectingEvent.Register(HandlePlayerFinishConnecting, Priority.Low);
            OnBlockChangingEvent.Register(TNT.HandleBlockChanging, Priority.Low);

            LobbyManager.CreateNewLobby(Player.Console); // Create the default lobby.
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(Command.Find("JoinTeam"));
            Command.Unregister(Command.Find("Lobby"));

            OnPlayerFinishConnectingEvent.Unregister(HandlePlayerFinishConnecting);
            OnBlockChangingEvent.Unregister(TNT.HandleBlockChanging);

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

        void HandlePlayerFinishConnecting(Player p)
        {
            LobbyManager.JoinLobby(p, 1, true);
        }
    }
}