using MCGalaxy;
using System.Collections.Generic;

namespace CTF
{
    public static class PlayerClassManager
    {
        private static Dictionary<string, PlayerClass> playerClasses = new Dictionary<string, PlayerClass>(); // Dictionary to map players to their classes.

        public static void SetPlayerClass(string player, PlayerClass playerClass)
        {
            playerClasses[player] = playerClass;
        }

        public static PlayerClass GetPlayerClass(string player)
        {
            return playerClasses.TryGetValue(player, out var playerClass) ? playerClass : null;
        }

        public static bool HasActiveClass(string player)
        {
            return playerClasses.ContainsKey(player);
        }
    }
}
