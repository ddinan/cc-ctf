using MCGalaxy;

namespace CTF
{
    public sealed class CmdJoinTeam : Command2
    {
        public override string name { get { return "JoinTeam"; } }
        public override string shortcut { get { return "jt"; } }
        public override string type { get { return "information"; } }

        public override void Use(Player player, string message, CommandData data)
        {
            string[] args = message.SplitSpaces();

            if (args.Length == 0)
            {
                Help(player);
                return;
            }

            switch (args[0])
            {
                case "red":
                    JoinTeam(player, "red");
                    break;
                case "blue":
                    JoinTeam(player, "blue");
                    break;
                case "spectator":
                    JoinTeam(player, "spectator");
                    break;
                default:
                    JoinTeam(player, "spectator");
                    break;
            }
        }

        private void JoinTeam(Player player, string team)
        {
            Lobby lobby = LobbyManager.GetPlayerLobby(player);

            if (lobby == null)
            {
                player.Message("&cYou are not in a lobby.");
                return;
            }

            if (lobby.BlueTeam.Players.Contains(player) && team.CaselessEq("blue"))
            {
                player.Message("&cYou are already on this team.");
                return;
            }

            if (lobby.RedTeam.Players.Contains(player) && team.CaselessEq("red"))
            {
                player.Message("&cYou are already on this team.");
                return;
            }

            if (team.CaselessEq("blue"))
            {
                if (lobby.BlueTeam.Players.Contains(player))
                {
                    player.Message("&cYou are already on this team.");
                    return;
                }

                if (lobby.RedTeam.Players.Contains(player))
                {
                    lobby.RedTeam.Players.Remove(player);
                }

                lobby.BlueTeam.Players.Add(player);
                lobby.MessagePlayers($"&b{player.truename} &Sjoined the &9blue &Steam.");
            }

            else if (team.CaselessEq("red"))
            {
                if (lobby.RedTeam.Players.Contains(player))
                {
                    player.Message("&cYou are already on this team.");
                    return;
                }

                if (lobby.BlueTeam.Players.Contains(player))
                {
                    lobby.BlueTeam.Players.Remove(player);
                }

                lobby.RedTeam.Players.Add(player);
                lobby.MessagePlayers($"&b{player.truename} &Sjoined the &cred &Steam.");
            }

            else
            {
                if (lobby.BlueTeam.Players.Contains(player))
                {
                    lobby.BlueTeam.Players.Remove(player);
                }

                if (lobby.RedTeam.Players.Contains(player))
                {
                    lobby.RedTeam.Players.Remove(player);
                }

                lobby.MessagePlayers($"&b{player.truename} &Sjoined the &7spectator &Steam.");
            }

            lobby.RespawnPlayer(player); // Spawn the player at their new team's spawn point.
        }

        public override void Help(Player player)
        {
            player.Message("%T/Team red/blue/spectator %H- Joins a team.");
        }
    }
}