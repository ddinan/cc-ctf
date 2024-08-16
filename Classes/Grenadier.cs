using CTF.Items;
using MCGalaxy;

namespace CTF.Classes
{
    public class Grenadier : PlayerClass
    {
        public bool HasGrenade { get; private set; } = true;

        public Grenadier() : base("Grenadier", ClassType.Support, true, 90) { }

        public override void UseAbility(Player player)
        {
            PowerUpCooldown = MaxPowerUpCooldown;

            if (HasGrenade)
            {
                player.Message("Throwing grenade!");
                Grenade grenade = new Grenade();
                grenade.ThrowGrenade(player, (ushort)Orientation.PackedToDegrees(player.Rot.RotY), (ushort)Orientation.PackedToDegrees(player.Rot.HeadX));
                HasGrenade = false;
            }
            else
            {
                player.Message("No grenades left!");
            }
        }

        public void ResetGrenade()
        {
            HasGrenade = true;
        }
    }
}
