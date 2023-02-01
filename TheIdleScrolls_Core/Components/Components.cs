using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Items;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.Components
{
    public class LifePoolComponent : IComponent
    {
        public int Current = 1;
        public int Maximum = 1;

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

    }

    public class PlayerComponent : IComponent
    {

    }

    public class AttackComponent : IComponent
    {
        public uint Target = 0;
        public double RawDamage = 1;
        public Cooldown Cooldown = new(1.0);

        public bool InCombat { get { return Target != 0; } }
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
    }

    public class ItemComponent : IComponent
    {
        public ItemIdentifier Code { get; set; }
        public string FamilyName { get { return Code.FamilyId.Localize(); } }
        public string GenusName { get { return Code.GenusId.Localize(); } }

        public ItemComponent(string itemCode)
        {
            Code = new ItemIdentifier(itemCode);
        }

        public ItemComponent(ItemIdentifier code)
        {
            Code = code;
        }
    }

    public class EquippableComponent : IComponent
    {
        public EquipmentSlot Slot { get; set; }

        public double Encumbrance { get; set; } 

        public EquippableComponent(EquipmentSlot slot, double encumbrance)
        { 
            Slot = slot;
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

    public class ItemRarityComponent : IComponent
    {
        public int RarityLevel { get; set; }

        public ItemRarityComponent(int rarityLevel)
        {
            RarityLevel = rarityLevel;
        }
    }

    public class ItemMaterialComponent : IComponent
    {
        public string Name { get; set; }

        public ItemMaterialComponent(string name)
        {
            Name = name;
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

    public class TravellerComponent : IComponent
    {
        public int MaxWilderness = 0;

        public HashSet<string> AvailableDungeons = new();
    }

    public class CoinPurseComponent : IComponent
    {
        public int Coins = 0;
    }
}
