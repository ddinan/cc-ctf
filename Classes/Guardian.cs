using System;

namespace CTF.Classes
{
    public class Guardian : PlayerClass
    {
        public bool CanRevive { get; private set; } = true;

        public Guardian() : base("Guardian", ClassType.Support, true, 120) { }

        public override void UseAbility()
        {
            if (CanRevive)
            {
                Console.WriteLine("Reviving player!");
                CanRevive = false;
            }
            else
            {
                Console.WriteLine("No revives left!");
            }
        }

        public void ResetRevive()
        {
            CanRevive = true;
        }
    }
}
