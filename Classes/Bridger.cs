using System;

namespace CTF.Classes
{
    public class Bridger : PlayerClass
    {
        public Bridger() : base("Bridger", ClassType.Hybrid, true) { }

        public override void UseAbility()
        {
            Console.WriteLine("Building a line bridge!");
        }
    }
}
