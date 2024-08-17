using MCGalaxy;

namespace CTF.Classes
{
    public class Shield : PlayerClass
    {
        public int ShieldDurability { get; private set; } = 5;

        public Shield() : base("Shield", ClassType.Hybrid, true, 150) { }

        public override void UseAbility(Player player)
        {
            SetCooldown();

            if (ShieldDurability > 0)
            {
                player.Message("Using shield!");
                ShieldDurability--;
            }
            else
            {
                player.Message("Shield is broken!");
            }
        }

        public void ResetShield()
        {
            ShieldDurability = 5;
        }
    }
}
