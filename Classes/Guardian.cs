using MCGalaxy;

namespace CTF.Classes
{
    public class Guardian : PlayerClass
    {
        public bool CanRevive { get; private set; } = true;

        public Guardian() : base("Guardian", ClassType.Support, true, 120) { }

        public override void UseAbility(Player player)
        {
            SetCooldown();

            if (CanRevive)
            {
                player.Message("Reviving player!");
                CanRevive = false;
            }
            else
            {
                player.Message("No revives left!");
            }
        }

        public void ResetRevive()
        {
            CanRevive = true;
        }
    }
}
