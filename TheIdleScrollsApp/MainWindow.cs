using System.Collections;
using System.Data;
using System.Text;
using TheIdleScrolls_Core;
using TheIdleScrolls_Core.DataAccess;
using TheIdleScrolls_Core.Storage;
using TheIdleScrolls_Core.Systems;

namespace TheIdleScrollsApp
{
    public partial class MainWindow : Form
    {
        public enum Area { Inventory }

        const int TimePerTick = 100;
        GameRunner m_runner;
        DateTime m_lastTickStart;
        IUserInputHandler m_userInputHandler;


        uint m_playerId = 0;
        SortableBindingList<InventoryItem> m_Inventory { get; set; }
        Equipment m_Equipment { get; set; }
        SortableBindingList<AbilityRepresentation> m_abilities { get; set; }

        public MainWindow(GameRunner runner, string name = "Leeroy")
        {
            InitializeComponent();

            m_runner = runner;
            m_runner.Initialize(name);
            m_userInputHandler = new CommandProcessingSystem(this, m_runner);

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

        public void SetAreaAvailable(Area area, bool available)
        {
            if (Area.Inventory == area)
            {
                tabControl1.TabPages["tabInventory"].Text = available ? "Inventory" : "";
                hdrEqWeapon.Visible = available;
                lblEqWeapon.Visible = available;
                gridInventory.Visible = available;
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
            lblCharXP.Text = $"XP: {current:#,#} / {target:#,#}";
        }

        public void SetAreaLevel(int level)
        {
            lblArea.Text = $"Wilderness - Level {level}";
        }

        public void SetMob(string name, int level)
        {
            lblMobName.Text = $"{name} (Lvl {level})";
        }

        public void SetMobHP(int current, int max)
        {
            lblMobHP.Text = $"HP: {current} / {max}";
        }

        public void SetAttackDamage(double raw, double dps)
        {
            lblAttackRawDmg.Text = raw.ToString();
            lblAttackDps.Text = dps.ToString("0.0#");
        }

        public void SetAttackCooldown(double duration, double remaining)
        {
            lblAttackCooldown.Text = $"{remaining:0.00} / {duration:0.00}";
        }

        List<InventoryItem> OrderItemList(List<InventoryItem> items)
        {
            //CornerCut: Magic strings :(, how does one ensure a nice order here?
            Func<string, string> familyOrder = new(c => c switch
            {
                "Short Blade" => "A",
                "Long Blade" => "B",
                "Axe" => "C",
                "Blunt" => "D",
                "Polearm" => "E",
                _ => c
            });

            return items.OrderBy(i => familyOrder(i.Family) + i.Name).ToList();
        }

        public void SetInventory(List<InventoryItem> items)
        {
            m_Inventory = new(OrderItemList(items));
            gridInventory.DataSource = m_Inventory;
        }

        public void SetEquipment(Dictionary<string, InventoryItem> items)
        {
            m_Equipment.Clear();
            var item = items.GetValueOrDefault("Hand");
            m_Equipment.Hand = item;
            lblEqWeapon.Text = item?.Name ?? "";
            lblAttack.Text = "Attack";
            if (item == null)
            {
                toolTip.SetToolTip(lblEqWeapon, "");
            }
            else
            {
                toolTip.SetToolTip(lblEqWeapon, $"{item.Name} ({item.Family})\n{item.Damage} DMG\n{item.Speed} s/A");
                lblAttack.Text += $"\n({item.Name})";
            }
        }

        public void SetAbilities(List<AbilityRepresentation> abilities)
        {
            m_abilities = new(abilities);
            gridAbilities.DataSource = m_abilities;
        }

        private void gridInventory_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= gridInventory.Rows.Count)
                return;
            InventoryItem item = m_Inventory[e.RowIndex];
            m_userInputHandler.EquipItem(m_playerId, item.Id);
        }

        private void lblEqWeapon_DoubleClick(object sender, EventArgs e)
        {
            if (m_Equipment.Hand != null)
                m_userInputHandler.UnequipItem(m_playerId, m_Equipment.Hand.Id);
        }
    }

    public class InventoryItem
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Slot { get; set; }
        public string Damage { get; set; }
        public string Speed { get; set; }
        public string Family { get; set; }

        public InventoryItem(uint id, string name, string slot, string stat1, string stat2, string family)
        {
            Id = id;
            Name = name;
            Slot = slot;
            Damage = stat1;
            Speed = stat2;
            Family = family;
        }

        public InventoryItem()
        {
            Id = 0;
            Name = Slot = Damage = Speed = Family = "";
        }
    }

    class Equipment
    {
        public InventoryItem? Hand { get; set; }

        public Equipment()
        {
            Clear();
        }

        public void Clear()
        {
            Hand = null;
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