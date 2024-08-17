using CTF.Items;
using MCGalaxy;

namespace CTF.Classes
{
    public class Sniper : PlayerClass
    {
        public Sniper() : base("Sniper", ClassType.Defense, true, 20) { }

        public override void UseAbility(Player player)
        {
            SetCooldown();

            player.Message("Shooting sniper rifle! Delay of 20 seconds before next shot.");
            Rocket rocket = new Rocket();
            rocket.LaunchRocket(player);
        }
    }
}
