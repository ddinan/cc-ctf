using CTF.Items;
using MCGalaxy;

namespace CTF.Classes
{
    public class Sniper : PlayerClass
    {
        public Sniper() : base("Sniper", ClassType.Defense, true, 5) { }

        public override void UseAbility(Player player)
        {
            PowerUpCooldown = MaxPowerUpCooldown;

            player.Message("Shooting sniper rifle! Delay of 5 seconds before next shot.");
            Rocket rocket = new Rocket();
            rocket.LaunchRocket(player);
        }
    }
}
