namespace TheIdleScrollsApp
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.timerTick = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnLeaveDungeon = new System.Windows.Forms.Button();
            this.cbNextAfterWin = new System.Windows.Forms.CheckBox();
            this.btnAreaNext = new System.Windows.Forms.Button();
            this.btnAreaPrev = new System.Windows.Forms.Button();
            this.lblTimeLimit = new System.Windows.Forms.Label();
            this.lblMobName = new System.Windows.Forms.Label();
            this.lblMobHP = new System.Windows.Forms.Label();
            this.lblArea = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCharacter = new System.Windows.Forms.TabPage();
            this.btnDungeon12 = new System.Windows.Forms.Button();
            this.btnDungeon11 = new System.Windows.Forms.Button();
            this.btnDungeon10 = new System.Windows.Forms.Button();
            this.btnDungeon9 = new System.Windows.Forms.Button();
            this.btnDungeon6 = new System.Windows.Forms.Button();
            this.btnDungeon3 = new System.Windows.Forms.Button();
            this.btnDungeon7 = new System.Windows.Forms.Button();
            this.btnDungeon8 = new System.Windows.Forms.Button();
            this.btnDungeon4 = new System.Windows.Forms.Button();
            this.btnDungeon5 = new System.Windows.Forms.Button();
            this.btnDungeon1 = new System.Windows.Forms.Button();
            this.btnDungeon2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblDefEvasion = new System.Windows.Forms.Label();
            this.lblDefArmor = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.hdrRawDmg = new System.Windows.Forms.Label();
            this.hdrDps = new System.Windows.Forms.Label();
            this.lblAttackDps = new System.Windows.Forms.Label();
            this.hdrCooldown = new System.Windows.Forms.Label();
            this.lblAttackCooldown = new System.Windows.Forms.Label();
            this.lblAttackRawDmg = new System.Windows.Forms.Label();
            this.lblAttack = new System.Windows.Forms.Label();
            this.lblCharXP = new System.Windows.Forms.Label();
            this.lblCharLevel = new System.Windows.Forms.Label();
            this.lblCharName = new System.Windows.Forms.Label();
            this.tabInventory = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lblInventoryEncumbrance = new System.Windows.Forms.Label();
            this.lblInventoryDefense = new System.Windows.Forms.Label();
            this.lblInventoryOffense = new System.Windows.Forms.Label();
            this.lblEqOffHand = new System.Windows.Forms.Label();
            this.lblEqBoots = new System.Windows.Forms.Label();
            this.lblEqGloves = new System.Windows.Forms.Label();
            this.lblEqHelmet = new System.Windows.Forms.Label();
            this.lblCoins = new System.Windows.Forms.Label();
            this.lblEqChest = new System.Windows.Forms.Label();
            this.lblEqWeapon = new System.Windows.Forms.Label();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.gridInventory = new System.Windows.Forms.DataGridView();
            this.cMenuInventory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cMenuInventorySell = new System.Windows.Forms.ToolStripMenuItem();
            this.cMenuInventoryReforge = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnEquipItem = new System.Windows.Forms.Button();
            this.btnSellItem = new System.Windows.Forms.Button();
            this.btnReforgeItem = new System.Windows.Forms.Button();
            this.rtbItemDescription = new System.Windows.Forms.RichTextBox();
            this.tabAbilities = new System.Windows.Forms.TabPage();
            this.gridAbilities = new System.Windows.Forms.DataGridView();
            this.tabAchievements = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.gridAchievements = new System.Windows.Forms.DataGridView();
            this.colEarned = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAchievementCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabStats = new System.Windows.Forms.TabPage();
            this.textBoxStats = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblFooter = new System.Windows.Forms.ToolStripStatusLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblEncumbrance = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabCharacter.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabInventory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridInventory)).BeginInit();
            this.cMenuInventory.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabAbilities.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAbilities)).BeginInit();
            this.tabAchievements.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAchievements)).BeginInit();
            this.tabStats.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerTick
            // 
            this.timerTick.Enabled = true;
            this.timerTick.Tick += new System.EventHandler(this.timerTick_Tick);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 20;
            this.toolTip.ShowAlways = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer2.Panel2MinSize = 22;
            this.splitContainer2.Size = new System.Drawing.Size(1041, 561);
            this.splitContainer2.SplitterDistance = 535;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnLeaveDungeon);
            this.splitContainer1.Panel1.Controls.Add(this.cbNextAfterWin);
            this.splitContainer1.Panel1.Controls.Add(this.btnAreaNext);
            this.splitContainer1.Panel1.Controls.Add(this.btnAreaPrev);
            this.splitContainer1.Panel1.Controls.Add(this.lblTimeLimit);
            this.splitContainer1.Panel1.Controls.Add(this.lblMobName);
            this.splitContainer1.Panel1.Controls.Add(this.lblMobHP);
            this.splitContainer1.Panel1.Controls.Add(this.lblArea);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1041, 535);
            this.splitContainer1.SplitterDistance = 380;
            this.splitContainer1.TabIndex = 1;
            // 
            // btnLeaveDungeon
            // 
            this.btnLeaveDungeon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeaveDungeon.Location = new System.Drawing.Point(150, 49);
            this.btnLeaveDungeon.Name = "btnLeaveDungeon";
            this.btnLeaveDungeon.Size = new System.Drawing.Size(80, 23);
            this.btnLeaveDungeon.TabIndex = 9;
            this.btnLeaveDungeon.Text = "Leave";
            this.btnLeaveDungeon.UseVisualStyleBackColor = true;
            this.btnLeaveDungeon.Click += new System.EventHandler(this.btnLeaveDungeon_Click);
            // 
            // cbNextAfterWin
            // 
            this.cbNextAfterWin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNextAfterWin.AutoSize = true;
            this.cbNextAfterWin.Location = new System.Drawing.Point(265, 52);
            this.cbNextAfterWin.Name = "cbNextAfterWin";
            this.cbNextAfterWin.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbNextAfterWin.Size = new System.Drawing.Size(107, 19);
            this.cbNextAfterWin.TabIndex = 8;
            this.cbNextAfterWin.Text = "Go on after win";
            this.cbNextAfterWin.UseVisualStyleBackColor = true;
            this.cbNextAfterWin.CheckedChanged += new System.EventHandler(this.cbNextAfterWin_CheckedChanged);
            // 
            // btnAreaNext
            // 
            this.btnAreaNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAreaNext.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAreaNext.Location = new System.Drawing.Point(332, 9);
            this.btnAreaNext.Name = "btnAreaNext";
            this.btnAreaNext.Size = new System.Drawing.Size(40, 33);
            this.btnAreaNext.TabIndex = 7;
            this.btnAreaNext.Text = " ▶";
            this.btnAreaNext.UseVisualStyleBackColor = true;
            this.btnAreaNext.Click += new System.EventHandler(this.btnAreaNext_Click);
            // 
            // btnAreaPrev
            // 
            this.btnAreaPrev.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnAreaPrev.Location = new System.Drawing.Point(12, 9);
            this.btnAreaPrev.Name = "btnAreaPrev";
            this.btnAreaPrev.Size = new System.Drawing.Size(40, 35);
            this.btnAreaPrev.TabIndex = 6;
            this.btnAreaPrev.Text = "◀";
            this.btnAreaPrev.UseVisualStyleBackColor = true;
            this.btnAreaPrev.Click += new System.EventHandler(this.btnAreaPrev_Click);
            // 
            // lblTimeLimit
            // 
            this.lblTimeLimit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblTimeLimit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTimeLimit.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblTimeLimit.Location = new System.Drawing.Point(0, 383);
            this.lblTimeLimit.Name = "lblTimeLimit";
            this.lblTimeLimit.Size = new System.Drawing.Size(380, 73);
            this.lblTimeLimit.TabIndex = 2;
            this.lblTimeLimit.Text = "10.00\r\n██████████████";
            this.lblTimeLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMobName
            // 
            this.lblMobName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblMobName.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMobName.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblMobName.Location = new System.Drawing.Point(0, 456);
            this.lblMobName.Name = "lblMobName";
            this.lblMobName.Size = new System.Drawing.Size(380, 21);
            this.lblMobName.TabIndex = 1;
            this.lblMobName.Text = "Training Dummy (Lvl 1)";
            this.lblMobName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMobHP
            // 
            this.lblMobHP.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblMobHP.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMobHP.ForeColor = System.Drawing.Color.Red;
            this.lblMobHP.Location = new System.Drawing.Point(0, 477);
            this.lblMobHP.Name = "lblMobHP";
            this.lblMobHP.Size = new System.Drawing.Size(380, 58);
            this.lblMobHP.TabIndex = 1;
            this.lblMobHP.Text = "HP: 100 / 100\r\n████████████████";
            this.lblMobHP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblArea
            // 
            this.lblArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblArea.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblArea.Location = new System.Drawing.Point(3, 14);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(374, 23);
            this.lblArea.TabIndex = 0;
            this.lblArea.Text = "Wilderness - Level 1";
            this.lblArea.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabCharacter);
            this.tabControl1.Controls.Add(this.tabInventory);
            this.tabControl1.Controls.Add(this.tabAbilities);
            this.tabControl1.Controls.Add(this.tabAchievements);
            this.tabControl1.Controls.Add(this.tabStats);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(0, 0);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(657, 535);
            this.tabControl1.TabIndex = 0;
            // 
            // tabCharacter
            // 
            this.tabCharacter.Controls.Add(this.btnDungeon12);
            this.tabCharacter.Controls.Add(this.btnDungeon11);
            this.tabCharacter.Controls.Add(this.btnDungeon10);
            this.tabCharacter.Controls.Add(this.btnDungeon9);
            this.tabCharacter.Controls.Add(this.btnDungeon6);
            this.tabCharacter.Controls.Add(this.btnDungeon3);
            this.tabCharacter.Controls.Add(this.btnDungeon7);
            this.tabCharacter.Controls.Add(this.btnDungeon8);
            this.tabCharacter.Controls.Add(this.btnDungeon4);
            this.tabCharacter.Controls.Add(this.btnDungeon5);
            this.tabCharacter.Controls.Add(this.btnDungeon1);
            this.tabCharacter.Controls.Add(this.btnDungeon2);
            this.tabCharacter.Controls.Add(this.tableLayoutPanel1);
            this.tabCharacter.Controls.Add(this.lblCharXP);
            this.tabCharacter.Controls.Add(this.lblCharLevel);
            this.tabCharacter.Controls.Add(this.lblCharName);
            this.tabCharacter.Location = new System.Drawing.Point(4, 24);
            this.tabCharacter.Name = "tabCharacter";
            this.tabCharacter.Padding = new System.Windows.Forms.Padding(3);
            this.tabCharacter.Size = new System.Drawing.Size(649, 507);
            this.tabCharacter.TabIndex = 0;
            this.tabCharacter.Text = "Character";
            this.tabCharacter.UseVisualStyleBackColor = true;
            // 
            // btnDungeon12
            // 
            this.btnDungeon12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon12.AutoSize = true;
            this.btnDungeon12.Location = new System.Drawing.Point(405, 475);
            this.btnDungeon12.Name = "btnDungeon12";
            this.btnDungeon12.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon12.TabIndex = 19;
            this.btnDungeon12.Text = "D12";
            this.btnDungeon12.UseVisualStyleBackColor = true;
            this.btnDungeon12.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon11
            // 
            this.btnDungeon11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon11.AutoSize = true;
            this.btnDungeon11.Location = new System.Drawing.Point(220, 475);
            this.btnDungeon11.Name = "btnDungeon11";
            this.btnDungeon11.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon11.TabIndex = 18;
            this.btnDungeon11.Text = "D11";
            this.btnDungeon11.UseVisualStyleBackColor = true;
            this.btnDungeon11.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon10
            // 
            this.btnDungeon10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon10.AutoSize = true;
            this.btnDungeon10.Location = new System.Drawing.Point(33, 475);
            this.btnDungeon10.Name = "btnDungeon10";
            this.btnDungeon10.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon10.TabIndex = 17;
            this.btnDungeon10.Text = "D10";
            this.btnDungeon10.UseVisualStyleBackColor = true;
            this.btnDungeon10.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon9
            // 
            this.btnDungeon9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon9.AutoSize = true;
            this.btnDungeon9.Location = new System.Drawing.Point(405, 444);
            this.btnDungeon9.Name = "btnDungeon9";
            this.btnDungeon9.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon9.TabIndex = 16;
            this.btnDungeon9.Text = "D9";
            this.btnDungeon9.UseVisualStyleBackColor = true;
            this.btnDungeon9.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon6
            // 
            this.btnDungeon6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon6.AutoSize = true;
            this.btnDungeon6.Location = new System.Drawing.Point(220, 444);
            this.btnDungeon6.Name = "btnDungeon6";
            this.btnDungeon6.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon6.TabIndex = 15;
            this.btnDungeon6.Text = "D6";
            this.btnDungeon6.UseVisualStyleBackColor = true;
            this.btnDungeon6.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon3
            // 
            this.btnDungeon3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon3.AutoSize = true;
            this.btnDungeon3.Location = new System.Drawing.Point(33, 444);
            this.btnDungeon3.Name = "btnDungeon3";
            this.btnDungeon3.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon3.TabIndex = 14;
            this.btnDungeon3.Text = "D3";
            this.btnDungeon3.UseVisualStyleBackColor = true;
            this.btnDungeon3.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon7
            // 
            this.btnDungeon7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon7.AutoSize = true;
            this.btnDungeon7.Location = new System.Drawing.Point(405, 383);
            this.btnDungeon7.Name = "btnDungeon7";
            this.btnDungeon7.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon7.TabIndex = 12;
            this.btnDungeon7.Text = "D7";
            this.btnDungeon7.UseVisualStyleBackColor = true;
            this.btnDungeon7.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon8
            // 
            this.btnDungeon8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon8.AutoSize = true;
            this.btnDungeon8.Location = new System.Drawing.Point(405, 414);
            this.btnDungeon8.Name = "btnDungeon8";
            this.btnDungeon8.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon8.TabIndex = 13;
            this.btnDungeon8.Text = "D8";
            this.btnDungeon8.UseVisualStyleBackColor = true;
            this.btnDungeon8.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon4
            // 
            this.btnDungeon4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon4.AutoSize = true;
            this.btnDungeon4.Location = new System.Drawing.Point(220, 383);
            this.btnDungeon4.Name = "btnDungeon4";
            this.btnDungeon4.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon4.TabIndex = 10;
            this.btnDungeon4.Text = "D4";
            this.btnDungeon4.UseVisualStyleBackColor = true;
            this.btnDungeon4.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon5
            // 
            this.btnDungeon5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon5.AutoSize = true;
            this.btnDungeon5.Location = new System.Drawing.Point(220, 414);
            this.btnDungeon5.Name = "btnDungeon5";
            this.btnDungeon5.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon5.TabIndex = 11;
            this.btnDungeon5.Text = "D5";
            this.btnDungeon5.UseVisualStyleBackColor = true;
            this.btnDungeon5.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon1
            // 
            this.btnDungeon1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon1.AutoSize = true;
            this.btnDungeon1.Location = new System.Drawing.Point(33, 383);
            this.btnDungeon1.Name = "btnDungeon1";
            this.btnDungeon1.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon1.TabIndex = 1;
            this.btnDungeon1.Text = "D1";
            this.btnDungeon1.UseVisualStyleBackColor = true;
            this.btnDungeon1.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // btnDungeon2
            // 
            this.btnDungeon2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDungeon2.AutoSize = true;
            this.btnDungeon2.Location = new System.Drawing.Point(33, 414);
            this.btnDungeon2.Name = "btnDungeon2";
            this.btnDungeon2.Size = new System.Drawing.Size(168, 25);
            this.btnDungeon2.TabIndex = 9;
            this.btnDungeon2.Text = "D2";
            this.btnDungeon2.UseVisualStyleBackColor = true;
            this.btnDungeon2.Click += new System.EventHandler(this.btnDungeon_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.tableLayoutPanel1.Controls.Add(this.lblDefEvasion, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblDefArmor, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.hdrRawDmg, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.hdrDps, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblAttackDps, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.hdrCooldown, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblAttackCooldown, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblAttackRawDmg, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblAttack, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblEncumbrance, 1, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 157);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(20, 1, 20, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(643, 190);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // lblDefEvasion
            // 
            this.lblDefEvasion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDefEvasion.Location = new System.Drawing.Point(228, 122);
            this.lblDefEvasion.Name = "lblDefEvasion";
            this.lblDefEvasion.Size = new System.Drawing.Size(125, 33);
            this.lblDefEvasion.TabIndex = 7;
            this.lblDefEvasion.Text = "1.0";
            this.lblDefEvasion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDefArmor
            // 
            this.lblDefArmor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDefArmor.Location = new System.Drawing.Point(228, 88);
            this.lblDefArmor.Name = "lblDefArmor";
            this.lblDefArmor.Size = new System.Drawing.Size(125, 33);
            this.lblDefArmor.TabIndex = 6;
            this.lblDefArmor.Text = "0.0";
            this.lblDefArmor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(24, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(197, 33);
            this.label3.TabIndex = 7;
            this.label3.Text = "Evasion";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(24, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(197, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "Armor";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hdrRawDmg
            // 
            this.hdrRawDmg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hdrRawDmg.Location = new System.Drawing.Point(228, 1);
            this.hdrRawDmg.Name = "hdrRawDmg";
            this.hdrRawDmg.Size = new System.Drawing.Size(125, 31);
            this.hdrRawDmg.TabIndex = 0;
            this.hdrRawDmg.Text = "Raw Damage";
            this.hdrRawDmg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hdrDps
            // 
            this.hdrDps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hdrDps.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.hdrDps.Location = new System.Drawing.Point(360, 1);
            this.hdrDps.Name = "hdrDps";
            this.hdrDps.Size = new System.Drawing.Size(125, 31);
            this.hdrDps.TabIndex = 0;
            this.hdrDps.Text = "DPS";
            this.hdrDps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttackDps
            // 
            this.lblAttackDps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttackDps.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAttackDps.Location = new System.Drawing.Point(360, 33);
            this.lblAttackDps.Name = "lblAttackDps";
            this.lblAttackDps.Size = new System.Drawing.Size(125, 33);
            this.lblAttackDps.TabIndex = 0;
            this.lblAttackDps.Text = "1.0";
            this.lblAttackDps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hdrCooldown
            // 
            this.hdrCooldown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hdrCooldown.Location = new System.Drawing.Point(492, 1);
            this.hdrCooldown.Name = "hdrCooldown";
            this.hdrCooldown.Size = new System.Drawing.Size(127, 31);
            this.hdrCooldown.TabIndex = 0;
            this.hdrCooldown.Text = "Cooldown";
            this.hdrCooldown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttackCooldown
            // 
            this.lblAttackCooldown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttackCooldown.Location = new System.Drawing.Point(492, 33);
            this.lblAttackCooldown.Name = "lblAttackCooldown";
            this.lblAttackCooldown.Size = new System.Drawing.Size(127, 33);
            this.lblAttackCooldown.TabIndex = 0;
            this.lblAttackCooldown.Text = "1.0";
            this.lblAttackCooldown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttackRawDmg
            // 
            this.lblAttackRawDmg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttackRawDmg.Location = new System.Drawing.Point(228, 33);
            this.lblAttackRawDmg.Name = "lblAttackRawDmg";
            this.lblAttackRawDmg.Size = new System.Drawing.Size(125, 33);
            this.lblAttackRawDmg.TabIndex = 0;
            this.lblAttackRawDmg.Text = "1.0";
            this.lblAttackRawDmg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttack
            // 
            this.lblAttack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttack.Location = new System.Drawing.Point(24, 33);
            this.lblAttack.Name = "lblAttack";
            this.lblAttack.Size = new System.Drawing.Size(197, 33);
            this.lblAttack.TabIndex = 0;
            this.lblAttack.Text = "Attack";
            this.lblAttack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCharXP
            // 
            this.lblCharXP.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCharXP.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCharXP.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblCharXP.Location = new System.Drawing.Point(3, 85);
            this.lblCharXP.Name = "lblCharXP";
            this.lblCharXP.Size = new System.Drawing.Size(643, 72);
            this.lblCharXP.TabIndex = 3;
            this.lblCharXP.Text = "XP: 0 / 500\r\n▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱▱";
            this.lblCharXP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCharLevel
            // 
            this.lblCharLevel.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCharLevel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCharLevel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblCharLevel.Location = new System.Drawing.Point(3, 53);
            this.lblCharLevel.Name = "lblCharLevel";
            this.lblCharLevel.Size = new System.Drawing.Size(643, 32);
            this.lblCharLevel.TabIndex = 2;
            this.lblCharLevel.Text = "Level 1";
            this.lblCharLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCharName
            // 
            this.lblCharName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCharName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCharName.Location = new System.Drawing.Point(3, 3);
            this.lblCharName.Name = "lblCharName";
            this.lblCharName.Size = new System.Drawing.Size(643, 50);
            this.lblCharName.TabIndex = 0;
            this.lblCharName.Text = "Leeroy ";
            this.lblCharName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabInventory
            // 
            this.tabInventory.Controls.Add(this.splitContainer3);
            this.tabInventory.Location = new System.Drawing.Point(4, 24);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Padding = new System.Windows.Forms.Padding(3);
            this.tabInventory.Size = new System.Drawing.Size(649, 507);
            this.tabInventory.TabIndex = 2;
            this.tabInventory.Text = "Inventory";
            this.tabInventory.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lblInventoryEncumbrance);
            this.splitContainer3.Panel1.Controls.Add(this.lblInventoryDefense);
            this.splitContainer3.Panel1.Controls.Add(this.lblInventoryOffense);
            this.splitContainer3.Panel1.Controls.Add(this.lblEqOffHand);
            this.splitContainer3.Panel1.Controls.Add(this.lblEqBoots);
            this.splitContainer3.Panel1.Controls.Add(this.lblEqGloves);
            this.splitContainer3.Panel1.Controls.Add(this.lblEqHelmet);
            this.splitContainer3.Panel1.Controls.Add(this.lblCoins);
            this.splitContainer3.Panel1.Controls.Add(this.lblEqChest);
            this.splitContainer3.Panel1.Controls.Add(this.lblEqWeapon);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Size = new System.Drawing.Size(643, 501);
            this.splitContainer3.SplitterDistance = 144;
            this.splitContainer3.SplitterWidth = 1;
            this.splitContainer3.TabIndex = 6;
            // 
            // lblInventoryEncumbrance
            // 
            this.lblInventoryEncumbrance.Location = new System.Drawing.Point(517, 41);
            this.lblInventoryEncumbrance.Name = "lblInventoryEncumbrance";
            this.lblInventoryEncumbrance.Size = new System.Drawing.Size(108, 15);
            this.lblInventoryEncumbrance.TabIndex = 12;
            this.lblInventoryEncumbrance.Text = "Encumbrance: 99%\r\n";
            this.lblInventoryEncumbrance.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblInventoryDefense
            // 
            this.lblInventoryDefense.Location = new System.Drawing.Point(517, 8);
            this.lblInventoryDefense.Name = "lblInventoryDefense";
            this.lblInventoryDefense.Size = new System.Drawing.Size(108, 30);
            this.lblInventoryDefense.TabIndex = 11;
            this.lblInventoryDefense.Text = "Armor: 999999\r\nEvasion: 999999\r\n";
            this.lblInventoryDefense.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblInventoryOffense
            // 
            this.lblInventoryOffense.AutoSize = true;
            this.lblInventoryOffense.Location = new System.Drawing.Point(13, 8);
            this.lblInventoryOffense.Name = "lblInventoryOffense";
            this.lblInventoryOffense.Size = new System.Drawing.Size(97, 30);
            this.lblInventoryOffense.TabIndex = 10;
            this.lblInventoryOffense.Text = "Damage: 999.999\r\nDPS:         999,999\r\n";
            // 
            // lblEqOffHand
            // 
            this.lblEqOffHand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEqOffHand.Location = new System.Drawing.Point(425, 59);
            this.lblEqOffHand.Name = "lblEqOffHand";
            this.lblEqOffHand.Size = new System.Drawing.Size(200, 23);
            this.lblEqOffHand.TabIndex = 9;
            this.lblEqOffHand.Tag = "Weapon/Shield";
            this.lblEqOffHand.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEqBoots
            // 
            this.lblEqBoots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEqBoots.Location = new System.Drawing.Point(322, 106);
            this.lblEqBoots.Name = "lblEqBoots";
            this.lblEqBoots.Size = new System.Drawing.Size(200, 23);
            this.lblEqBoots.TabIndex = 8;
            this.lblEqBoots.Tag = "Boots";
            this.lblEqBoots.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEqGloves
            // 
            this.lblEqGloves.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEqGloves.Location = new System.Drawing.Point(116, 106);
            this.lblEqGloves.Name = "lblEqGloves";
            this.lblEqGloves.Size = new System.Drawing.Size(200, 23);
            this.lblEqGloves.TabIndex = 7;
            this.lblEqGloves.Tag = "Gloves";
            this.lblEqGloves.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEqHelmet
            // 
            this.lblEqHelmet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEqHelmet.Location = new System.Drawing.Point(219, 12);
            this.lblEqHelmet.Name = "lblEqHelmet";
            this.lblEqHelmet.Size = new System.Drawing.Size(200, 23);
            this.lblEqHelmet.TabIndex = 6;
            this.lblEqHelmet.Tag = "Helmet";
            this.lblEqHelmet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCoins
            // 
            this.lblCoins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCoins.Location = new System.Drawing.Point(525, 127);
            this.lblCoins.Margin = new System.Windows.Forms.Padding(0);
            this.lblCoins.Name = "lblCoins";
            this.lblCoins.Size = new System.Drawing.Size(116, 15);
            this.lblCoins.TabIndex = 0;
            this.lblCoins.Text = "XX Coins";
            this.lblCoins.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblEqChest
            // 
            this.lblEqChest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEqChest.Location = new System.Drawing.Point(219, 59);
            this.lblEqChest.Name = "lblEqChest";
            this.lblEqChest.Size = new System.Drawing.Size(200, 23);
            this.lblEqChest.TabIndex = 5;
            this.lblEqChest.Tag = "Chest Armor";
            this.lblEqChest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEqWeapon
            // 
            this.lblEqWeapon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEqWeapon.Location = new System.Drawing.Point(13, 59);
            this.lblEqWeapon.Name = "lblEqWeapon";
            this.lblEqWeapon.Size = new System.Drawing.Size(200, 23);
            this.lblEqWeapon.TabIndex = 3;
            this.lblEqWeapon.Tag = "Weapon";
            this.lblEqWeapon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.gridInventory);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer4.Panel2.Controls.Add(this.rtbItemDescription);
            this.splitContainer4.Size = new System.Drawing.Size(643, 356);
            this.splitContainer4.SplitterDistance = 316;
            this.splitContainer4.TabIndex = 1;
            // 
            // gridInventory
            // 
            this.gridInventory.AllowUserToAddRows = false;
            this.gridInventory.AllowUserToDeleteRows = false;
            this.gridInventory.AllowUserToResizeRows = false;
            this.gridInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.gridInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridInventory.ContextMenuStrip = this.cMenuInventory;
            this.gridInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridInventory.Location = new System.Drawing.Point(0, 0);
            this.gridInventory.Name = "gridInventory";
            this.gridInventory.ReadOnly = true;
            this.gridInventory.RowHeadersVisible = false;
            this.gridInventory.RowTemplate.Height = 25;
            this.gridInventory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridInventory.Size = new System.Drawing.Size(316, 356);
            this.gridInventory.TabIndex = 0;
            this.gridInventory.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.gridInventory_CellContextMenuStripNeeded);
            this.gridInventory.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridInventory_CellMouseDoubleClick);
            this.gridInventory.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.gridInventory_RowPrePaint);
            this.gridInventory.SelectionChanged += new System.EventHandler(this.gridInventory_SelectionChanged);
            this.gridInventory.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gridInventory_KeyUp);
            // 
            // cMenuInventory
            // 
            this.cMenuInventory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cMenuInventorySell,
            this.cMenuInventoryReforge});
            this.cMenuInventory.Name = "cMenuInventory";
            this.cMenuInventory.Size = new System.Drawing.Size(116, 48);
            this.cMenuInventory.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.cMenuInventory_Closed);
            // 
            // cMenuInventorySell
            // 
            this.cMenuInventorySell.Name = "cMenuInventorySell";
            this.cMenuInventorySell.Size = new System.Drawing.Size(115, 22);
            this.cMenuInventorySell.Text = "Sell";
            // 
            // cMenuInventoryReforge
            // 
            this.cMenuInventoryReforge.Name = "cMenuInventoryReforge";
            this.cMenuInventoryReforge.Size = new System.Drawing.Size(115, 22);
            this.cMenuInventoryReforge.Text = "Reforge";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnEquipItem);
            this.flowLayoutPanel1.Controls.Add(this.btnSellItem);
            this.flowLayoutPanel1.Controls.Add(this.btnReforgeItem);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 325);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(323, 31);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnEquipItem
            // 
            this.btnEquipItem.Location = new System.Drawing.Point(3, 3);
            this.btnEquipItem.Name = "btnEquipItem";
            this.btnEquipItem.Size = new System.Drawing.Size(100, 23);
            this.btnEquipItem.TabIndex = 1;
            this.btnEquipItem.Text = "Equip";
            this.btnEquipItem.UseVisualStyleBackColor = true;
            // 
            // btnSellItem
            // 
            this.btnSellItem.Location = new System.Drawing.Point(109, 3);
            this.btnSellItem.Name = "btnSellItem";
            this.btnSellItem.Size = new System.Drawing.Size(100, 23);
            this.btnSellItem.TabIndex = 2;
            this.btnSellItem.Text = "Sell";
            this.btnSellItem.UseVisualStyleBackColor = true;
            // 
            // btnReforgeItem
            // 
            this.btnReforgeItem.Location = new System.Drawing.Point(215, 3);
            this.btnReforgeItem.Name = "btnReforgeItem";
            this.btnReforgeItem.Size = new System.Drawing.Size(100, 23);
            this.btnReforgeItem.TabIndex = 3;
            this.btnReforgeItem.Text = "Reforge";
            this.btnReforgeItem.UseVisualStyleBackColor = true;
            // 
            // rtbItemDescription
            // 
            this.rtbItemDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbItemDescription.Location = new System.Drawing.Point(0, 0);
            this.rtbItemDescription.Name = "rtbItemDescription";
            this.rtbItemDescription.ReadOnly = true;
            this.rtbItemDescription.Size = new System.Drawing.Size(323, 356);
            this.rtbItemDescription.TabIndex = 0;
            this.rtbItemDescription.Text = "";
            // 
            // tabAbilities
            // 
            this.tabAbilities.Controls.Add(this.gridAbilities);
            this.tabAbilities.Location = new System.Drawing.Point(4, 24);
            this.tabAbilities.Name = "tabAbilities";
            this.tabAbilities.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbilities.Size = new System.Drawing.Size(649, 507);
            this.tabAbilities.TabIndex = 3;
            this.tabAbilities.Text = "Abilities";
            this.tabAbilities.UseVisualStyleBackColor = true;
            // 
            // gridAbilities
            // 
            this.gridAbilities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAbilities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAbilities.Location = new System.Drawing.Point(3, 3);
            this.gridAbilities.Name = "gridAbilities";
            this.gridAbilities.ReadOnly = true;
            this.gridAbilities.RowHeadersVisible = false;
            this.gridAbilities.RowTemplate.Height = 25;
            this.gridAbilities.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAbilities.Size = new System.Drawing.Size(643, 501);
            this.gridAbilities.TabIndex = 0;
            // 
            // tabAchievements
            // 
            this.tabAchievements.Controls.Add(this.tableLayoutPanel2);
            this.tabAchievements.Location = new System.Drawing.Point(4, 24);
            this.tabAchievements.Name = "tabAchievements";
            this.tabAchievements.Size = new System.Drawing.Size(649, 507);
            this.tabAchievements.TabIndex = 4;
            this.tabAchievements.Text = "Achievements";
            this.tabAchievements.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.gridAchievements, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblAchievementCount, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(649, 507);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // gridAchievements
            // 
            this.gridAchievements.AllowUserToAddRows = false;
            this.gridAchievements.AllowUserToDeleteRows = false;
            this.gridAchievements.AllowUserToResizeRows = false;
            this.gridAchievements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAchievements.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEarned,
            this.Title,
            this.colDescription});
            this.tableLayoutPanel2.SetColumnSpan(this.gridAchievements, 2);
            this.gridAchievements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAchievements.Location = new System.Drawing.Point(3, 38);
            this.gridAchievements.Name = "gridAchievements";
            this.gridAchievements.ReadOnly = true;
            this.gridAchievements.RowHeadersVisible = false;
            this.gridAchievements.RowTemplate.Height = 25;
            this.gridAchievements.Size = new System.Drawing.Size(643, 466);
            this.gridAchievements.TabIndex = 0;
            this.gridAchievements.SelectionChanged += new System.EventHandler(this.gridAchievements_SelectionChanged);
            // 
            // colEarned
            // 
            this.colEarned.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colEarned.HeaderText = "     ";
            this.colEarned.Name = "colEarned";
            this.colEarned.ReadOnly = true;
            this.colEarned.Width = 25;
            // 
            // Title
            // 
            this.Title.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Title.HeaderText = "Title";
            this.Title.Name = "Title";
            this.Title.ReadOnly = true;
            this.Title.Width = 54;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            // 
            // lblAchievementCount
            // 
            this.lblAchievementCount.AutoSize = true;
            this.lblAchievementCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAchievementCount.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAchievementCount.Location = new System.Drawing.Point(327, 0);
            this.lblAchievementCount.Name = "lblAchievementCount";
            this.lblAchievementCount.Size = new System.Drawing.Size(319, 35);
            this.lblAchievementCount.TabIndex = 1;
            this.lblAchievementCount.Text = "50 / 50 Completed";
            this.lblAchievementCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(318, 35);
            this.label2.TabIndex = 2;
            this.label2.Text = "Achievements";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabStats
            // 
            this.tabStats.Controls.Add(this.textBoxStats);
            this.tabStats.Location = new System.Drawing.Point(4, 24);
            this.tabStats.Name = "tabStats";
            this.tabStats.Padding = new System.Windows.Forms.Padding(3);
            this.tabStats.Size = new System.Drawing.Size(649, 507);
            this.tabStats.TabIndex = 1;
            this.tabStats.Text = "Stats";
            this.tabStats.UseVisualStyleBackColor = true;
            // 
            // textBoxStats
            // 
            this.textBoxStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxStats.Location = new System.Drawing.Point(3, 3);
            this.textBoxStats.MaxLength = 3276700;
            this.textBoxStats.Multiline = true;
            this.textBoxStats.Name = "textBoxStats";
            this.textBoxStats.Size = new System.Drawing.Size(643, 501);
            this.textBoxStats.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblFooter});
            this.statusStrip1.Location = new System.Drawing.Point(0, 3);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1041, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblFooter
            // 
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.lblFooter.Size = new System.Drawing.Size(118, 17);
            this.lblFooter.Text = "toolStripStatusLabel1";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(24, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 33);
            this.label4.TabIndex = 8;
            this.label4.Text = "Encumbrance";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEncumbrance
            // 
            this.lblEncumbrance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEncumbrance.Location = new System.Drawing.Point(228, 156);
            this.lblEncumbrance.Name = "lblEncumbrance";
            this.lblEncumbrance.Size = new System.Drawing.Size(125, 33);
            this.lblEncumbrance.TabIndex = 9;
            this.lblEncumbrance.Text = "1.0";
            this.lblEncumbrance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1041, 561);
            this.Controls.Add(this.splitContainer2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1050, 600);
            this.Name = "MainWindow";
            this.Text = "The Idle Scrolls";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabCharacter.ResumeLayout(false);
            this.tabCharacter.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridInventory)).EndInit();
            this.cMenuInventory.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tabAbilities.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridAbilities)).EndInit();
            this.tabAchievements.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAchievements)).EndInit();
            this.tabStats.ResumeLayout(false);
            this.tabStats.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerTick;
        private ToolTip toolTip;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer1;
        private CheckBox cbNextAfterWin;
        private Button btnAreaNext;
        private Button btnAreaPrev;
        private Label lblTimeLimit;
        private Label lblMobName;
        private Label lblMobHP;
        private Label lblArea;
        private TabControl tabControl1;
        private TabPage tabCharacter;
        private Button btnDungeon1;
        private Button btnDungeon2;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblDefEvasion;
        private Label lblDefArmor;
        private Label label3;
        private Label label1;
        private Label hdrRawDmg;
        private Label hdrDps;
        private Label lblAttackDps;
        private Label hdrCooldown;
        private Label lblAttackCooldown;
        private Label lblAttackRawDmg;
        private Label lblAttack;
        private Label lblCharXP;
        private Label lblCharLevel;
        private Label lblCharName;
        private TabPage tabInventory;
        private Label lblEqChest;
        private Label lblEqWeapon;
        private DataGridView gridInventory;
        private TabPage tabAbilities;
        private DataGridView gridAbilities;
        private TabPage tabAchievements;
        private DataGridView gridAchievements;
        private DataGridViewCheckBoxColumn colEarned;
        private DataGridViewTextBoxColumn Title;
        private DataGridViewTextBoxColumn colDescription;
        private TabPage tabStats;
        private TextBox textBoxStats;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblFooter;
        private TableLayoutPanel tableLayoutPanel2;
        private Label lblAchievementCount;
        private Label label2;
        private Button btnLeaveDungeon;
        private SplitContainer splitContainer3;
        private Button btnDungeon9;
        private Button btnDungeon6;
        private Button btnDungeon3;
        private Button btnDungeon7;
        private Button btnDungeon8;
        private Button btnDungeon4;
        private Button btnDungeon5;
        private Label lblCoins;
        private ContextMenuStrip cMenuInventory;
        private ToolStripMenuItem cMenuInventorySell;
        private Label lblEqHelmet;
        private Label lblEqBoots;
        private Label lblEqGloves;
        private ToolStripMenuItem cMenuInventoryReforge;
        private Button btnDungeon12;
        private Button btnDungeon11;
        private Button btnDungeon10;
        private Label lblEqOffHand;
        private SplitContainer splitContainer4;
        private RichTextBox rtbItemDescription;
        private Label lblInventoryDefense;
        private Label lblInventoryOffense;
        private Button btnEquipItem;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnSellItem;
        private Button btnReforgeItem;
        private Label lblInventoryEncumbrance;
        private Label label4;
        private Label lblEncumbrance;
    }
}