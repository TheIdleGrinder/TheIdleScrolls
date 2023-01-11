using System.Collections;
using System.Data;
using System.Text;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Core.Systems;
using TheIdleScrolls_Core.Components;
using System.Collections.Generic;

namespace TheIdleScrollsApp
{
    public partial class MainWindow : Form
    {
        public enum Area { Inventory }

        const int TimePerTick = 100;
        GameRunner m_runner;
        DateTime m_lastTickStart;
        IUserInputHandler m_inputHandler;

        const char BlockChar = '\u258e';

        uint m_playerId = 0;
        int m_areaLevel = 0;
        int m_maxWilderness = 0;
        SortableBindingList<ItemRepresentation> m_Inventory { get; set; }
        Equipment m_Equipment { get; set; }
        SortableBindingList<AbilityRepresentation> m_abilities { get; set; }

        public MainWindow(GameRunner runner, string name = "Leeroy")
        {
            CharacterSelectionDialog dialog = new CharacterSelectionDialog(name);
            dialog.ShowDialog();
            name = dialog.CharacterName;

            InitializeComponent();

            m_runner = runner;
            m_runner.Initialize(name);
            m_inputHandler = m_runner.GetUserInputHandler();
            m_runner.SetAppInterface(new MainWindowUpdater(this));

            timerTick.Interval = TimePerTick;
            timerTick.Enabled = true;
            timerTick.Start();
            m_lastTickStart = DateTime.Now;

            m_Inventory = new(new());
            m_Equipment = new();
            m_abilities = new(new());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gridInventory.DataSource = m_Inventory;
            gridInventory.Columns[0].Visible = false;
            gridInventory.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            gridAbilities.DataSource = m_abilities;
            gridAbilities.Columns[0].Visible = false;
            gridAbilities.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void timerTick_Tick(object sender, EventArgs e)
        {
            var tickStart = DateTime.Now;
            var lastTickDuration = (tickStart - m_lastTickStart).TotalMilliseconds;
            m_lastTickStart = tickStart;

            m_runner.ExecuteTick(lastTickDuration / 1000);
        }

        public void SetFeatureAvailable(GameFeature area, bool available)
        {
            if (GameFeature.Inventory == area)
            {
                tabControl1.TabPages["tabInventory"].Text = available ? "Inventory" : "";
                hdrEqWeapon.Visible = available;
                lblEqWeapon.Visible = available;
                gridInventory.Visible = available;
            } 
            else if (GameFeature.Armor == area)
            {
                hdrEqArmor.Visible = available;
                lblEqArmor.Visible = available;
            }
            else if (GameFeature.Abilities == area)
            {
                tabControl1.TabPages["tabAbilities"].Text = available ? "Abilities" : "";
                gridAbilities.Visible = available;
            }
            else if (GameFeature.Travel == area)
            {
                btnAreaNext.Visible = available;
                btnAreaPrev.Visible = available;
                cbNextAfterWin.Visible = available;
            }
        }

        public void SetCharacter(uint id, string name)
        {
            m_playerId = id;
            lblCharName.Text = name;
        }

        public void SetCharacterLevel(int level)
        {
            lblCharLevel.Text = $"Level {level}";
        }

        public void SetCharacterXP(int current, int target)
        {
            const int bubbles = 40;
            int filledBubbles = (int)Math.Round((bubbles * ((double)current / target)));
            lblCharXP.Text = $"XP: {current:#,0} / {target:#,0}\n" + new string('▰', filledBubbles) + new String('▱', bubbles - filledBubbles);
        }

        public void SetArea(string name, int level)
        {
            btnAreaNext.Enabled = level < m_maxWilderness;
            m_areaLevel = level;
            lblArea.Text = $"{name}";
        }

        public void SetMob(string name, int level)
        {
            lblMobName.Text = $"{name} (Lvl {level})";
        }

        public void SetMobHP(int current, int max)
        {
            var barString = new String(BlockChar, (int)Math.Ceiling(60.0 * current / max));
            lblMobHP.Text = $"HP: {current} / {max}\n{barString}";
        }

        public void SetAttackDamage(double raw, double dps)
        {
            lblAttackRawDmg.Text = raw.ToString("0.##");
            lblAttackDps.Text = dps.ToString("0.0#");
        }

        public void SetAttackCooldown(double duration, double remaining)
        {
            lblAttackCooldown.Text = $"{remaining:0.00} / {duration:0.00}";
        }

        public void SetDefenses(double armor, double evasion)
        {
            lblDefArmor.Text = armor.ToString("0.0#");
            lblDefEvasion.Text = evasion.ToString("0.0#");
        }

        public void SetInventory(List<ItemRepresentation> items)
        {
            m_Inventory = new(items);
            gridInventory.DataSource = m_Inventory;
        }

        public void SetEquipment(List<ItemRepresentation> items)
        {
            m_Equipment.Clear();
            foreach (var item in items)
            {
                foreach (var slot in item.Slots)
                {
                    switch (slot)
                    {
                        case EquipmentSlot.Hand: m_Equipment.Hand = item; break;
                        case EquipmentSlot.Armor: m_Equipment.Armor = item; break;
                    }
                }
            }

            lblEqWeapon.Text = m_Equipment.Hand?.Name ?? "";
            lblEqArmor.Text = m_Equipment.Armor?.Name ?? "";

            toolTip.SetToolTip(lblEqWeapon, m_Equipment.Hand?.Description?.Replace("; ", "\n") ?? "");
            toolTip.SetToolTip(lblEqArmor, m_Equipment.Armor?.Description?.Replace("; ", "\n") ?? "");

            lblAttack.Text = "Attack" + ((m_Equipment.Hand != null) ? $"\n({m_Equipment.Hand?.Name})" : "");
        }

        public void SetAbilities(List<AbilityRepresentation> abilities)
        {
            m_abilities = new(abilities);
            gridAbilities.DataSource = m_abilities;
        }

        public void UpdateTimeLimit(double remaining, double limit)
        {
            lblTimeLimit.Visible = limit > 0.0;
            if (limit > 0.0)
            {
                var barString = new String(BlockChar, (int)Math.Ceiling(50 * remaining / limit));
                lblTimeLimit.Text = $"{remaining:0.000} s\n{barString}";
            }
        }

        public void SetAutoProceed(bool autoProceed)
        {
            if (autoProceed != cbNextAfterWin.Checked)
                cbNextAfterWin.Checked = autoProceed;
        }

        public void SetAccessibleAreas(int maxWilderness, List<DungeonRepresentation> dungeons)
        {
            m_maxWilderness = maxWilderness;
            btnAreaNext.Enabled = m_areaLevel < maxWilderness;

            var buttons = new List<Button>() { btnDungeon1, btnDungeon2 };
            buttons.ForEach(b => b.Visible = false);

            for (int i = 0; i < Math.Min(dungeons.Count, buttons.Count); i++)
            {
                buttons[i].Text = $"{dungeons[i].Name} (Level {dungeons[i].Level})";
                buttons[i].Tag = dungeons[i].Id;
                buttons[i].Visible = true;
                toolTip.SetToolTip(buttons[i], $"{dungeons[i].Name}\nLevel {dungeons[i].Level}\n\n{dungeons[i].Description}");
            }
        }

        public void SetAchievements(List<AchievementRepresentation> visibleAchievements, int achievementCount)
        {
            gridAchievements.Rows.Clear();
            foreach (var a in visibleAchievements)
            {
                var idx = gridAchievements.Rows.Add(a.Earned, a.Title, a.Description);
                //gridAchievements.Rows[idx].DefaultCellStyle(new DataGridViewCellStyle() { })
            }
            lblAchievementCount.Text = $"{visibleAchievements.Count(a => a.Earned)} / {achievementCount} Completed";
        }

        public void AddLogMessages(List<string> messages)
        {
            if (!messages.Any())
                return;
            lblFooter.Text = "Recent news: " + String.Join(" | ", messages);
        }

        private void btnDungeon_Click(object sender, EventArgs e)
        {
            var dungeonId = ((Button)sender).Tag;
            m_inputHandler.EnterDungeon((string)dungeonId);
        }

        public void ShowMessageBox(string title, string text)
        {
            Thread t = new(() => MessageBox.Show(text, title));
            t.Start();
        }

        private void gridInventory_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= gridInventory.Rows.Count)
                return;
            ItemRepresentation item = m_Inventory[e.RowIndex];
            m_inputHandler.EquipItem(m_playerId, item.Id);
        }

        private void lblEqWeapon_DoubleClick(object sender, EventArgs e)
        {
            if (m_Equipment.Hand != null)
                m_inputHandler.UnequipItem(m_playerId, m_Equipment.Hand.Id);
        }

        private void lblEqArmor_DoubleClick(object sender, EventArgs e)
        {
            if (m_Equipment.Armor != null)
                m_inputHandler.UnequipItem(m_playerId, m_Equipment.Armor.Id);
        }

        private void btnAreaPrev_Click(object sender, EventArgs e)
        {
            m_inputHandler.TravelIntoWilderness(m_areaLevel - 1);
        }

        private void btnAreaNext_Click(object sender, EventArgs e)
        {
            m_inputHandler.TravelIntoWilderness(m_areaLevel + 1);
        }

        private void cbNextAfterWin_CheckedChanged(object sender, EventArgs e)
        {
            m_inputHandler.SetAutoProceed(cbNextAfterWin.Checked);
        }

        private void gridAchievements_SelectionChanged(object sender, EventArgs e)
        {
            gridAchievements.CurrentRow.Selected = false;
        }
    }

    //public class InventoryItem
    //{
    //    public uint Id { get; set; }
    //    public string Name { get; set; }
    //    public string Slot { get; set; }
    //    public string Damage { get; set; }
    //    public string Speed { get; set; }
    //    public string Family { get; set; }

    //    public InventoryItem(uint id, string name, string slot, string stat1, string stat2, string family)
    //    {
    //        Id = id;
    //        Name = name;
    //        Slot = slot;
    //        Damage = stat1;
    //        Speed = stat2;
    //        Family = family;
    //    }

    //    public InventoryItem()
    //    {
    //        Id = 0;
    //        Name = Slot = Damage = Speed = Family = "";
    //    }
    //}

    class Equipment
    {
        public ItemRepresentation? Hand { get; set; }
        public ItemRepresentation? Armor { get; set; }

        public Equipment()
        {
            Clear();
        }

        public void Clear()
        {
            Hand = null;
            Armor = null;
        }
    }

    public class AbilityRepresentation
    {
        public string Key { get; set; }
        public string Name { set; get; }
        public int Level { get; set; }
        public string Progress { get; set; }

        public AbilityRepresentation(string key, string name, int level, string progress)
        {
            Key = key;
            Name = name;
            Level = level;
            Progress = progress;
        }
    }
}