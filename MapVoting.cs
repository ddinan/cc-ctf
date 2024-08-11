using MCGalaxy;
using MCGalaxy.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CTF
{
    public class MapVoting
    {
        private Lobby parentLobby;
        private List<string> AvailableMaps;
        private Dictionary<string, int> MapVotes; // Map name to vote count.

        private bool votingActive = false;

        public MapVoting(Lobby lobby, List<string> availableMaps)
        {
            parentLobby = lobby;
            AvailableMaps = availableMaps;
            MapVotes = new Dictionary<string, int>();
        }

        public void StartMapVoting()
        {
            if (votingActive)
            {
                return;
            }

            // Select 3 random maps excluding the currently active map.
            string activeMap = parentLobby.Map.name.Replace($"lobby{parentLobby.LobbyId}_", "");
            List<string> eligibleMaps = AvailableMaps.Where(map => map != activeMap).ToList();
            List<string> randomMaps = GetRandomMapOptions(eligibleMaps, 3);

            // Initialize vote counts for the selected maps.
            foreach (var map in randomMaps)
            {
                MapVotes[map] = 0;
            }

            // Display voting options to players
            parentLobby.MessagePlayers("&SVote for the next map:");
            for (int i = 0; i < randomMaps.Count; i++)
            {
                parentLobby.MessagePlayers($"{i + 1}. {randomMaps[i]}");
            }

            Server.MainScheduler.QueueOnce(CountVotesAndSelectMap, null, TimeSpan.FromSeconds(5));
            votingActive = true;
        }

        private List<string> GetRandomMapOptions(List<string> maps, int count)
        {
            Random random = new Random();
            List<string> randomMaps = maps.OrderBy(x => random.Next()).Take(count).ToList();
            return randomMaps;
        }

        public void VoteForMap(Player player, int optionIndex)
        {
            if (!votingActive)
            {
                player.Message("&cMap voting is not active right now.");
                return;
            }

            // Validate optionIndex.
            if (optionIndex < 1 || optionIndex > MapVotes.Count)
            {
                player.Message("&cInvalid vote option.");
                return;
            }

            // Get the selected map name and increase its vote count.
            string selectedMap = MapVotes.Keys.ElementAt(optionIndex - 1);
            MapVotes[selectedMap]++;

            player.Message($"&aYou voted for {selectedMap}.");
        }

        private void CountVotesAndSelectMap(SchedulerTask task)
        {
            parentLobby.MessagePlayers("&SMap voting has ended.");

            string selectedMap = MapVotes.OrderByDescending(kv => kv.Value).First().Key; // Find the map with the highest votes.

            parentLobby.MessagePlayers($"Map {selectedMap} selected for the next game!");

            parentLobby.StartNextMap(selectedMap); // Start a new game on the new map.

            // Reset voting state.
            votingActive = false;
            MapVotes.Clear();
        }
    }
}
