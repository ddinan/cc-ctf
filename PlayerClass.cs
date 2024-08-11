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

        protected PlayerClass(string name, ClassType classType, bool canSprint)
        {
            Name = name;
            ClassType = classType;
            CanSprint = canSprint;
        }

        public abstract void UseAbility();
    }
}
