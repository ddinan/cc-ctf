using MCGalaxy;

namespace CTF.Classes
{
    public class Mechanic : PlayerClass
    {
        public int Mines { get; private set; } = 2;

        public Mechanic() : base("Mechanic", ClassType.Defense, true, 15) { }

        public override void UseAbility(Player player)
        {
            PowerUpCooldown = MaxPowerUpCooldown;

            player.Message("Placing mine!");
            Mines--;
        }

        public void ResetMines()
        {
            Mines = 2;
        }
    }
}
