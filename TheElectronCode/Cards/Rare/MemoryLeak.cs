using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class MemoryLeak : ElectronCard
{
    public MemoryLeak() : base(0, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCards(4, 1);
        WithVar(new FaradVar(6).WithUpgrade(2));
        WithVar("Amount", 1);
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<StrangeQuark>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue, this, play);
        
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

        await PowerCmd.Apply<MemoryLeakPower>(choiceContext, Owner.Creature,
            DynamicVars["Amount"].BaseValue, Owner.Creature, this);
    }
}