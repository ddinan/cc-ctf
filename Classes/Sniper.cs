using System;

namespace CTF.Classes
{
    public class Sniper : PlayerClass
    {
        public Sniper() : base("Sniper", ClassType.Defense, true, 20) { }

        public override void UseAbility()
        {
            Console.WriteLine("Shooting sniper rifle! Delay of 20 seconds before next shot.");
        }
    }
}
