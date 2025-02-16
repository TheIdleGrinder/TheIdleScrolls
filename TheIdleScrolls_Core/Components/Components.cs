using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Components
{
    public class LifePoolComponent : IComponent
    {
        public int Current = 1;
        public int Maximum = 1;

        public bool IsAlive => Current > 0;
        public bool IsDead => Current <= 0;

        public LifePoolComponent(int maximum = 1)
        {
            Current = maximum;
            Maximum = maximum;
        }

        public void AddPoints(int points)
        {
            Current = Math.Clamp(Current + points, 0, Maximum);
        }

        public void ApplyDamage(int points)
        {
            AddPoints(-points);
        }
    }

    public class LevelComponent : IComponent
    {
        public int Level = 1;

        public void IncreaseLevel()
        {
            Level++;
        }
    }

    public class NameComponent : IComponent
    {
        public string Name = "";

        public NameComponent()
        {
            Name = "??";
        }

        public NameComponent(string name)
        {
            Name = name;
        }
    }

    public class MobComponent : IComponent
    {
        public string Id { get; set; } = "";

        public MobComponent(string id)
        {
            Id = id;
        }
    }

    public class PlayerComponent : IComponent
    {
        public HashSet<GameFeature> AvailableFeatures { get; set; } = new();

        public void SetFeatureState(GameFeature feature, bool available)
        {
            if (available)
                AvailableFeatures.Add(feature);
            else
                AvailableFeatures.Remove(feature);
        }
    }

    public class AttackComponent : IComponent
    {
        public double RawDamage = 1;
        public Cooldown Cooldown = new(1.0);      
    }

    public class DefenseComponent : IComponent
    {
        public double Armor = 0.0;
        public double Evasion = 0.0;
    }

    public class KilledComponent : IComponent
    {
        public uint Killer = 0;
    }

    public class XpGiverComponent : IComponent
    {
        public int Amount = 0;
    }

    public class XpGainerComponent : IComponent
    {
        public int Current = 0;
        public Func<int, int> TargetFunction; // Calculates XP required to go from level n to n+1

        public XpGainerComponent()
        {
            TargetFunction = lvl => (int)Math.Min(
                Math.Round(
                    (11.0) 
                    * Math.Pow(lvl, 1.74) 
                    * Math.Pow(1.14, Math.Pow(lvl, 0.75))
                ), 
                1_500_000_000);
        }

        public XpGainerComponent(Func<int, int> targetFunction)
        {
            TargetFunction = targetFunction;
        }

        public void AddXp(int amount)
        {
            Current += amount;
        }

        public void Reset()
        {
            Current = 0;
        }
    }

    public class ItemComponent : IComponent
    {
        public ItemBlueprint Blueprint { get; set; }

        public ItemComponent(string itemCode)
        {
            Blueprint = ItemBlueprint.Parse(itemCode);
        }

        public ItemComponent(ItemBlueprint blueprint)
        {
            Blueprint = blueprint;
        }
    }

    public class EquippableComponent : IComponent
    {
        public List<EquipmentSlot> Slots { get; set; }

        public double Encumbrance { get; set; } 

        public EquippableComponent(List<EquipmentSlot> slots, double encumbrance)
        { 
            Slots = slots;
            Encumbrance = encumbrance;
        }
    }

    public class WeaponComponent : IComponent
    {
        //public string Family = "";
        //public string Genus = "";
        public double Damage = 1.0;
        public double Cooldown = 1.0;

        public WeaponComponent()
        {

        }

        public WeaponComponent(double baseDamage, double baseCooldown)
        {
            Damage = baseDamage;
            Cooldown = baseCooldown;
        }
    }

    public class ArmorComponent : IComponent
    {
        public double Armor { get; set; }
        public double Evasion { get; set; }

        public ArmorComponent(double armor, double evasion)
        {
            Armor = armor;
            Evasion = evasion;
        }
    }

    public class ItemQualityComponent : IComponent
    {
        public int Quality { get; set; }

        public ItemQualityComponent(int quality)
        {
            Quality = quality;
        }
    }

    public class ItemMaterialComponent : IComponent
    {
        public string Name { get; set; }

        public int Tier { get; set; }

        public ItemMaterialComponent(string name, int tier)
        {
            Name = name;
            Tier = tier;
        }   
    }

    public class MobDamageComponent : IComponent
    {
        public double Multiplier { get; set; }

        public MobDamageComponent(double multiplier)
        {
            Multiplier = multiplier;
        }
    }

    public class AccuracyComponent : IComponent
    {
        public double Accuracy { get; set; }

        public AccuracyComponent(double accuracy)
        {
            Accuracy = accuracy;
        }
    }

    public class TravellerComponent : IComponent
    {
        public bool Active { get; set; } = true;
        public int MaxWilderness { get; set; } = 0;
        public Dictionary<string, int[]> AvailableDungeons { get; set; } = [];
        public bool AutoProceed { get; set; } = false;
        public bool AutoGrindDungeons { get; set; } = false;
    }

    public class CoinPurseComponent : IComponent
    {
        public int Coins = 0;

        public void AddCoins(int value)
        {
            Coins += value;
        }

        public bool RemoveCoins(int value)
        {
            if (value > Coins)
                return false;
            Coins -= value;
            return true;
        }

        public void Empty()
        {
            Coins = 0;
        }
    }

    public class ItemValueComponent : IComponent
    {
        public int Value = 0;
    }

    public class ItemRefinableComponent : IComponent
    {
        public int Cost = 0;

        public bool Refined = false;
    }

    /// <summary>
    /// Denotes to the SaveSystem that the entity is should not be saved (used for testing).
    /// </summary>
    public class ROMComponent : IComponent
    {
        public bool ROM = true;
    }
}
