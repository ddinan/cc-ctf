using MCGalaxy;
using System.Collections.Generic;

namespace CTF
{
    public class Team
    {
        public string Name { get; private set; }
        public List<Player> Players { get; private set; }

        public Team(string name)
        {
            Name = name;
            Players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
        }

        public bool ContainsPlayer(Player player)
        {
            return Players.Contains(player);
        }

        public void SetFlagPosition(string map) {
        
        }
    }
}
