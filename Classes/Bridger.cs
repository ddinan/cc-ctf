using System;

namespace CTF.Classes
{
    public class Bridger : PlayerClass
    {
        public Bridger() : base("Bridger", ClassType.Hybrid, true, 60) { }

        public override void UseAbility()
        {
            Console.WriteLine("Building a line bridge!");
        }
    }
}
