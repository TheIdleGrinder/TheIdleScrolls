using MiniECS;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.Components;

namespace Test_TheIdleScrolls_Core
{
    public class Test_CharacterClass
    {
        private Entity CreateCharacter(int level, Dictionary<string, int> abilities)
        {
            Entity result = new();
            result.AddComponent(new LevelComponent() { Level = level });
            var abiComp = new AbilitiesComponent();
            result.AddComponent(abiComp);
            foreach (string a in abilities.Keys)
            {
                abiComp.AddAbility(a, abilities[a]);
            }
            return result;
        }

        private void SetLevel(Entity character, int level)
        {
            character.AddComponent(new LevelComponent() { Level = level });
        }

        private void SetAbilityLevel(Entity character, string ability, int level)
        {
            var abiComp = character.GetComponent<AbilitiesComponent>()!;
            if (abiComp.GetAbility(ability) == null)
                abiComp.AddAbility(ability, level);
            else
                abiComp.UpdateAbility(ability, level, 0);
        }

        [Test]
        public void Localizations_for_all_classes_exist()
        {
            List<string> weapons = new() { "X", "AXE", "BLN", "LBL", "POL", "SBL" };
            List<string> armors = new() { "X", "HAR", "LAR" };
            foreach (var weapon in weapons)
            {
                foreach (var armor in armors)
                {
                    string key = $"CLASS_{weapon}_{armor}";
                    Assert.That(key.Localize(), Is.Not.EqualTo(key));
                }
            }
        }

        [Test]
        public void Default_class_is_selected_correctly()
        {
            const string defaultKey = "CLASS_DEFAULT";

            var cha = CreateCharacter(1, new());
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo(defaultKey));

            SetLevel(cha, 20);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.Not.EqualTo(defaultKey));

            cha = CreateCharacter(19, new() { { "LBL", 30 } });
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo(defaultKey));

            SetLevel(cha, 20);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.Not.EqualTo(defaultKey));
            
            cha = CreateCharacter(19, new() { { "LAR", 30 } });
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo(defaultKey));

            SetLevel(cha, 30);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.Not.EqualTo(defaultKey));

            SetLevel(cha, 100);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.Not.EqualTo(defaultKey));
        }

        [Test]
        public void Classes_are_selected_correctly()
        {
            var cha = CreateCharacter(20, new());
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo("CLASS_X_X"));

            SetAbilityLevel(cha, "LBL", 20);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo("CLASS_LBL_X"));

            SetLevel(cha, 50);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo("CLASS_X_X"));

            SetAbilityLevel(cha, "LAR", 50);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo("CLASS_X_LAR"));

            SetAbilityLevel(cha, "HAR", 51);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo("CLASS_X_HAR"));

            SetAbilityLevel(cha, "POL", 30);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo("CLASS_X_HAR"));

            SetLevel(cha, 40);
            Assert.That(PlayerFactory.GetCharacterClass(cha), Is.EqualTo("CLASS_POL_HAR"));
        }
    }
}
