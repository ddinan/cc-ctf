using System;

namespace CTF.Classes
{
    public class Demolitionist : PlayerClass
    {
        public int TNTRadius { get; private set; } = 3;

        public Demolitionist() : base("Demolitionist", ClassType.Offense, false) { }

        public override void UseAbility()
        {
            Console.WriteLine($"Placing TNT with {TNTRadius}-block radius!");
        }
    }
}
