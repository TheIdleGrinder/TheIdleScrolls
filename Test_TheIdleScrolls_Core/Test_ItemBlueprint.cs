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
                Assert.That(new ItemBlueprint(ItemFamilies.LightArmor, 2, MaterialId.Leather, 2), Is.EqualTo(ItemBlueprint.Parse("L0-LAR2+2")));
                Assert.That(new ItemBlueprint(ItemFamilies.HeavyArmor, 1, MaterialId.Dwarven, 3), Is.EqualTo(ItemBlueprint.Parse("M2-HAR1+3")));
            });
        }
    }
}
