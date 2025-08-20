using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Achievements;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Modifiers;
using TheIdleScrolls_Core.Resources;
using TheIdleScrolls_Core.Utility;

namespace TheIdleScrolls_Core.ContentPacks
{
	internal class UnderequippedAchievementPack : IContentPack
	{
		const double NoArmorBaseEvasion = 25;
		const double NoArmorBaseEvasion1 = 1.0;
		const double NoArmorBaseEvasion2 = 2.0;

		const int NoWeaponLevel = 5;
		const double NoWeaponBaseDamageBonus = 3.0;
		const double NoWeaponLevelDamageBonus = 0.1;
		const double NoWeaponMaxLevel = 50;


		public string Id => "CP_UnderequippedAchievements";
		public string Name => "Limited Equipment";
		public string Description => "Achievements for reaching certain milestones while not using weapons or armor";
		public List<IContentPiece> ContentPieces => [
			new AchievementContent(new(
				"NOARMOR1",
				"Plain Clothes",
				$"Complete the {Properties.Places.Dungeon_RatDen} without ever raising an armor ability",
				(e, w) => true,
				ExpressionParser.ParseToFunction($"dng:{DungeonIds.DenOfRats} > 0 && abl:LAR <= 1 && abl:HAR <= 1"))
				{
					Reward = new PerkReward(PerkFactory.MakeStaticPerk("NOARMOR1",
						"Agile",
						$"Gain +{NoArmorBaseEvasion:0.#} base evasion rating while unarmored",
						ModifierType.AddBase,
						NoArmorBaseEvasion,
						[Tags.EvasionRating, Tags.Global],
						[Tags.Unarmored]
					))
				}
			),
			new AchievementContent(new(
				"NOARMOR2",
				"Wollt Ihr Ewig Leben?!",
				$"Complete the {Properties.Places.Dungeon_Lighthouse} without ever raising an armor ability",
				ExpressionParser.ParseToFunction("NOARMOR1"),
				ExpressionParser.ParseToFunction($"dng:{DungeonIds.Lighthouse} > 0 && abl:LAR <= 1 && abl:HAR <= 1"))
				{
					Reward = new AbilityReward(Abilities.Unarmored)
				}
			),
			new AchievementContent(new(
				"HC:NOARMOR",
				"Not Today",
				"Complete the Beacon without ever raising an armor ability or losing a fight",
				ExpressionParser.ParseToFunction("NOARMOR2"),
				ExpressionParser.ParseToFunction($"dng:{DungeonIds.Lighthouse} > 0 && abl:LAR <= 1 && abl:HAR <= 1 && Losses == 0"))
				{
					Reward = new PerkReward(PerkFactory.MakeAbilityLevelBasedPerk("HC:NOARMOR",
						"Elusive",
						$"Gain +{NoArmorBaseEvasion1:0.#} evasion rating per level of the {Properties.LocalizedStrings.UNARMORED} ability",
						Abilities.Unarmored,
						ModifierType.AddBase,
						NoArmorBaseEvasion1,
						[Tags.EvasionRating, Tags.Global],
						[]
					))
				}
			),
			new AchievementContent(new(
				"HC:NOARMOR_50",
				"Armored in Faith",
				"Reach level 50 without ever raising an armor ability or losing a fight",
				ExpressionParser.ParseToFunction("HC:NOARMOR"),
				ExpressionParser.ParseToFunction("Level >= 50 && abl:LAR <= 1 && abl:HAR <= 1 && Losses == 0"))
				{
					Reward = new PerkReward(PerkFactory.MakeAbilityLevelBasedPerk("HC:NOARMOR_50",
						"Ethereal Form",
						$"Gain +{NoArmorBaseEvasion2:0.#} base evasion rating per level of the {Properties.LocalizedStrings.UNARMORED} ability",
						Abilities.Unarmored,
						ModifierType.AddBase,
						NoArmorBaseEvasion2,
						[Tags.EvasionRating, Tags.Global],
						[]
					))
				}
			),
			new AchievementContent(new(
				"NOWEAPON",
				"Boxer",
				$"Reach level {NoWeaponLevel} without ever raising a weapon ability",
				(e, w) => true,
				ExpressionParser.ParseToFunction($"Level >= {NoWeaponLevel} && abl:AXE <= 1 && abl:BLN <= 1 " +
					$"&& abl:LBL <= 1 && abl:POL <= 1 && abl:SBL <= 1"))
				{
					Reward = new PerkReward(PerkFactory.MakeStaticPerk("NOWEAPON",
						"Boxer",
						$"Increased base damage with unarmed attacks",
						ModifierType.AddBase,
						NoWeaponBaseDamageBonus,
						[Tags.Damage],
						[Tags.Unarmed]
					))
				}
			),
			new AchievementContent(new(
				"NOWEAPON_DMG",
				"Prize Fighter",
				$"Complete the {Properties.Places.Dungeon_RatDen} without ever raising a weapon ability",
				ExpressionParser.ParseToFunction("NOWEAPON"),
				ExpressionParser.ParseToFunction($"dng:{DungeonIds.DenOfRats} > 0 && abl:AXE <= 1 " +
					$"&& abl:BLN <= 1 && abl:LBL <= 1 && abl:POL <= 1 && abl:SBL <= 1"))
				{
					Reward = new AbilityReward(Abilities.Unarmed)
				}
			),
			new AchievementContent(new(
				"NOWEAPON_DMG2",
				"I Am The Greatest!",
				$"Complete the {Properties.Places.Dungeon_Lighthouse} without ever raising a weapon ability",
				ExpressionParser.ParseToFunction("NOWEAPON_DMG"),
				ExpressionParser.ParseToFunction($"dng:{DungeonIds.Lighthouse} > 0 && abl:AXE <= 1 " +
					$"&& abl:BLN <= 1 && abl:LBL <= 1 && abl:POL <= 1 && abl:SBL <= 1"))
				{
					Reward = new PerkReward(PerkFactory.MakeAbilityLevelBasedPerk("NOWEAPON_DMG2",
						"Iron Fists",
						$"Gain +{NoWeaponLevelDamageBonus:0.#} unarmed damage per level of the {Properties.LocalizedStrings.UNARMED} ability",
						Abilities.Unarmed,
						ModifierType.AddBase,
						NoWeaponLevelDamageBonus,
						[Tags.Damage],
						[]
					))
				}
			),
			new AchievementContent(new(
				$"HC:NOWEAPON_{NoWeaponMaxLevel}",
				"Path of the Monk",
				$"Reach level {NoWeaponMaxLevel} without ever raising a weapon ability or losing a fight",
				ExpressionParser.ParseToFunction("NOWEAPON_DMG2"),
				ExpressionParser.ParseToFunction($"Level >= {NoWeaponMaxLevel} && abl:AXE <= 1 && abl:BLN <= 1 && abl:LBL <= 1 " +
					"&& abl:POL <= 1 && abl:SBL <= 1 && Losses == 0"))
				{
					Reward = new PerkReward(PerkFactory.MakeAbilityLevelBasedPerk($"HC:NOWEAPON_{NoWeaponMaxLevel}",
						"Force of Spirit",
						$"Gain {Stats.AttackBonusPerLevel:0.#%} increased unarmed damage per level of the {Properties.LocalizedStrings.UNARMED} ability",
						Abilities.Unarmed,
						ModifierType.Increase,
						Stats.AttackBonusPerLevel,
						[Tags.Damage],
						[]
					))
				}
			),
			new AchievementContent(new(
				"HC:NOARMOR+NOWEAPON",
				"One with Nothing",
				"Defeat a level 75 enemy in the wilderness without raising a weapon or armor ability or losing a fight",
				ExpressionParser.ParseToFunction($"HC:NOWEAPON_{NoWeaponMaxLevel}"),
				ExpressionParser.ParseToFunction("Wilderness >= 75 && abl:AXE <= 1 && abl:BLN <= 1 && abl:LBL <= 1 && abl:POL <= 1 " +
					"&& abl:SBL <= 1 && abl:LAR <= 1 && abl:HAR <= 1 && Losses == 0"))
				{
					Reward = new PerkReward(PerkFactory.MakeStaticMultiModPerk("HC:NOARMOR+NOWEAPON",
						"Luminous Being",
						"Gain a significant bonus to damage and defense while unarmed and unarmored",
						[ModifierType.More, ModifierType.More],
						[0.5, 0.5],
						[[Tags.Damage], [Tags.Defense]],
						[[Tags.Unarmed, Tags.Unarmored], [Tags.Unarmed, Tags.Unarmored]]
					))
				}
			)
		];
	}
}
