using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class WaveDash : ElectronCard
{
    private const string FaradCostKey = "FaradCost";

    public WaveDash() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(12, 3);
        WithBlock(12, 3);
        WithVar(FaradCostKey, 3);
    }

    private bool HasEnoughFarad => (Owner.PlayerCombatState?.GetFarad() ?? 0) >= DynamicVars[FaradCostKey].IntValue;

    protected override bool ShouldGlowRedInternal => !HasEnoughFarad;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (HasEnoughFarad)
        {
            await ElectronPlayerCmd.LoseFarad(choiceContext, Owner, DynamicVars[FaradCostKey].BaseValue, this, play);

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
                .TargetingAllOpponents(CombatState!)
                .WithHitFx("vfx/vfx_flying_slash")
                .Execute(choiceContext);

            await CommonActions.CardBlock(this, play);
        }
    }
}