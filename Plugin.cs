using MCGalaxy;
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
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(Command.Find("JoinTeam"));
            Command.Unregister(Command.Find("Lobby"));

            // Create a copy of the lobbies list to avoid enumeration errors.
            List<Lobby> lobbiesCopy = new List<Lobby>(LobbyManager.GetLobbies());

            foreach (Lobby lobby in lobbiesCopy)
            {
                lobby.EndGame(true);
            }
        }

        public override void Help(Player p)
        {
        }
    }
}