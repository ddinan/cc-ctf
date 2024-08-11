using CTF.Classes;
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
                if (!PlayerClassManager.HasActiveClass(p.truename))
                {
                    p.Message("&SYou do not have an active class.");
                }

                else
                {
                    var playerClass = PlayerClassManager.GetPlayerClass(p.truename);
                    p.Message($"&SYour class is: &b{playerClass.Name}&S.");
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
            Dictionary<string, PlayerClass> classMappings = new Dictionary<string, PlayerClass>
            {
                { "Bridger", new Bridger() },
                { "Demolitionist", new Demolitionist() },
                { "Grenadier", new Grenadier() },
                { "Guardian", new Guardian() },
                { "Mechanic", new Mechanic() },
                { "Pyromaniac", new Pyromaniac() },
                { "Shield", new Shield() },
                { "Sniper", new Sniper() }
            };

            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
            string formattedClassName = ti.ToTitleCase(className);

            if (classMappings.ContainsKey(formattedClassName))
            {
                PlayerClassManager.SetPlayerClass(p.truename, classMappings[formattedClassName]);
                p.Message($"&SYou set your class to &b{formattedClassName}&S.");
            }

            else
            {
                p.Message($"&cClass &b{formattedClassName}&c does not exist.");
            }
        }

        public override void Help(Player p)
        {
            p.Message("%T/Class [class name] %H- Sets your active class to [class name].");
        }
    }
}