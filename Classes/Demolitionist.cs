using MCGalaxy;

namespace CTF.Classes
{
    public class Demolitionist : PlayerClass
    {
        public int TNTRadius { get; private set; } = 3;

        public Demolitionist() : base("Demolitionist", ClassType.Offense, false, 150) { }

        public override void UseAbility(Player player)
        {
            PowerUpCooldown = MaxPowerUpCooldown;

            player.Message($"Placing TNT with {TNTRadius}-block radius!");
        }
    }
}
