using System;

namespace CTF.Classes
{
    public class Mechanic : PlayerClass
    {
        public int Mines { get; private set; } = 2;

        public Mechanic() : base("Mechanic", ClassType.Defense, true) { }

        public override void UseAbility()
        {
            if (Mines > 0)
            {
                Console.WriteLine("Placing mine!");
                Mines--;
            }
            else
            {
                Console.WriteLine("No mines left!");
            }
        }

        public void ResetMines()
        {
            Mines = 2;
        }
    }
}
