using MCGalaxy;
using System.Collections.Generic;
using System.Globalization;

namespace CTF
{
    public sealed class CmdClass : Command2
    {
        public override string name { get { return "Class"; } }
        public override string shortcut { get { return "kit"; } }
        public override string type { get { return "information"; } }

        public override void Use(Player p, string message, CommandData data)
        {
            string[] args = message.SplitSpaces();

            if (message.Length == 0)
            {
                if (p.Extras.GetString("CTF_ACTIVE_CLASS") == null)
                {
                    p.Message("&SYou do not have an active class.");
                }

                else
                {
                    p.Message($"&SYour class is: &b{p.Extras.GetString("CTF_ACTIVE_CLASS")}&S.");
                }

                p.Message("&SYou may change your class with &b/Class [class name]&S.");
                return;
            }

            Lobby lobby = LobbyManager.GetPlayerLobby(p);

            if (lobby != null && lobby.IsGameRunning)
            {
                p.Message("&cYou may not change classes during a game.");
                return;
            }

            List<string> classes = new List<string> { "Bridger", "Demolitionist", "Grenadier", "Guardian", "Mechanic", "Pyromaniac", "Shield", "Sniper" };

            if (!classes.CaselessContains(args[0]))
            {
                p.Message($"&cValid classes are: &b{string.Join("&c, &b", classes)}");
                return;
            }

            SetClass(p, args[0]);
        }

        private void SetClass(Player p, string className)
        {
            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
            p.Extras["CTF_ACTIVE_CLASS"] = ti.ToTitleCase(className);
            p.Message($"&SYou set your class to &b{ti.ToTitleCase(className)}.");
        }

        public override void Help(Player p)
        {
            p.Message("%T/Class [class name] %H- Sets your active class to [class name].");
        }
    }
}