using MCGalaxy;
using System.Collections.Generic;

namespace CTF
{
    public class Team
    {
        public string Name { get; private set; }
        public List<Player> Players { get; private set; }
        public int Points { get; private set; }
        public int Captures { get; private set; }

        public Team(string name)
        {
            Name = name;
            Players = new List<Player>();
            Points = 0;
            Captures = 0;
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

        public void SetPoints(int points)
        {
            Points = points;
        }

        public void SetCaptures(int captures)
        {
            Captures = captures;
        }
    }
}
