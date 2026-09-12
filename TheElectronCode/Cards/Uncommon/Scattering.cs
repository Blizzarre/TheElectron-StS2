using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Scattering : ElectronEmptyCard
{
    private const string IncreaseKey = "Increase";

    private decimal ExtraQuantumLink
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    private bool ShouldReturnNextTurn { get; set; }

    public Scattering() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<QuantumLinkPower>(6, 2);
        WithVar(IncreaseKey, 3, 1);
    }

    public override TargetType TargetType => HasEnoughEnergy ? TargetType.AnyEnemy : TargetType.None;

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<QuantumLinkPower>(choiceContext, play.Target,
            DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);
    }

    protected override Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var increment = DynamicVars[IncreaseKey].BaseValue;
        DynamicVars.Power<QuantumLinkPower>().BaseValue += increment;
        ExtraQuantumLink += increment;
        ShouldReturnNextTurn = true;
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player == Owner &&
            CombatManager.Instance.History.CardPlaysFinished.Any(e =>
                e.HappenedLastPlayerTurn(Owner) && e.CardPlay.Card == this && ShouldReturnNextTurn))
        {
            ShouldReturnNextTurn = false;
            if (Pile is not { Type: PileType.Hand }) await CardPileCmd.Add(this, PileType.Hand);
        }
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Power<QuantumLinkPower>().BaseValue += ExtraQuantumLink;
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        ShouldReturnNextTurn = false;
    }
}