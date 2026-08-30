using System.Collections.Immutable;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Field;
using TheElectron.TheElectronCode.Models;
using TheElectron.TheElectronCode.Nodes.Quarks;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Entities;

public class QuarkQueue
{
    public const int MaxCapacity = 10;

    public const int DefaultCapacity = 3;

    private Player Owner { get; }

    private readonly List<QuarkModel> _quarks = [];
    
    private int _capacity = 0;

    public IReadOnlyList<QuarkModel> Quarks => _quarks;

    public int Capacity
    {
        get => Math.Min(MaxCapacity, _capacity + TempCapacity);
        private set => _capacity = value;
    }

    private int TempCapacity { get; set; }

    public QuarkQueue(Player owner)
    {
        Owner = owner;
    }

    public void Clear()
    {
        _quarks.Clear();
        Capacity = 0;
        TempCapacity = 0;
    }
    
    public void RemoveCapacity(int capacity)
    {
        _capacity = Math.Max(0, _capacity - capacity);
        while (Quarks.Count > Capacity)
        {
            var lastQuark = _quarks.Last();
            if (lastQuark.IsStable)
            {
                TempCapacity--;
            }
            Remove(lastQuark);
        }

        RestoreTempSlotOwnerships();
    }
    
    // Set Stable Quarks' HasStableSlot bool to reflect the state of current temp slots available.
    private void RestoreTempSlotOwnerships()
    {
        var stableSlots = Quarks.Count(q => q is { IsStable: true, HasStableSlot: true });
        var availableTempSlots = Math.Min(MaxCapacity - _capacity, TempCapacity);


        var missingSlots = availableTempSlots - stableSlots;

        foreach (var quark in Quarks)
        {
            if (missingSlots <= 0) break;
            if (quark is not { IsStable: true, HasStableSlot: false }) continue;
            quark.HasStableSlot = true;
            missingSlots--;
        }
    }
    
    /// <summary>
    /// Add capacity (Slots) to the queue.
    /// </summary>
    /// <param name="capacity"></param>
    /// <param name="isTemp"></param>
    /// <returns>Number of slots that should be visually added</returns>
    public int AddCapacity(int capacity, bool isTemp = false)
    {
        var before = Capacity;
        if (isTemp)
        {
            TempCapacity += capacity;
        }
        else
        {
            _capacity = Math.Min(MaxCapacity, _capacity + capacity);
        }
        RevokeTempSlotOwnerships();
        return Capacity - before;
    }

    // Set Stable Quarks' HasStableSlot bool to reflect the state of current temp slots available.
    private void RevokeTempSlotOwnerships()
    {
        var stableSlots = Quarks.Count(q => q is { IsStable: true, HasStableSlot: true });
        var availableTempSlots = Math.Min(MaxCapacity - _capacity, TempCapacity);

        var excessSlots = stableSlots - availableTempSlots;

        foreach (var quark in Quarks)
        {
            if (excessSlots <= 0) break;
            if (quark is not { IsStable: true, HasStableSlot: true }) continue;
            quark.HasStableSlot = false;
            excessSlots--;
        }
    }

    public bool HasAny()
    {
        return _quarks.Count != 0;
    }

    public async Task<bool> TryEnqueue(QuarkModel quark)
    {
        if (Capacity == 0) return false;
        quark.AssertMutable();
        if (Quarks.Count >= Capacity) throw new InvalidOperationException("QuarkQueue is full");

        _quarks.Add(quark);
        RevokeTempSlotOwnerships();
        await SmallWait();
        return true;
    }

    /// <summary>
    /// Fuse the Quarks (apply all Quark effects) and remove Quarks. Perform animation by calling NQuarkManager. 
    /// </summary>
    /// <param name="choiceContext"></param>
    public async Task FuseQuarks(PlayerChoiceContext choiceContext)
    {
        var stats = GetStats();
        var nCreature = NCombatRoom.Instance?.GetCreatureNode(Owner.Creature);
        NQuarkManager? quarkManager = null;
        if (nCreature != null)
        {
            quarkManager = ElectronNode.NQuarkManager[nCreature];
        }
        
        // Begin Fusing animation
        if (quarkManager != null)
            await quarkManager.BeginFuseQuarksAnim(stats.Keys.ToHashSet());
        
        if (stats.TryGetValue(QuarkModel.FuseStat.Damage, out var damage))
        {
            // Consume display anim
            if (quarkManager != null)
                await quarkManager.StepFuseQuarksAnim(QuarkModel.FuseStat.Damage);
            
            if (damage > 0)
            {
                var targets = Owner.Creature.CombatState?.GetOpponentsOf(Owner.Creature)
                    .Where(e => e.IsHittable)
                    .ToList() ?? [];

                var maxQuantumLinkStack = targets.Max(e => e.GetPowerAmount<QuantumLinkPower>());
                var priorityTargets = targets.Where(e => e.GetPowerAmount<QuantumLinkPower>() == maxQuantumLinkStack);

                // TODO consider implementing aoe attack here.

                var target = Owner.RunState.Rng.CombatTargets.NextItem(priorityTargets);
                if (target != null)
                {
                    VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_attack_blunt");
                    await CreatureCmd.Damage(choiceContext, target, damage, ValueProp.Unpowered, Owner.Creature);
                }
            }
            await MediumWait();
        }

        if (stats.TryGetValue(QuarkModel.FuseStat.Block, out var block))
        {
            if (quarkManager != null)
                await quarkManager.StepFuseQuarksAnim(QuarkModel.FuseStat.Block);
            if (block > 0)
            {
                await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Unpowered, null);
            }
            await MediumWait();
        }
        
        if (stats.TryGetValue(QuarkModel.FuseStat.Draw, out var draw))
        {
            if (quarkManager != null)
                await quarkManager.StepFuseQuarksAnim(QuarkModel.FuseStat.Draw);
            if(draw > 0)
            {
                await CardPileCmd.Draw(choiceContext, draw, Owner);
            }
            await MediumWait();
        }

        if (stats.TryGetValue(QuarkModel.FuseStat.Energy, out var energy))
        {
            if (quarkManager != null)
                await quarkManager.StepFuseQuarksAnim(QuarkModel.FuseStat.Energy);
            if(energy > 0)
            {
                await PlayerCmd.GainEnergy(energy, Owner);
            }
            await MediumWait();
        }
        
        _quarks.Clear();
        TempCapacity = 0;

        quarkManager?.EndFuseQuarksAnim();
    }

    public bool Remove(QuarkModel quark)
    {
        return _quarks.Remove(quark);
    }

    public void Insert(int idx, QuarkModel quark)
    {
        if (idx > Capacity) throw new InvalidOperationException("idx cannot be greater than capacity");

        _quarks.Insert(idx, quark);
    }

    public bool IsFull()
    {
        return Quarks.Count >= Capacity;
    }

    public bool IsSlotsFull()
    {
        return Capacity >= MaxCapacity;
    }
    
    private Dictionary<QuarkModel.FuseStat, decimal> GetStats()
    {
        var ret = new Dictionary<QuarkModel.FuseStat, decimal>();
        foreach (var quark in _quarks.Where(quark => quark.Stat != QuarkModel.FuseStat.None))
        {
            ret[quark.Stat] = ret.GetValueOrDefault(quark.Stat, 0) + quark.Value;
        }

        return ret;
    }
    
    private async Task SmallWait()
    {
        if (LocalContext.IsMe(Owner))
            await Cmd.CustomScaledWait(0.1f, 0.25f);
        else
            await Cmd.Wait(0.05f);
    }
    
    private async Task MediumWait()
    {
        if (LocalContext.IsMe(Owner))
            await Cmd.CustomScaledWait(0.2f, 0.4f);
        else
            await Cmd.Wait(0.05f);
    }

}