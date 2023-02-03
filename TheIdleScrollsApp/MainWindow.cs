using System.Collections;
using System.Data;
using System.Text;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Core.Systems;
using TheIdleScrolls_Core.Components;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Intrinsics.X86;

namespace TheIdleScrollsApp
{
    public partial class MainWindow : Form
    {
        public enum Area { Inventory }

        const int TimePerTick = 100;
        GameRunner m_runner;
        DateTime m_lastTickStart;
        IUserInputHandler m_inputHandler;

        uint m_playerId = 0;
        int m_areaLevel = 0;
        int m_maxWilderness = 0;
        bool m_canTravel = false;
        bool m_inDungeon = false;
        SortableBindingList<ItemRepresentation> m_Inventory { get; set; }
        Equipment m_Equipment { get; set; }
        SortableBindingList<AbilityRepresentation> m_abilities { get; set; }

        public MainWindow(GameRunner runner, string name = "Leeroy")
        {
            CharacterSelectionDialog dialog = new CharacterSelectionDialog(name);
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK)
                throw new KeyNotFoundException("No character selected");
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
            gridInventory.Columns[1].MinimumWidth = 175;
            gridInventory.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridInventory.Columns[3].MinimumWidth = 35;
            gridInventory.Columns[3].Visible = false;
            gridInventory.Columns[4].Visible = false;

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

        private static Color GetColorForRarity(int rarity)
        {
            switch (rarity)
            {
                case 1: return Color.Purple;
                case 2: return Color.MediumBlue;
                case 3: return Color.Teal;
                case 4: return Color.ForestGreen;
                case 5: return Color.YellowGreen;
                case 6: return Color.Goldenrod;
                case 7: return Color.Red;
                default: return Color.Black;
            }
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
                m_canTravel = available;
                UpdateButtonVisibility();
            }
        }

        private void UpdateButtonVisibility()
        {
            btnAreaNext.Visible 
                = btnAreaPrev.Visible
                = cbNextAfterWin.Visible = m_canTravel && !m_inDungeon;
            btnLeaveDungeon.Visible = m_canTravel && m_inDungeon;
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

        public void SetPlayerCoins(int coins)
        {
            lblCoins.Text = $"{coins} Coins";
        }

        public void SetArea(string name, int level, bool isDungeon)
        {
            btnAreaNext.Enabled = level < m_maxWilderness;
            m_areaLevel = level;
            lblArea.Text = $"{name}";

            m_inDungeon = isDungeon;
            UpdateButtonVisibility();
        }

        public void SetMob(string name, int level)
        {
            lblMobName.Text = $"{name} (Lvl {level})";
        }

        private string BuildBarString(int maxLength, double progress, double maximum)
        {
            //return new string('█', maxLength);
            double percentage = (double)progress / maximum;
            int fullBlocks = (int)Math.Floor(percentage * maxLength);
            string result = new string('█', fullBlocks);

            List<char> parts = new() { '▏', '▎', '▍', '▌', '▋', '▊', '▉', '█' };
            double remainder = (percentage - (double)fullBlocks / maxLength) * maxLength; // Percentage of a block that remains
            int eighths = (int)Math.Round(remainder * 8); // Remainder is displayed in 1/8-block slices
            if (eighths == 0)
                return result;
            
            result += parts[eighths - 1]; // Select best fitting partial block
            return result;
        }

        public void SetMobHP(int current, int max)
        {
            var barString = BuildBarString(16, current, max);
            lblMobHP.Text = $"HP: {current:#,0} / {max:#,0}\n{barString}";
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
            int offset = gridInventory.FirstDisplayedScrollingRowIndex;
            int currentRow = gridInventory.CurrentRow?.Index ?? -1;
            int selection = gridInventory.Rows.GetFirstRow(DataGridViewElementStates.Selected);
            m_Inventory = new(items);
            gridInventory.DataSource = m_Inventory;
            if (offset >= 0)
                gridInventory.FirstDisplayedScrollingRowIndex = offset;
            if (currentRow >= gridInventory.Rows.Count)
                currentRow = gridInventory.Rows.Count - 1;
            if (currentRow >= 0 && currentRow < gridInventory.Rows.Count)
                gridInventory.CurrentCell = gridInventory.Rows[currentRow].Cells[1];
            if (selection >= gridInventory.Rows.Count)
                selection = gridInventory.Rows.Count - 1;
            if (selection >= 0 && selection < gridInventory.Rows.Count)
                gridInventory.Rows[selection].Selected = true;
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

            var SetLabelItem = (Label label, ItemRepresentation? item) =>
            {
                int rarity = item?.Rarity ?? 0;
                label.Text = item?.Name ?? "";
                label.ForeColor = GetColorForRarity(rarity);
                label.Font = new Font(label.Font, (rarity > 0) ? FontStyle.Bold : FontStyle.Regular);
                toolTip.SetToolTip(label, item?.Description?.Replace("; ", "\n") ?? "");
            };

            SetLabelItem(lblEqWeapon, m_Equipment.Hand);
            SetLabelItem(lblEqArmor, m_Equipment.Armor);

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
                var barString = BuildBarString(14, remaining, limit);
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

            var buttons = new List<Button>() { 
                btnDungeon1, btnDungeon2, btnDungeon3, btnDungeon4, btnDungeon5,
                btnDungeon6, btnDungeon7, btnDungeon8, btnDungeon9
            };
            buttons.ForEach(b => b.Visible = false);

            for (int i = 0; i < Math.Min(dungeons.Count, buttons.Count); i++)
            {
                buttons[i].Text = $"{dungeons[i].Name} (Level {dungeons[i].Level})";
                buttons[i].Tag = dungeons[i];
                buttons[i].Visible = true;
                buttons[i].ForeColor = GetColorForRarity(dungeons[i].Rarity);
                toolTip.SetToolTip(buttons[i], $"{dungeons[i].Name}\nLevel {dungeons[i].Level}\n\n{dungeons[i].Description}");
            }
        }

        private void btnDungeon_Click(object sender, EventArgs e)
        {
            var dungeon = (DungeonRepresentation)((Button)sender).Tag;

            Thread t = new(() => {
                DialogResult response = DialogResult.OK;
                if (!m_inDungeon)
                {
                    string text = $"{dungeon.Name} - Level {dungeon.Level}\n{dungeon.Description}";
                    response = MessageBox.Show(text, $"Entering {dungeon.Name}", MessageBoxButtons.OKCancel);
                }
                if (response == DialogResult.OK)
                {
                    m_inputHandler.EnterDungeon(dungeon.Id);
                }
            });
            t.Start();
        }

        public void SetAchievements(List<AchievementRepresentation> visibleAchievements, int achievementCount)
        {
            int offset = gridAchievements.FirstDisplayedScrollingRowIndex;
            gridAchievements.Rows.Clear();
            foreach (var a in visibleAchievements)
            {
                var idx = gridAchievements.Rows.Add(a.Earned, a.Title, a.Description);
                //gridAchievements.Rows[idx].DefaultCellStyle(new DataGridViewCellStyle() { })
            }
            if (offset >= 0)
                gridAchievements.FirstDisplayedScrollingRowIndex = offset;
            lblAchievementCount.Text = $"{visibleAchievements.Count(a => a.Earned)} / {achievementCount} Completed";
        }

        public void AddLogMessages(List<string> messages)
        {
            if (!messages.Any())
                return;
            lblFooter.Text = "Recent news: " + String.Join(" | ", messages);
        }

        public void SetStatisticsReport(string report)
        {
            textBoxStats.Text = report;
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

        private void btnLeaveDungeon_Click(object sender, EventArgs e)
        {
            m_inputHandler.LeaveDungeon();
        }

        private void gridInventory_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var cell = gridInventory.Rows[e.RowIndex].Cells[1];
            int rarity = m_Inventory[e.RowIndex].Rarity;
            cell.Style.ForeColor = GetColorForRarity(rarity);
            cell.Style.Font = new Font(gridInventory.Font, (rarity > 0) ? FontStyle.Bold : FontStyle.Regular);
        }

        private void gridInventory_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            gridInventory.ClearSelection();
            int row = e.RowIndex;
            bool validSelection = row >= 0 && row < gridInventory.Rows.Count;
            if (validSelection)
            {
                cMenuInventorySell.Text = $"Sell ({m_Inventory[row].Value}c)";
                cMenuInventorySell.Visible = true;
                gridInventory.Rows[row].Selected = true;
            }
        }

        private void cMenuInventorySell_Click(object sender, EventArgs e)
        {
            int row = gridInventory.Rows.GetFirstRow(DataGridViewElementStates.Selected);
            if (row != -1)
            {
                m_inputHandler.SellItem(m_playerId, m_Inventory[row].Id);
            }
        }

        private void cMenuInventory_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            cMenuInventorySell.Visible = false;
        }

        private void gridInventory_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                for (int i = 0; i < gridInventory.Rows.Count; i++)
                {
                    if (gridInventory.Rows[i].Selected)
                        m_inputHandler.SellItem(m_playerId, m_Inventory[i].Id);
                }
            }
        }
    }

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