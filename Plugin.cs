using BepInEx;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace IsamClient
{
    [BepInPlugin("com.isam.isam-client", "isam-client", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            Log.LogInfo("isam-client v1.0.0 loaded");

            ClassInjector.RegisterTypeInIl2Cpp<Patches.LogoSwapper>();
            ClassInjector.RegisterTypeInIl2Cpp<Patches.WatermarkRenderer>();

            var go = new GameObject("IsamClient");
            Object.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            go.AddComponent<Patches.LogoSwapper>();
            go.AddComponent<Patches.WatermarkRenderer>();

            Log.LogInfo("isam-client components registered");
        }

        public override bool Unload()
        {
            Log.LogInfo("isam-client unloaded");
            return true;
        }
    }
}
