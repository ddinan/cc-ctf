using CTF.Items;
using MCGalaxy;

namespace CTF.Classes
{
    public class Bridger : PlayerClass
    {
        public Bridger() : base("Bridger", ClassType.Hybrid, true, 60) { }

        public override void UseAbility(Player player)
        {
            PowerUpCooldown = MaxPowerUpCooldown;

            Bridge.BuildBridge(player);
            player.Message("Building a line bridge!");
        }
    }
}
