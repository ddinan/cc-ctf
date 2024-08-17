using MCGalaxy;

namespace CTF
{
    public enum ClassType
    {
        Defense,
        Offense,
        Support,
        Hybrid
    }

    public abstract class PlayerClass
    {
        public string Name { get; private set; }
        public ClassType ClassType { get; private set; }
        public bool CanSprint { get; private set; }
        public bool HasTNT = true;
        public static int MaxPowerUpCooldown = 0;
        public int PowerUpCooldown = MaxPowerUpCooldown;
        public string PowerUpOpacity => PowerUpCooldown > 0 ? "&8" : "&f";
        public string PowerUpGuiFormat => $"&7[ {PowerUpOpacity}x &7] {PowerUpCooldown}s".Replace(" 0s", "");

        protected PlayerClass(string name, ClassType classType, bool canSprint, int powerUpCooldown)
        {
            Name = name;
            ClassType = classType;
            CanSprint = canSprint;
            PowerUpCooldown = 0;
            MaxPowerUpCooldown = powerUpCooldown;
        }

        public abstract void UseAbility(Player player);
    }
}
