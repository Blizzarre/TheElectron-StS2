using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class Partition : ElectronEmptyCard
{
    private const string EnergyDecreaseKey = "EnergyDecrease";

    private decimal EnergyDecrement
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public Partition() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Farad);
        WithVar(new FaradVar(2));
        WithEnergy(3, 2);
        WithTip(CardKeyword.Exhaust);
        WithVar(new EnergyVar(EnergyDecreaseKey, 1));
    }

    protected override async Task BeforeOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.IntValue, this, play);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CardCmd.Exhaust(choiceContext, this);
    }

    protected override Task AfterOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (HasPaidEnergyCost)
        {
            var decrement = DynamicVars[EnergyDecreaseKey].BaseValue;
            DynamicVars.Energy.BaseValue = Math.Max(0, DynamicVars.Energy.BaseValue - decrement);
            EnergyDecrement -= decrement;
        }

        return Task.CompletedTask;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Energy.BaseValue = Math.Max(0, DynamicVars.Energy.BaseValue - EnergyDecrement);
    }
}