using BaseLib.Utils;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class EnergyShield : ElectronEmptyCard
{
    public EnergyShield() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 2);
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<TopQuark>();
        WithVar(new QuarkCountVar(1));
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var amount = await CommonActions.CardBlock(this, play);
        await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<TopQuark>(choiceContext, Owner, this, play);
    }
}