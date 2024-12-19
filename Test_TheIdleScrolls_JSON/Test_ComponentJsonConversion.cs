using System.Text.Json.Nodes;
using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
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
            component.AddAbility(Abilities.LongBlade);
            component.UpdateAbility(Abilities.LongBlade, 17, 71);
            component.AddAbility(Abilities.Axe);
            component.UpdateAbility(Abilities.Axe, 2, 15);
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
                Assert.That(abilityLbl.Level, Is.EqualTo(17));
                Assert.That(abilityLbl.XP, Is.EqualTo(71));
            });

            var abilityAxe = newComponent.GetAbility("AXE");
            Assert.That(abilityAxe, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(abilityAxe.Key, Is.EqualTo("AXE"));
                Assert.That(abilityAxe.Level, Is.EqualTo(2));
                Assert.That(abilityAxe.XP, Is.EqualTo(15));
            });
        }
    }

    public class Test_PlayerProgressComponent
    {
        [Test]
        public void Back_and_forth_conversion_works()
        {
            List<TutorialStep> tutorialSteps = new() { TutorialStep.MobAttacks, TutorialStep.Abilities};

            PlayerProgressComponent component = new();
            tutorialSteps.ForEach(s => component.Data.TutorialProgress.Add(s));

            var json = component.ToJson();
            Assert.That(json, Is.Not.Null);

            PlayerProgressComponent deserialized = new();
            deserialized.SetFromJson(json);

            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized.Data.TutorialProgress, Has.Count.EqualTo(2));
            Assert.That(deserialized.Data.TutorialProgress, Contains.Item(TutorialStep.MobAttacks));
            Assert.That(deserialized.Data.TutorialProgress, Contains.Item(TutorialStep.Abilities));
        }
    }
}