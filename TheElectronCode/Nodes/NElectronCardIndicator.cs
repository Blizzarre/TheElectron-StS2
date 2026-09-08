using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Nodes.Cards;
using TheElectron.TheElectronCode.Cards;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Nodes;

public partial class NElectronCardIndicator: HBoxContainer
{
    private static readonly Color BlackBgColor = new(0f, 0f, 0f, 0.54f);
    private static readonly Color EmptyBgGlowColor = new(1.0f, 1.0f, 1.0f, 0.95f);
    private static readonly Color DepleteBgGlowColor = new(1.0f, 0.891f, 0.196f, 0.95f);
    private static readonly Color DrainBgGlowColor = new(0.75f, 0.21f, 1.0f, 0.95f);
    private static readonly Color IconActiveColor = new(1.5f, 1.5f, 1.5f);
    
    private NCard? NCard { get; set; }

    private TextureRect _empty = null!;
    private TextureRect _emptyBg = null!;
    private TextureRect _deplete = null!;
    private TextureRect _depleteBg = null!;
    private TextureRect _drain = null!;
    private TextureRect _drainBg = null!;

    public NElectronCardIndicator WithData(NCard card)
    {
        NCard = card;
        return this;
    }

    public override void _Ready()
    {
        _empty = GetNode<TextureRect>("%Empty");
        _emptyBg = GetNode<TextureRect>("%EmptyBg");
        _deplete = GetNode<TextureRect>("%Deplete");
        _depleteBg = GetNode<TextureRect>("%DepleteBg");
        _drain = GetNode<TextureRect>("%Drain");
        _drainBg = GetNode<TextureRect>("%DrainBg");

        _empty.Visible = false;
        _deplete.Visible = false;
        _drain.Visible = false;
    }

    public void UpdateVisuals()
    {
        if (!IsNodeReady()) return;
        var card = NCard?._model;
        if (card != null && NCard!.Visibility == ModelVisibility.Visible)
        {
            Visible = true;
            var inCombat = card.IsInCombat;
            if (card is ElectronEmptyCard emptyCard)
            {
                _empty.Visible = true;
                if (inCombat && emptyCard.WouldBeEmpty)
                {
                    _emptyBg.Modulate = EmptyBgGlowColor;
                    _empty.SelfModulate = IconActiveColor;
                }
                else
                {
                    _emptyBg.Modulate = BlackBgColor;
                    _empty.SelfModulate = Colors.White;
                }
            }
            else
            {
                _empty.Visible = false;
            }
            
            if (card is ElectronDepleteCard depleteCard)
            {
                _deplete.Visible = true;
                if (inCombat && depleteCard.WouldDeplete)
                {
                    _depleteBg.Modulate = DepleteBgGlowColor;
                    _deplete.SelfModulate = IconActiveColor;
                }
                else
                {
                    _depleteBg.Modulate = BlackBgColor;
                    _deplete.SelfModulate = Colors.White;
                }
            }
            else
            {
                _deplete.Visible = false;
            }

            if (card.Keywords.Contains(ElectronKeywords.Drain))
            {
                _drain.Visible = true;
                if (inCombat && (card.Owner.PlayerCombatState?.Energy ?? 0) <
                    card.EnergyCost.GetWithModifiers(CostModifiers.All))
                {
                    _drainBg.Modulate = DrainBgGlowColor;
                    _drain.SelfModulate = IconActiveColor;
                }
                else
                {
                    _drainBg.Modulate = BlackBgColor;
                    _drain.SelfModulate = Colors.White;
                }
            }
            else
            {
                _drain.Visible = false;
            }
            return;
        }
        Visible = false;
    }
}