#region

using Godot;
using MegaCrit.Sts2.Core.Helpers;

#endregion

namespace TheElectron.TheElectronCode.Utils;

public static class BaseSceneIndex
{
    public static PackedScene SelectionReticleScene => SceneHelper.Load("ui/selection_reticle");
}