using MCGalaxy;
using System.Collections.Generic;

namespace CTF
{
    public class Gui
    {
        public void SendSmallAnnouncement(List<Player> players, string message)
        {
            foreach (Player p in players)
            {
                p.SendCpeMessage(CpeMessageType.SmallAnnouncement, message);
            }
        }

        public void SendAnnouncement(List<Player> players, string message)
        {
            foreach (Player p in players)
            {
                p.SendCpeMessage(CpeMessageType.Announcement, message);
            }
        }

        public void SendBigAnnouncement(List<Player> players, string message)
        {
            foreach (Player p in players)
            {
                p.SendCpeMessage(CpeMessageType.BigAnnouncement, message);
            }
        }

        public void UpdateTopRight(List<Player> players, string messageTop, string messageMiddle, string messageBottom)
        {
            foreach (Player p in players)
            {
                if (messageTop != null) p.SendCpeMessage(CpeMessageType.Status1, messageTop);
                if (messageMiddle != null) p.SendCpeMessage(CpeMessageType.Status2, messageMiddle);
                if (messageBottom != null) p.SendCpeMessage(CpeMessageType.Status3, messageBottom);
            }
        }

        public void UpdateBottomRight(List<Player> players, string messageTop, string messageMiddle, string messageBottom)
        {
            foreach (Player p in players)
            {
                if (messageTop != null) p.SendCpeMessage(CpeMessageType.BottomRight3, messageTop);
                if (messageMiddle != null) p.SendCpeMessage(CpeMessageType.BottomRight2, messageMiddle);
                if (messageBottom != null) p.SendCpeMessage(CpeMessageType.BottomRight1, messageBottom);
            }
        }
    }
}
