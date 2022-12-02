using System.Text.Json.Nodes;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_JSON;
using TheIdleScrolls_Storage;

namespace Test_TheIdleScrolls_JSON
{
    public class Test_AbilitiesComponent
    {
        AbilitiesComponent component;

        [SetUp]
        public void Setup()
        {
            component = new();
            var abilityLbl = new Ability("LBL", "Long Blade");
            abilityLbl.Level = 17;
            abilityLbl.XP = 71;
            component.AddAbility(abilityLbl);

            var abilityAxe = new Ability("AXE", "Axe");
            abilityAxe.Level = 2;
            abilityAxe.XP = 15;
            component.AddAbility(abilityAxe);
        }

        [Test]
        public void Back_and_forth_conversion_works()
        {
            var json = component.ToJson();
            Assert.That(json, Is.Not.Null);
            int count = component.GetAbilities().Count;

            AbilitiesComponent newComponent = new();
            bool result = newComponent.SetFromJson(json);
            Assert.That(result, Is.True);
            Assert.That(newComponent.GetAbilities, Has.Count.EqualTo(count));
            
            var abilityLbl = newComponent.GetAbility("LBL");
            Assert.That(abilityLbl, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(abilityLbl.Key, Is.EqualTo("LBL"));
                Assert.That(abilityLbl.Name, Is.EqualTo("Long Blade"));
                Assert.That(abilityLbl.Level, Is.EqualTo(17));
                Assert.That(abilityLbl.XP, Is.EqualTo(71));
            });

            var abilityAxe = newComponent.GetAbility("AXE");
            Assert.That(abilityAxe, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(abilityAxe.Key, Is.EqualTo("AXE"));
                Assert.That(abilityAxe.Name, Is.EqualTo("Axe"));
                Assert.That(abilityAxe.Level, Is.EqualTo(2));
                Assert.That(abilityAxe.XP, Is.EqualTo(15));
            });
        }
    }
}