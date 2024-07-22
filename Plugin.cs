﻿using MCGalaxy;

namespace CTF
{
    public class CTF : Plugin
    {
        public override string name { get { return "CTF"; } }
        public override string MCGalaxy_Version { get { return "1.9.4.9"; } }
        public override string creator { get { return "Venk"; } }

        public override void Load(bool startup)
        {
            Command.Register(new CmdLobby());
        }

        public override void Unload(bool shutdown)
        {
            Command.Unregister(Command.Find("Lobby"));

            foreach (Lobby lobby in LobbyManager.GetLobbies()) {
                lobby.EndGame();
            }
        }

        public override void Help(Player p)
        {
        }
    }
}