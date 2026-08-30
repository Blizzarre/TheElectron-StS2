using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode;

//You're recommended but not required to keep all your code in this package and all your assets in the TheElectron folder.
[ModInitializer(nameof(Initialize))]
public partial class TheElectronMod : Node
{
    public const string ModId = "TheElectron"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();

        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        
        ElectronSubscriber.Subscribe();

        Harmony harmony = new(ModId);

        harmony.PatchAll(assembly);
    }
}