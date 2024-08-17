using CTF.Items;
using MCGalaxy;

namespace CTF.Classes
{
    public class Pyromaniac : PlayerClass
    {
        public int Fuel { get; private set; } = 100;

        public Pyromaniac() : base("Pyromaniac", ClassType.Offense, true, 120) { }

        public override void UseAbility(Player player)
        {
            PowerUpCooldown = MaxPowerUpCooldown;

            if (player.Extras.GetBoolean("CTF_FLAMETHROWER_ACTIVATED"))
            {
                player.Message("flamethrower off");
                player.Extras["CTF_FLAMETHROWER_ACTIVATED"] = false;
                return;
            }

            else
            {
                player.Message("flamethrower on");
                Flamethrower flamethrower = new Flamethrower();
                flamethrower.ActivateFlamethrower(player);
                return;
            }
        }

        public void ResetFuel()
        {
            Fuel = 100;
        }
    }
}
