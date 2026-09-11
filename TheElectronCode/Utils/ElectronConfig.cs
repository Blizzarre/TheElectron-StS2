using BaseLib.Config;

namespace TheElectron.TheElectronCode.Utils;

public class ElectronConfig : SimpleModConfig
{
    [ConfigHoverTip] public static bool UploadMetrics { get; set; } = false;

    [ConfigHideInUI]
    [ConfigIgnoreRestoreDefaults]
    public static bool UploadMetricsFtueSeen { get; set; } = false;
}