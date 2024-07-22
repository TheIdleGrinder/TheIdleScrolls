using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Items;

namespace Test_TheIdleScrolls_Core
{
    public class Test_ItemBlueprint
    {
        [Test]
        public void BackAndForthConversionWorks()
        {
            foreach (var family in ItemKingdom.Families)
            {
                for (int genusIndex = 0; genusIndex < family.Genera.Count; genusIndex++)
                {
                    foreach (var materialId in family.Genera[genusIndex].ValidMaterials)
                    {
                        for (int rarity = 0; rarity <= ItemKingdom.Rarities.Count; rarity++)
                        {
                            ItemBlueprint blueprint = new(family.Id, genusIndex, materialId, rarity);
                            string code = blueprint.ToString();
                            ItemBlueprint result = ItemBlueprint.Parse(code);
                            Assert.Multiple(() =>
                            {
                                Assert.That(result.FamilyId, Is.EqualTo(blueprint.FamilyId));
                                Assert.That(result.GenusIndex, Is.EqualTo(blueprint.GenusIndex));
                                Assert.That(result.MaterialId, Is.EqualTo(blueprint.MaterialId));
                                Assert.That(result.Rarity, Is.EqualTo(blueprint.Rarity));
                                Assert.That(blueprint, Is.EqualTo(result));
                            });
                        }
                    }
                }
            }
        }

        [Test]
        public void WorksWithOldItemCodeFormat()
        {
            Assert.Multiple(() =>
            {
                Assert.That(new ItemBlueprint(ItemFamilies.ShortBlade, 1, MaterialId.Steel, 1), Is.EqualTo(ItemBlueprint.Parse("M1-SBL1+1")));
                Assert.That(new ItemBlueprint(ItemFamilies.Polearm, 0, MaterialId.Simple, 0), Is.EqualTo(ItemBlueprint.Parse("SIMPLE-POL0")));

                Assert.That(new ItemBlueprint(ItemFamilies.LightChest,  0, MaterialId.Leather, 2), Is.EqualTo(ItemBlueprint.Parse("L0-LAR0+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightChest,  1, MaterialId.Leather, 3), Is.EqualTo(ItemBlueprint.Parse("L0-LAR1+3")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightHelmet, 0, MaterialId.Leather, 2), Is.EqualTo(ItemBlueprint.Parse("L0-LAR2+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightGloves, 0, MaterialId.Leather, 1), Is.EqualTo(ItemBlueprint.Parse("L0-LAR3+1")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightBoots,  0, MaterialId.Leather, 0), Is.EqualTo(ItemBlueprint.Parse("L0-LAR4")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightShield, 0, MaterialId.Ash,     1), Is.EqualTo(ItemBlueprint.Parse("W2-LAR5+1")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightChest,  2, MaterialId.Leather, 2), Is.EqualTo(ItemBlueprint.Parse("L0-LAR6+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightHelmet, 1, MaterialId.Leather, 3), Is.EqualTo(ItemBlueprint.Parse("L0-LAR7+3")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightGloves, 1, MaterialId.Leather, 2), Is.EqualTo(ItemBlueprint.Parse("L0-LAR8+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightBoots,  1, MaterialId.Leather, 1), Is.EqualTo(ItemBlueprint.Parse("L0-LAR9+1")));
                Assert.That(new ItemBlueprint(ItemFamilies.LightShield, 1, MaterialId.Beech,   0), Is.EqualTo(ItemBlueprint.Parse("W0-LAR10")));

                Assert.That(new ItemBlueprint(ItemFamilies.HeavyChest,  0, MaterialId.Iron,  2), Is.EqualTo(ItemBlueprint.Parse("M0-HAR0+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyChest,  1, MaterialId.Iron,  3), Is.EqualTo(ItemBlueprint.Parse("M0-HAR1+3")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyHelmet, 0, MaterialId.Iron,  2), Is.EqualTo(ItemBlueprint.Parse("M0-HAR2+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyGloves, 0, MaterialId.Iron,  1), Is.EqualTo(ItemBlueprint.Parse("M0-HAR3+1")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyBoots,  0, MaterialId.Iron,  0), Is.EqualTo(ItemBlueprint.Parse("M0-HAR4")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyShield, 0, MaterialId.Ash,   1), Is.EqualTo(ItemBlueprint.Parse("W2-HAR5+1")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyChest,  2, MaterialId.Iron,  2), Is.EqualTo(ItemBlueprint.Parse("M0-HAR6+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyHelmet, 1, MaterialId.Iron,  3), Is.EqualTo(ItemBlueprint.Parse("M0-HAR7+3")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyGloves, 1, MaterialId.Iron,  2), Is.EqualTo(ItemBlueprint.Parse("M0-HAR8+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyBoots,  1, MaterialId.Iron,  1), Is.EqualTo(ItemBlueprint.Parse("M0-HAR9+1")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyShield, 1, MaterialId.Beech, 0), Is.EqualTo(ItemBlueprint.Parse("W0-HAR10")));

            });
        }
    }
}
