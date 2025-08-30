using MiniECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Skills.SkillEffects;

namespace TheIdleScrolls_Core.Skills
{
	public static class DefaultAttack
	{
		public static readonly ActiveSkillDefinition Skill = new(
			"DfltAttack", 
			"Default Attack", 
			ActiveSkillDefinition.TargetingMode.SingleEnemy,
			UpdateFunction);

		public static void UpdateFunction(Entity user, ActiveSkill skill)
		{
			var equipComp = user.GetComponent<EquipmentComponent>();
			var modComp = user.GetComponent<ModifierComponent>();

			double rawDamage = 2.0;
			double cooldown = 1.0;
			int weaponCount = 0;
			double encumbrance = 0.0;

			var globalTags = user.GetTags();

			if (equipComp != null)
			{
				double combinedDmg = 0.0;
				double combinedCD = 0.0;

				foreach (var item in equipComp.GetItems())
				{
					var itemComp = item.GetComponent<ItemComponent>();
					var weaponComp = item.GetComponent<WeaponComponent>();
					var localTags = item.GetTags();
					encumbrance += item.GetComponent<EquippableComponent>()?.Encumbrance ?? 0.0;

					// Add situational local tags 
					var slots = item.GetRequiredSlots();
					if (slots.Count == 1 && slots[0] == EquipmentSlot.Hand)
					{
						localTags.Add((item.IsShield() || weaponCount > 0) ? Tags.OffHand : Tags.MainHand);
					}

					if (itemComp != null && weaponComp != null)
					{
						double localDmg = weaponComp.Damage;
						double localCD = weaponComp.Cooldown;
						weaponCount++;

						if (modComp != null)
						{
							localDmg = modComp.ApplyApplicableModifiers(localDmg, localTags.Append(Tags.Damage), globalTags);
							localCD = 1.0 / modComp.ApplyApplicableModifiers(1.0 / localCD,
								localTags.Append(Tags.AttackSpeed),  // invert due to speed/cooldown mismatch
								globalTags);
						}

						combinedDmg += localDmg;
						combinedCD += localCD;
						//Console.WriteLine($"{item.GetName()}({weaponCount}): Dmg: {localDmg} -> {combinedDmg}; CD: {localCD} -> {combinedCD}");
					}
				}

				if (weaponCount > 0)
				{
					rawDamage = (combinedDmg / weaponCount);
					cooldown = (combinedCD / weaponCount);
				}
			}

			if (weaponCount == 0)
			{
				rawDamage = modComp?.ApplyApplicableModifiers(rawDamage,
					[Tags.Damage, Abilities.Unarmed], globalTags) ?? rawDamage;
				// invert attack speed due to speed/cooldown mismatch
				cooldown = 1.0 / modComp?.ApplyApplicableModifiers(1.0 / cooldown,
					[Tags.AttackSpeed, Abilities.Unarmed], globalTags) ?? cooldown;
			}

			double encumbranceSlowdown = 1.0 + Math.Max(encumbrance, 0.0) / 100.0;

			DamageSkillEffect dmgEffect = new(Math.Round(rawDamage), ["Attack"]);
			skill.Effects = [dmgEffect];

			cooldown *= encumbranceSlowdown; // Encumbrance slows attack speed multiplicatively
			cooldown = Math.Max(cooldown, 1.0 / Stats.MaxAttacksPerSecond); // Cap attack speed
			skill.ChargingTime = cooldown;
		}
	}
}
