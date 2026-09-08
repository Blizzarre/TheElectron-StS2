using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Utils;

public class ElectronResource
{
    public const string NQuarkManagerPath = "res://TheElectron/scenes/quarks/quark_manager.tscn";
    public const string NQuarkPath = "res://TheElectron/scenes/quarks/quark.tscn";
    public const string NFaradCounterPath = "res://TheElectron/scenes/combat/energy_counters/farad_counter.tscn";
    public const string NElectronCardIndicatorPath = "res://TheElectron/scenes/card/electron_card_indicator.tscn";

    // These assets will be loaded with PreloadManager
    public static readonly IEnumerable<string> AssetPaths =
    [
        NQuarkManagerPath, NQuarkPath, NFaradCounterPath, NElectronCardIndicatorPath,
        "empty".CardUiResourcePath().ToRes(),
        "deplete".CardUiResourcePath().ToRes(),
        "drain".CardUiResourcePath().ToRes()
    ];
}