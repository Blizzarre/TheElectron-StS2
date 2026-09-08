using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class Spaghettify : ElectronCard
{
    public Spaghettify() : base(5, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(50, 10);
        WithKeyword(ElectronKeywords.Drain);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: "heavy_attack.mp3")
            .WithHitVfxSpawnedAtBase()
            .Execute(choiceContext);
    }
}