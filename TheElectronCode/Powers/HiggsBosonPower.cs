using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Powers;

public class HiggsBosonPower : TheElectronPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DisplayVar<HiggsBosonPower>("TotalAmount", static p =>
        {
            var quarkCount = p.Owner.Player?.PlayerCombatState?.GetQuarkQueue()?.Quarks.Count ?? 0;
            return (p.Amount * quarkCount).ToString("0");
        })
    ];


    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource != null)
        {
            if (cardSource.Owner.Creature != Owner)
                return 0M;
        }
        else if (Owner != target)
            return 0M;

        var quarkCount = Owner.Player?.PlayerCombatState?.GetQuarkQueue()?.Quarks.Count ?? 0;
        
        return !props.IsPoweredCardOrMonsterMoveBlock() ? 0M : Amount * quarkCount;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Owner != dealer || !props.IsPoweredAttack())
            return 0M;

        var quarkCount = Owner.Player?.PlayerCombatState?.GetQuarkQueue()?.Quarks.Count ?? 0;

        return Amount * quarkCount;
    }
}