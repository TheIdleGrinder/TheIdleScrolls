namespace TheIdleScrolls_Web.Definitions
{
    public static class GameVersion
    {
        const uint Major = 0;
        const uint Minor = 8;
        const uint Patch = 2;

        public static string GetCurrentVersionString()
        {
            return GetVersionString(Major, Minor, Patch);
        }

        public static string GetVersionString(uint major, uint minor, uint patch)
        {
            return $"v{major}.{minor}.{patch}";
        }

        public static bool IsAtleast(uint major, uint minor, uint patch)
        {
            if (Major > major)
            {
                return true;
            }
            else if (Major == major)
            {
                if (Minor > minor)
                {
                    return true;
                }
                else if (Minor == minor)
                {
                    if (Patch >= patch)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
