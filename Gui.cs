using MCGalaxy;
using System.Collections.Generic;

namespace CTF
{
    public class Gui
    {
        public void SendSmallAnnouncement(List<Player> players, string message)
        {
            foreach (Player player in players)
            {
                player.SendCpeMessage(CpeMessageType.SmallAnnouncement, message);
            }
        }

        public void SendAnnouncement(List<Player> players, string message)
        {
            foreach (Player player in players)
            {
                player.SendCpeMessage(CpeMessageType.Announcement, message);
            }
        }

        public void SendBigAnnouncement(List<Player> players, string message)
        {
            foreach (Player player in players)
            {
                player.SendCpeMessage(CpeMessageType.BigAnnouncement, message);
            }
        }

        public void UpdateTopRight(List<Player> players, string messageTop, string messageMiddle, string messageBottom)
        {
            foreach (Player player in players)
            {
                if (messageTop != null) player.SendCpeMessage(CpeMessageType.Status1, messageTop);
                if (messageMiddle != null) player.SendCpeMessage(CpeMessageType.Status2, messageMiddle);
                if (messageBottom != null) player.SendCpeMessage(CpeMessageType.Status3, messageBottom);
            }
        }

        public void UpdateBottomRight(List<Player> players, string messageTop, string messageMiddle, string messageBottom)
        {
            foreach (Player player in players)
            {
                if (messageTop != null) player.SendCpeMessage(CpeMessageType.BottomRight3, messageTop);
                if (messageMiddle != null) player.SendCpeMessage(CpeMessageType.BottomRight2, messageMiddle);
                if (messageBottom != null) player.SendCpeMessage(CpeMessageType.BottomRight1, messageBottom);
            }
        }

        public static void SendGuiToPlayers(List<Player> players)
        {
            foreach (Player player in players)
            {
            }
        }
    }
}
