using MCGalaxy;
using MCGalaxy.Config;
using MCGalaxy.Maths;

namespace CTF
{
    public class MapConfig
    {
        [ConfigInt("divider-offset", null, 0)] public int DividerOffset;

        [ConfigVec3("blue-flag-position", null)] public Vec3U16 BlueFlagPosition;
        [ConfigVec3("blue-spawn-position", null)] public Vec3U16 BlueSpawnPosition;
        [ConfigInt("blue-spawn-pitch", null, 0)] public int BlueSpawnPitch;
        [ConfigInt("blue-spawn-yaw", null, 270)] public int BlueSpawnYaw;

        [ConfigVec3("red-flag-position", null)] public Vec3U16 RedFlagPosition;
        [ConfigVec3("red-spawn-position", null)] public Vec3U16 RedSpawnPosition;
        [ConfigInt("red-spawn-pitch", null, 0)] public int RedSpawnPitch;
        [ConfigInt("red-spawn-yaw", null, 90)] public int RedSpawnYaw;

        static string Path(string map) { return $"./maps/{map}.properties"; }
        static ConfigElement[] config;

        public void SetDefaults(Level lvl)
        {
            DividerOffset = lvl.Length / 2;

            ushort midX = (ushort)(lvl.Width / 2);
            ushort midY = (ushort)(lvl.Height / 2);
            ushort topY = (ushort)(midY + 2);
            ushort maxZ = (ushort)(lvl.Length - 1);

            BlueFlagPosition = new Vec3U16(midX, topY, maxZ);
            BlueSpawnPosition = new Vec3U16(midX, midY, maxZ);

            BlueSpawnPitch = 0;
            BlueSpawnYaw = 270;

            RedFlagPosition = new Vec3U16(midX, topY, 0);
            RedSpawnPosition = new Vec3U16(midX, midY, 0);

            RedSpawnPitch = 0;
            RedSpawnYaw = 90;
        }

        public void Load(string map)
        {
            if (config == null) config = ConfigElement.GetAll(typeof(MapConfig));
            ConfigElement.ParseFile(config, Path(map), this);
        }

        public void Save(string map)
        {
            if (config == null) config = ConfigElement.GetAll(typeof(MapConfig));
            ConfigElement.SerialiseSimple(config, Path(map), this);
        }
    }
}