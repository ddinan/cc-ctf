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

            if (lobby.BlueTeam.Players.Contains(p) && team.CaselessEq("blue"))
            {
                p.Message("&cYou are already on this team.");
                return;
            }

            if (lobby.RedTeam.Players.Contains(p) && team.CaselessEq("red"))
            {
                p.Message("&cYou are already on this team.");
                return;
            }

            if (team.CaselessEq("blue"))
            {
                if (lobby.BlueTeam.Players.Contains(p))
                {
                    p.Message("&cYou are already on this team.");
                    return;
                }

                if (lobby.RedTeam.Players.Contains(p))
                {
                    lobby.RedTeam.Players.Remove(p);
                }

                lobby.BlueTeam.Players.Add(p);
                lobby.MessagePlayers($"&b{p.truename} &Sjoined the &9blue &Steam.");
            }

            else if (team.CaselessEq("red"))
            {
                if (lobby.RedTeam.Players.Contains(p))
                {
                    p.Message("&cYou are already on this team.");
                    return;
                }

                if (lobby.BlueTeam.Players.Contains(p))
                {
                    lobby.BlueTeam.Players.Remove(p);
                }

                lobby.RedTeam.Players.Add(p);
                lobby.MessagePlayers($"&b{p.truename} &Sjoined the &cred &Steam.");
            }

            else
            {
                if (lobby.BlueTeam.Players.Contains(p))
                {
                    lobby.BlueTeam.Players.Remove(p);
                }

                if (lobby.RedTeam.Players.Contains(p))
                {
                    lobby.RedTeam.Players.Remove(p);
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