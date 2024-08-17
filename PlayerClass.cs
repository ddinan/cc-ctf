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
        public int MaxPowerUpCooldown { get; private set; }
        private int _powerUpCooldown;

        public int PowerUpCooldown
        {
            get => _powerUpCooldown;
            private set => _powerUpCooldown = value;
        }

        public string PowerUpOpacity => PowerUpCooldown > 0 ? "&8" : "&f";
        public string PowerUpGuiFormat => $"&7[ {PowerUpOpacity}x &7] {PowerUpCooldown}s".Replace(" 0s", "");

        protected PlayerClass(string name, ClassType classType, bool canSprint, int powerUpCooldown)
        {
            Name = name;
            ClassType = classType;
            CanSprint = canSprint;
            MaxPowerUpCooldown = powerUpCooldown;
            PowerUpCooldown = 0;
        }

        protected void SetCooldown()
        {
            PowerUpCooldown = MaxPowerUpCooldown;
        }

        public void DecrementCooldown()
        {
            if (PowerUpCooldown > 0)
            {
                PowerUpCooldown--;
            }
        }

        public abstract void UseAbility(Player player);
    }

}
