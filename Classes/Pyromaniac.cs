using System;

namespace CTF.Classes
{
    public class Pyromaniac : PlayerClass
    {
        public int Fuel { get; private set; } = 100;

        public Pyromaniac() : base("Pyromaniac", ClassType.Offense, true, 120) { }

        public override void UseAbility()
        {
            if (Fuel > 0)
            {
                Console.WriteLine("Using flamethrower!");
                Fuel -= 10; // Example fuel consumption
            }
            else
            {
                Console.WriteLine("Out of fuel!");
            }
        }

        public void ResetFuel()
        {
            Fuel = 100;
        }
    }
}
