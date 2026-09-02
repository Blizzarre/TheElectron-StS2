using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.DynamicVars;

public class FaradVar : DynamicVar
{
    public const string defaultName = "Farad";

    public FaradVar(int farad)
        : this(defaultName, farad)
    {
    }

    public FaradVar(string name, int farad)
        : base(name, farad)
    {
        this.WithTooltip("THEELECTRON-FARAD");
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        var modifiedValue = BaseValue;

        if (runGlobalHooks && card.CombatState != null)
            modifiedValue =
                ElectronHook.ModifyFaradGain(card.CombatState, card.Owner, BaseValue, ValueProp.Move, card, out _);

        PreviewValue = modifiedValue;
    }
}