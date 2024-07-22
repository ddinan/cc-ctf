using MCGalaxy;

namespace CTF
{
    public sealed class CmdJoinTeam : Command2
    {
        public override string name { get { return "JoinTeam"; } }
        public override string shortcut { get { return "jt"; } }
        public override string type { get { return "information"; } }

        public override void Use(Player p, string message, CommandData data)
        {
            string[] args = message.SplitSpaces();

            if (args.Length == 0)
            {
                Help(p);
                return;
            }

            switch (args[0])
            {
                case "red":
                    JoinTeam(p, "red");
                    break;
                case "blue":
                    JoinTeam(p, "blue");
                    break;
                case "spectator":
                    JoinTeam(p, "spectator");
                    break;
                default:
                    JoinTeam(p, "spectator");
                    break;
            }
        }

        private void JoinTeam(Player p, string team)
        {
            Lobby lobby = LobbyManager.GetPlayerLobby(p);

            if (lobby == null)
            {
                p.Message("&cYou are not in a lobby.");
                return;
            }

            if (lobby.BluePlayers.Contains(p) && team.CaselessEq("blue"))
            {
                p.Message("&cYou are already on this team.");
                return;
            }

            if (lobby.RedPlayers.Contains(p) && team.CaselessEq("red"))
            {
                p.Message("&cYou are already on this team.");
                return;
            }

            if (team.CaselessEq("blue"))
            {
                if (lobby.BluePlayers.Contains(p))
                {
                    p.Message("&cYou are already on this team.");
                    return;
                }

                if (lobby.RedPlayers.Contains(p))
                {
                    lobby.RedPlayers.Remove(p);
                }

                lobby.BluePlayers.Add(p);
                lobby.MessagePlayers($"&b{p.truename} &Sjoined the &9blue &Steam.");
            }

            else if (team.CaselessEq("red"))
            {
                if (lobby.RedPlayers.Contains(p))
                {
                    p.Message("&cYou are already on this team.");
                    return;
                }

                if (lobby.BluePlayers.Contains(p))
                {
                    lobby.BluePlayers.Remove(p);
                }

                lobby.RedPlayers.Add(p);
                lobby.MessagePlayers($"&b{p.truename} &Sjoined the &cred &Steam.");
            }

            else
            {
                if (lobby.BluePlayers.Contains(p))
                {
                    lobby.BluePlayers.Remove(p);
                }

                if (lobby.RedPlayers.Contains(p))
                {
                    lobby.RedPlayers.Remove(p);
                }

                lobby.MessagePlayers($"&b{p.truename} &Sjoined the &7spectator &Steam.");
            }
        }

        public override void Help(Player p)
        {
            p.Message("%T/Team red/blue/spectator %H- Joins a team.");
        }
    }
}