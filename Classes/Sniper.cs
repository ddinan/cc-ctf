using System;

namespace CTF.Classes
{
    public class Sniper : PlayerClass
    {
        public Sniper() : base("Sniper", ClassType.Defense, true) { }

        public override void UseAbility()
        {
            Console.WriteLine("Shooting sniper rifle! Delay of 5 seconds before next shot.");
        }
    }
}
