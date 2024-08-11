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
        public bool HasTNT { get; private set; } = true;
        public int PowerUpCooldown { get; private set; }
        public string PowerUpOpacity => PowerUpCooldown > 0 ? "&8" : "&f";
        public string PowerUpGuiFormat => $"&7[ {PowerUpOpacity}x &7] {PowerUpCooldown}s";

        protected PlayerClass(string name, ClassType classType, bool canSprint, int powerUpCooldown)
        {
            Name = name;
            ClassType = classType;
            CanSprint = canSprint;
            PowerUpCooldown = powerUpCooldown;
        }

        public abstract void UseAbility();
    }
}
