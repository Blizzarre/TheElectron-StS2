using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Character;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Nodes.Quarks;

namespace TheElectron.TheElectronCode.Models;

public abstract class QuarkModel : AbstractModel, ICustomModel
{
    public const string LocTable = "quarks";

    public enum FuseStat
    {
        None,
        Damage,
        Block,
        Draw,
        Energy
    }
    
    public virtual decimal Value { get; set; }
    
    public virtual bool IsStable { get; set; }
    
    public virtual bool HasStableSlot { get; set; }

    public virtual bool ShowLabel => false;

    public virtual FuseStat Stat => FuseStat.None; 
    
    public LocString Title => new(LocTable, Id.Entry + ".title");

    public LocString Description => new(LocTable, Id.Entry + ".description");
    
    public static HoverTip EmptySlotHoverTip => new(new LocString(LocTable, "THEELECTRON-EMPTY_SLOT.title"),
        new LocString(LocTable, "THEELECTRON-EMPTY_SLOT.description"));
    
    private string IconPath => Id.Entry.RemovePrefix().ToLowerInvariant().QuarkImagePath();
    
    private string SpritePath => Id.Entry.RemovePrefix().ToLowerInvariant().QuarkScenePath();
    
    public CompressedTexture2D Icon => PreloadManager.Cache.GetCompressedTexture2D(IconPath);
    
    public virtual Color DarkenedColor => new("a0a0a0");
    
    public bool HasBeenRemovedFromState { get; private set; }
    
    private string SmartDescriptionLocKey => Id.Entry + ".smartDescription";
    
    public bool HasSmartDescription => LocString.Exists(LocTable, SmartDescriptionLocKey);
    
    public LocString SmartDescription =>
        !HasSmartDescription ? Description : new LocString(LocTable, Id.Entry + ".smartDescription");
    
    public HoverTip DumbHoverTip => ElectronHoverTipFactory.CreateQuarkHoverTip(this, Description);
    
    
    protected virtual IEnumerable<IHoverTip> ExtraHoverTips => [];

    public IEnumerable<IHoverTip> HoverTips
    {
        get
        {
            var list = new List<IHoverTip>();
            if (HasSmartDescription && IsMutable)
            {
                var smartDescription = SmartDescription;
                var prefix = GetQuarkOwnerPool().EnergyColorName;
                smartDescription.Add("energyPrefix", prefix);
                smartDescription.Add(new EnergyVar((int)Value){ColorPrefix = prefix});
                smartDescription.Add("Value", Value);
                list.Add(ElectronHoverTipFactory.CreateQuarkHoverTip(this, smartDescription));
                if (IsStable)
                    list.Add(ElectronHoverTipFactory.Static(ElectronHoverTip.StableQuark,
                        ls => ls.Add("HasStableSlot", HasStableSlot)));
            }
            else
            {
                list.Add(DumbHoverTip);
            }
            list.AddRange(ExtraHoverTips);

            return list;
        }
    }
    
    private IPoolModel GetQuarkOwnerPool()
    {
        return IsMutable ? Owner.Character.CardPool : ModelDb.CardPool<TheElectronCardPool>();
    }
    
    private QuarkModel? CanonicalInstance
    {
        get => !IsMutable ? this : field;
        set
        {
            AssertMutable();
            field = value;
        }
    }
    
    private Player? _owner;

    public Player Owner
    {
        get
        {
            AssertMutable();
            return _owner ?? throw new Exception("Quark " + Id.Entry + " does not have an owner.");
        }
        set
        {
            AssertMutable();
            if (_owner != null && value != null && value != _owner)
                throw new InvalidOperationException("Quark " + Id.Entry + " already has an owner.");

            _owner = value;
        }
    }
    
    public override bool ShouldReceiveCombatHooks => true;
    
    public NQuarkVisuals CreateSprite()
    {
        var quarkVisuals = PreloadManager.Cache.GetScene(SpritePath).Instantiate<NQuarkVisuals>();
        return quarkVisuals;
    }
    
    public QuarkModel ToMutable()
    {
        AssertCanonical();
        var quarkModel = (QuarkModel)MutableClone();
        quarkModel.CanonicalInstance = this;
        return quarkModel;
    }
    
    public QuarkModel CreateClone()
    {
        AssertMutable();
        var clonedQuark = (QuarkModel)ClonePreservingMutability();
        return clonedQuark;
    }
    
    // Modify value of quarks (Spin/Strange/Charm)
    protected decimal ModifyQuarkValue(decimal amount)
    {
        if (Owner.Creature.CombatState == null) return amount;
        return ElectronHook.ModifyQuarkValue(Owner.Creature.CombatState, this, amount);
    }
    
    public void RemoveInternal()
    {
        HasBeenRemovedFromState = true;
    }
}