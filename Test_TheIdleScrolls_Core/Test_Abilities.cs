using TheIdleScrolls_Core.Components;
using TheIdleScrolls_Core.Definitions;
using TheIdleScrolls_Core.Resources;

namespace Test_TheIdleScrolls_Core
{
    public class Test_Abilities
    {
        const string Key = Abilities.Axe;

        AbilitiesComponent component;
        
        [SetUp]
        public void Setup()
        {
            component = new();
        }

        [Test]
        public void Constructing_works()
        {
            var ability = new Ability(Key);
            Assert.Multiple(() =>
            {
                Assert.That(ability, Is.Not.Null);
                Assert.That(ability.Key, Is.EqualTo(Key));
                Assert.That(ability.Level, Is.EqualTo(1));
                Assert.That(ability.XP, Is.EqualTo(0));
                Assert.That(ability.TargetXP, Is.EqualTo(100));
            });
        }

        [Test]
        public void Adding_XP_works()
        {
            var ability = new Ability(Key);
            const int amount = 50;
            ability.AddXP(amount);
            Assert.That(ability.XP, Is.EqualTo(amount));
            ability.AddXP(amount);
            Assert.That(ability.XP, Is.EqualTo(2 * amount));
            ability.AddXP(2 * amount);
            Assert.That(ability.XP, Is.EqualTo(4 * amount));
        }

        [Test]
        public void Adding_ability_to_component_works()
        {
            Assert.Multiple(() =>
            {
                Assert.That(component.GetAbilities(), Has.Count.EqualTo(0));
                Assert.That(component.GetAbility(Key), Is.Null);
            });

            component.AddAbility(Key);
            Assert.Multiple(() =>
            {
                Assert.That(component.GetAbilities(), Has.Count.EqualTo(1));
                Assert.That(component.GetAbility(Key), Is.Not.Null);
            });
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        public void Adding_ability_to_component_sets_target_XP(int level)
        {
            component.AddAbility(Key, level);
            var ability = component.GetAbility(Key);
            Assert.That(ability, Is.Not.Null);
            Assert.That(ability.TargetXP, Is.EqualTo(AbilityList.GetAbility(ability.Key)!.RequiredXpForLevelUp(level)));
        }

        [Test]
        public void Adding_XP_to_ability_not_in_component_returns_false()
        {
            int abilityCount = component.GetAbilities().Count;
            var result = component.AddXP(Key, 50);
            Assert.That(result, Is.EqualTo(AbilitiesComponent.AddXPResult.NotFound));
            Assert.That(component.GetAbilities(), Has.Count.EqualTo(abilityCount));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(22)]
        public void Adding_XP_to_ability_in_component_works(int amount)
        {
            component.AddAbility(Key);
            var result = component.AddXP(Key, amount);
            var ability = component.GetAbility(Key);
            Assert.That(ability, Is.Not.Null);
            Assert.That(result, Is.EqualTo(AbilitiesComponent.AddXPResult.Added));
            Assert.That(ability.XP, Is.EqualTo(amount));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(83)]
        public void Adding_XP_to_ability_in_component_may_increase_level(int level)
        {
            component.AddAbility(Key, level);
            var ability = component.GetAbility(Key);
            Assert.That(ability, Is.Not.Null);
            component.AddXP(Key, ability.TargetXP + level);
            Assert.That(ability.Level, Is.EqualTo(level + 1));
            Assert.That(ability.XP, Is.EqualTo(level));
        }

        public void Level_can_increase_more_than_once()
        {
            component.AddAbility(Key);
            var ability = component.GetAbility(Key);
            Assert.That(ability, Is.Not.Null);
            Assert.That(ability.Level, Is.EqualTo(1));
            var xpFunction = AbilityList.GetAbility(ability.Key)!.RequiredXpForLevelUp;
            int xp = xpFunction(2) + xpFunction(3);
            component.AddXP(Key, xp);
            Assert.That(ability.Level, Is.EqualTo(3));
            Assert.That(ability.XP, Is.EqualTo(0));
        }
    }
}