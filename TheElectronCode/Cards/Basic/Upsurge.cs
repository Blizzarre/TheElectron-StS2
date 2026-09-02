using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Basic;

public class Upsurge : ElectronCard
{
    public Upsurge() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithDamage(6, 4);
        WithVar(new QuarkCountVar(2));
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<UpQuark>();
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        for (var i = 0; i < DynamicVars.QuarkCount().IntValue; i++)
        {
            await QuarkCmd.Produce<UpQuark>(choiceContext, Owner, this, play);
        }
    }
}