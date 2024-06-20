using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                            });
                        }
                    }
                }
            }
        }
    }
}
