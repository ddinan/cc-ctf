using MCGalaxy;

namespace CTF
{
    public sealed class CmdLobby : Command2
    {
        public override string name { get { return "Lobby"; } }
        public override string shortcut { get { return "game"; } }
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
                case "list":
                    HandleList(player);
                    break;
                case "join":
                    HandleJoin(player, args);
                    break;
                case "create":
                    HandleCreate(player);
                    break;
                default:
                    Help(player);
                    break;
            }
        }

        private void HandleList(Player player)
        {
            player.Message("Active Lobbies:");

            foreach (var lobby in LobbyManager.lobbies)
            {
                player.Message($"Lobby ID: {lobby.LobbyId}, Players: {lobby.Players.Count}, Game Running: {lobby.IsGameRunning}");
            }
        }

        private void HandleJoin(Player player, string[] args)
        {
            if (args.Length < 2) {
                player.Message("Please specify the lobby to join with /lobby join [lobby id]");
                return;
            }

            if (!int.TryParse(args[1], out int id))
            {
                player.Message("Invalid lobby ID. Use /lobby list to see a list.");
                return;
            }

            LobbyManager.JoinLobby(player, id);
        }

        private void HandleCreate(Player player)
        {
            LobbyManager.CreateNewLobby(player);
        }

        public override void Help(Player player)
        {
            player.Message("%T/Lobby list %H- Lists all available game lobbies.");
            player.Message("%T/Lobby join [lobby] %H- Joins an existing game lobby, if permitted.");
            player.Message("%T/Lobby create %H- Creates a new game lobby.");
        }
    }
}