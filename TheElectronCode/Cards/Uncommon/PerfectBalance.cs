using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class PerfectBalance : ElectronCard
{
    public PerfectBalance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Produce);
        WithTip(ElectronHoverTip.Stable);
        WithQuarkTip<UpQuark>();
        WithQuarkTip<DownQuark>();
        WithVar(new QuarkCountVar(1));
        WithPower<StabilityPower>(1, 2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<UpQuark>(choiceContext, Owner, this, play, true);

        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<DownQuark>(choiceContext, Owner, this, play, true);

        await PowerCmd.Apply<StabilityPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<StabilityPower>().BaseValue,
            Owner.Creature, this);
    }
}