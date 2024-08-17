using CTF.Items;
using MCGalaxy;

namespace CTF.Classes
{
    public class Grenadier : PlayerClass
    {
        public Grenadier() : base("Grenadier", ClassType.Support, true, 90) { }

        public override void UseAbility(Player player)
        {
            if (PowerUpCooldown == 0)
            {
                player.Message("Throwing grenade!");
                Grenade grenade = new Grenade();

                grenade.ThrowGrenade(player);
                SetCooldown();
            }
            else
            {
                player.Message("No grenades left!");
            }
        }
    }
}
