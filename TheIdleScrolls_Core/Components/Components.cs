﻿using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public double Slowdown = 0.0;
        public double TimeMulti = 1.0;
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
            TargetFunction = lvl => (int)Math.Round(5 * Math.Pow(lvl + 1, 3.5));
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
        public string FamilyName { get; set; }
        public string GenusName { get; set; }

        public ItemComponent(string familyName, string genusName)
        {
            FamilyName = familyName;
            GenusName = genusName;
        }
    }

    public class EquippableComponent : IComponent
    {
        public EquipmentSlot Slot { get; set; }

        public EquippableComponent(EquipmentSlot slot)
        { 
            Slot = slot;
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

    public class MobDamageComponent : IComponent
    {
        public double Multiplier { get; set; }

        public MobDamageComponent(double multiplier)
        {
            Multiplier = multiplier;
        }
    }
}
