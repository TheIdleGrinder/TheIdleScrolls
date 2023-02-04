using TheIdleScrolls_Core.Components;

namespace Test_TheIdleScrolls_Core
{
    public class Test_Abilities
    {
        const string Key = "key";

        Ability ability;
        AbilitiesComponent component;
        
        [SetUp]
        public void Setup()
        {
            ability = new Ability(Key);
            component = new();
        }

        [Test]
        public void Constructing_works()
        {
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
            const int amount = 50;
            ability.AddXP(amount);
            Assert.That(ability.XP, Is.EqualTo(amount));
            ability.AddXP(amount);
            Assert.That(ability.XP, Is.EqualTo(2 * amount));
            ability.AddXP(2 * amount);
            Assert.That(ability.XP, Is.EqualTo(4 * amount));
        }

        //[Test]
        //public void Adding_ability_to_component_works()
        //{
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(component.GetAbilities(), Has.Count.EqualTo(0));
        //        Assert.That(component.GetAbility(Key), Is.Null);
        //    });

        //    component.AddAbility(ability);
        //    Assert.Multiple(() =>
        //    {
        //        Assert.That(component.GetAbilities(), Has.Count.EqualTo(1));
        //        Assert.That(component.GetAbility(Key), Is.EqualTo(ability));
        //    });
        //}

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        public void Adding_ability_to_component_sets_target_XP(int level)
        {
            ability.Level = level;
            component.AddAbility(ability);
            Assert.That(ability.TargetXP, Is.EqualTo(60 * level));
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
        [TestCase(50)]
        public void Adding_XP_to_ability_in_component_works(int amount)
        {
            component.AddAbility(ability);
            var result = component.AddXP(Key, amount);
            Assert.That(result, Is.EqualTo(AbilitiesComponent.AddXPResult.Added));
            Assert.That(ability.XP, Is.EqualTo(amount));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(83)]
        public void Adding_XP_to_ability_in_component_may_increase_level(int level)
        {
            ability.Level = level;
            component.AddAbility(ability);
            component.AddXP(Key, ability.TargetXP + level);
            Assert.That(ability.Level, Is.EqualTo(level + 1));
            Assert.That(ability.XP, Is.EqualTo(level));
        }

        public void Level_can_increase_more_than_once()
        {
            component.AddAbility(ability);
            Assert.That(ability.Level, Is.EqualTo(1));
            component.AddXP(Key, ability.TargetXP * 3);
            Assert.That(ability.Level, Is.EqualTo(3));
            Assert.That(ability.XP, Is.EqualTo(0));
        }
    }
}