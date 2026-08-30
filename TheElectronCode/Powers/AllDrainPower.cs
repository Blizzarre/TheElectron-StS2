using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Models;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Powers;

public class AllDrainPower : TheElectronPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        if (card.Owner.Creature != Owner || card.EnergyCost.GetWithModifiers(CostModifiers.All) < 1) return false;

        return keywords.Add(ElectronKeywords.Drain);
    }
}