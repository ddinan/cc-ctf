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

        public override void Help(Player player)
        {
            player.Message("%T/Class [class name] %H- Sets your active class to [class name].");
        }

        public override void Use(Player player, string message, CommandData data)
        {
            Lobby lobby = LobbyManager.GetPlayerLobby(player);

            if (lobby == null)
            {
                player.Message("&cYou need to be in a lobby to use this command.");
                return;
            }

            string[] args = message.SplitSpaces();

            if (message.Length == 0)
            {
                if (!PlayerClassManager.HasActiveClass(player.truename))
                {
                    player.Message("&SYou do not have an active class.");
                }

                else
                {
                    var playerClass = PlayerClassManager.GetPlayerClass(player.truename);
                    player.Message($"&SYour class is: &b{playerClass.Name}&S.");
                }

                player.Message("&SYou may change your class with &b/Class [class name]&S.");
                return;
            }

            if (message.CaselessEq("usePowerUp"))
            {
                if (!PlayerClassManager.HasActiveClass(player.truename)) return;

                PlayerClass playerClass = PlayerClassManager.GetPlayerClass(player.truename);
                if (playerClass == null) return;

                if (!lobby.IsGameRunning)
                {
                    player.Message("&cPlease wait until the game has started to use power ups.");
                    return;
                }

                if (playerClass.Name.CaselessEq("Pyromaniac") && player.Extras.GetBoolean("CTF_FLAMETHROWER_ACTIVATED"))
                { // Turning the flamethrower off shouldn't affect the cooldown.
                    playerClass.UseAbility(player);
                    return;
                }

                if (playerClass.PowerUpCooldown > 0)
                {
                    player.Message($"&cYou need to wait &b{playerClass.PowerUpCooldown}s &cto use this power up.");
                    return;
                }

                playerClass.UseAbility(player);
                return;
            }

            if (lobby.IsGameRunning)
            {
                player.Message("&cYou may not change classes during a game.");
                return;
            }

            List<string> classes = new List<string> { "Bridger", "Demolitionist", "Grenadier", "Guardian", "Mechanic", "Pyromaniac", "Shield", "Sniper" };

            if (!classes.CaselessContains(args[0]))
            {
                player.Message($"&cValid classes are: &b{string.Join("&c, &b", classes)}");
                return;
            }

            SetClass(player, args[0]);
        }

        private void SetClass(Player player, string className)
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
                PlayerClassManager.SetPlayerClass(player.truename, classMappings[formattedClassName]);
                player.Message($"&SYou set your class to &b{formattedClassName}&S.");
            }

            else
            {
                player.Message($"&cClass &b{formattedClassName}&c does not exist.");
            }
        }
    }
}