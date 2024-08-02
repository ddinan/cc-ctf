using System;

namespace CTF.Classes
{
    public class Grenadier : PlayerClass
    {
        public bool HasGrenade { get; private set; } = true;

        public Grenadier() : base("Grenadier", ClassType.Support, true) { }

        public override void UseAbility()
        {
            if (HasGrenade)
            {
                Console.WriteLine("Throwing grenade!");
                HasGrenade = false;
            }
            else
            {
                Console.WriteLine("No grenades left!");
            }
        }

        public void ResetGrenade()
        {
            HasGrenade = true;
        }
    }
}
