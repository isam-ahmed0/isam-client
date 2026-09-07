using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace IsamClient
{
    [BepInPlugin("com.isam.isam-client", "isam-client", "1.0.0")]
    public class Plugin : BasePlugin
    {
        private Harmony _harmony;

        public override void Load()
        {
            Log.LogInfo("isam-client v1.0.0 loaded");

            _harmony = new Harmony("com.isam.isam-client");
            _harmony.PatchAll(typeof(Plugin).Assembly);

            Log.LogInfo("isam-client patches applied");
        }

        public override bool Unload()
        {
            _harmony?.UnpatchSelf();
            Log.LogInfo("isam-client unloaded");
            return true;
        }
    }
}
