using System;

namespace CTF.Classes
{
    public class Shield : PlayerClass
    {
        public int ShieldDurability { get; private set; } = 5;

        public Shield() : base("Shield", ClassType.Hybrid, true, 150) { }

        public override void UseAbility()
        {
            if (ShieldDurability > 0)
            {
                Console.WriteLine("Using shield!");
                ShieldDurability--;
            }
            else
            {
                Console.WriteLine("Shield is broken!");
            }
        }

        public void ResetShield()
        {
            ShieldDurability = 5;
        }
    }
}
