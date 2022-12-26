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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbNextAfterWin = new System.Windows.Forms.CheckBox();
            this.btnAreaNext = new System.Windows.Forms.Button();
            this.btnAreaPrev = new System.Windows.Forms.Button();
            this.lblTimeLimit = new System.Windows.Forms.Label();
            this.lblMobName = new System.Windows.Forms.Label();
            this.lblMobHP = new System.Windows.Forms.Label();
            this.lblArea = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCharacter = new System.Windows.Forms.TabPage();
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
            this.lblEqArmor = new System.Windows.Forms.Label();
            this.hdrEqArmor = new System.Windows.Forms.Label();
            this.lblEqWeapon = new System.Windows.Forms.Label();
            this.hdrEqWeapon = new System.Windows.Forms.Label();
            this.gridInventory = new System.Windows.Forms.DataGridView();
            this.tabAbilities = new System.Windows.Forms.TabPage();
            this.gridAbilities = new System.Windows.Forms.DataGridView();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.timerTick = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabCharacter.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabInventory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridInventory)).BeginInit();
            this.tabAbilities.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAbilities)).BeginInit();
            this.tabLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
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
            this.splitContainer1.Size = new System.Drawing.Size(772, 366);
            this.splitContainer1.SplitterDistance = 357;
            this.splitContainer1.TabIndex = 0;
            // 
            // cbNextAfterWin
            // 
            this.cbNextAfterWin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbNextAfterWin.AutoSize = true;
            this.cbNextAfterWin.Location = new System.Drawing.Point(273, 40);
            this.cbNextAfterWin.Name = "cbNextAfterWin";
            this.cbNextAfterWin.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbNextAfterWin.Size = new System.Drawing.Size(79, 19);
            this.cbNextAfterWin.TabIndex = 8;
            this.cbNextAfterWin.Text = "Auto Proc";
            this.cbNextAfterWin.UseVisualStyleBackColor = true;
            this.cbNextAfterWin.CheckedChanged += new System.EventHandler(this.cbNextAfterWin_CheckedChanged);
            // 
            // btnAreaNext
            // 
            this.btnAreaNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAreaNext.Location = new System.Drawing.Point(312, 11);
            this.btnAreaNext.Name = "btnAreaNext";
            this.btnAreaNext.Size = new System.Drawing.Size(40, 23);
            this.btnAreaNext.TabIndex = 7;
            this.btnAreaNext.Text = "==>";
            this.btnAreaNext.UseVisualStyleBackColor = true;
            this.btnAreaNext.Click += new System.EventHandler(this.btnAreaNext_Click);
            // 
            // btnAreaPrev
            // 
            this.btnAreaPrev.Location = new System.Drawing.Point(12, 9);
            this.btnAreaPrev.Name = "btnAreaPrev";
            this.btnAreaPrev.Size = new System.Drawing.Size(40, 23);
            this.btnAreaPrev.TabIndex = 6;
            this.btnAreaPrev.Text = "<==";
            this.btnAreaPrev.UseVisualStyleBackColor = true;
            this.btnAreaPrev.Click += new System.EventHandler(this.btnAreaPrev_Click);
            // 
            // lblTimeLimit
            // 
            this.lblTimeLimit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTimeLimit.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTimeLimit.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblTimeLimit.Location = new System.Drawing.Point(3, 245);
            this.lblTimeLimit.Name = "lblTimeLimit";
            this.lblTimeLimit.Size = new System.Drawing.Size(349, 42);
            this.lblTimeLimit.TabIndex = 2;
            this.lblTimeLimit.Text = "10.00\r\n##############################";
            this.lblTimeLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMobName
            // 
            this.lblMobName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMobName.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMobName.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblMobName.Location = new System.Drawing.Point(5, 299);
            this.lblMobName.Name = "lblMobName";
            this.lblMobName.Size = new System.Drawing.Size(349, 21);
            this.lblMobName.TabIndex = 1;
            this.lblMobName.Text = "Training Dummy (Lvl 1)";
            this.lblMobName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMobHP
            // 
            this.lblMobHP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMobHP.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMobHP.ForeColor = System.Drawing.Color.Red;
            this.lblMobHP.Location = new System.Drawing.Point(3, 320);
            this.lblMobHP.Name = "lblMobHP";
            this.lblMobHP.Size = new System.Drawing.Size(349, 37);
            this.lblMobHP.TabIndex = 1;
            this.lblMobHP.Text = "HP: 100 / 100\r\n###################################";
            this.lblMobHP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblArea
            // 
            this.lblArea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblArea.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblArea.Location = new System.Drawing.Point(3, 9);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(349, 23);
            this.lblArea.TabIndex = 0;
            this.lblArea.Text = "Wilderness - Level 1";
            this.lblArea.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabCharacter);
            this.tabControl1.Controls.Add(this.tabInventory);
            this.tabControl1.Controls.Add(this.tabAbilities);
            this.tabControl1.Controls.Add(this.tabLog);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(405, 360);
            this.tabControl1.TabIndex = 0;
            // 
            // tabCharacter
            // 
            this.tabCharacter.Controls.Add(this.tableLayoutPanel1);
            this.tabCharacter.Controls.Add(this.lblCharXP);
            this.tabCharacter.Controls.Add(this.lblCharLevel);
            this.tabCharacter.Controls.Add(this.lblCharName);
            this.tabCharacter.Location = new System.Drawing.Point(4, 24);
            this.tabCharacter.Name = "tabCharacter";
            this.tabCharacter.Padding = new System.Windows.Forms.Padding(3);
            this.tabCharacter.Size = new System.Drawing.Size(397, 332);
            this.tabCharacter.TabIndex = 0;
            this.tabCharacter.Text = "Character";
            this.tabCharacter.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
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
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 109);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(385, 164);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // lblDefEvasion
            // 
            this.lblDefEvasion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDefEvasion.Location = new System.Drawing.Point(101, 126);
            this.lblDefEvasion.Name = "lblDefEvasion";
            this.lblDefEvasion.Size = new System.Drawing.Size(90, 37);
            this.lblDefEvasion.TabIndex = 7;
            this.lblDefEvasion.Text = "1.0";
            this.lblDefEvasion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDefArmor
            // 
            this.lblDefArmor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDefArmor.Location = new System.Drawing.Point(101, 90);
            this.lblDefArmor.Name = "lblDefArmor";
            this.lblDefArmor.Size = new System.Drawing.Size(90, 35);
            this.lblDefArmor.TabIndex = 6;
            this.lblDefArmor.Text = "0.0";
            this.lblDefArmor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(4, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 37);
            this.label3.TabIndex = 7;
            this.label3.Text = "Evasion";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(4, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 35);
            this.label1.TabIndex = 6;
            this.label1.Text = "Armor";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hdrRawDmg
            // 
            this.hdrRawDmg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hdrRawDmg.Location = new System.Drawing.Point(101, 1);
            this.hdrRawDmg.Name = "hdrRawDmg";
            this.hdrRawDmg.Size = new System.Drawing.Size(90, 31);
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
            this.hdrDps.Location = new System.Drawing.Point(198, 1);
            this.hdrDps.Name = "hdrDps";
            this.hdrDps.Size = new System.Drawing.Size(90, 31);
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
            this.lblAttackDps.Location = new System.Drawing.Point(198, 33);
            this.lblAttackDps.Name = "lblAttackDps";
            this.lblAttackDps.Size = new System.Drawing.Size(90, 35);
            this.lblAttackDps.TabIndex = 0;
            this.lblAttackDps.Text = "1.0";
            this.lblAttackDps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // hdrCooldown
            // 
            this.hdrCooldown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hdrCooldown.Location = new System.Drawing.Point(295, 1);
            this.hdrCooldown.Name = "hdrCooldown";
            this.hdrCooldown.Size = new System.Drawing.Size(86, 31);
            this.hdrCooldown.TabIndex = 0;
            this.hdrCooldown.Text = "Cooldown";
            this.hdrCooldown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttackCooldown
            // 
            this.lblAttackCooldown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttackCooldown.Location = new System.Drawing.Point(295, 33);
            this.lblAttackCooldown.Name = "lblAttackCooldown";
            this.lblAttackCooldown.Size = new System.Drawing.Size(86, 35);
            this.lblAttackCooldown.TabIndex = 0;
            this.lblAttackCooldown.Text = "1.0";
            this.lblAttackCooldown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttackRawDmg
            // 
            this.lblAttackRawDmg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttackRawDmg.Location = new System.Drawing.Point(101, 33);
            this.lblAttackRawDmg.Name = "lblAttackRawDmg";
            this.lblAttackRawDmg.Size = new System.Drawing.Size(90, 35);
            this.lblAttackRawDmg.TabIndex = 0;
            this.lblAttackRawDmg.Text = "1.0";
            this.lblAttackRawDmg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttack
            // 
            this.lblAttack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAttack.Location = new System.Drawing.Point(4, 33);
            this.lblAttack.Name = "lblAttack";
            this.lblAttack.Size = new System.Drawing.Size(90, 35);
            this.lblAttack.TabIndex = 0;
            this.lblAttack.Text = "Attack";
            this.lblAttack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCharXP
            // 
            this.lblCharXP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCharXP.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCharXP.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblCharXP.Location = new System.Drawing.Point(6, 66);
            this.lblCharXP.Name = "lblCharXP";
            this.lblCharXP.Size = new System.Drawing.Size(385, 21);
            this.lblCharXP.TabIndex = 3;
            this.lblCharXP.Text = "XP: 0 / 500";
            this.lblCharXP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCharLevel
            // 
            this.lblCharLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCharLevel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCharLevel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblCharLevel.Location = new System.Drawing.Point(6, 33);
            this.lblCharLevel.Name = "lblCharLevel";
            this.lblCharLevel.Size = new System.Drawing.Size(385, 21);
            this.lblCharLevel.TabIndex = 2;
            this.lblCharLevel.Text = "Level 1";
            this.lblCharLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCharName
            // 
            this.lblCharName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCharName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCharName.Location = new System.Drawing.Point(6, 3);
            this.lblCharName.Name = "lblCharName";
            this.lblCharName.Size = new System.Drawing.Size(385, 30);
            this.lblCharName.TabIndex = 0;
            this.lblCharName.Text = "Leeroy ";
            this.lblCharName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabInventory
            // 
            this.tabInventory.Controls.Add(this.lblEqArmor);
            this.tabInventory.Controls.Add(this.hdrEqArmor);
            this.tabInventory.Controls.Add(this.lblEqWeapon);
            this.tabInventory.Controls.Add(this.hdrEqWeapon);
            this.tabInventory.Controls.Add(this.gridInventory);
            this.tabInventory.Location = new System.Drawing.Point(4, 24);
            this.tabInventory.Name = "tabInventory";
            this.tabInventory.Padding = new System.Windows.Forms.Padding(3);
            this.tabInventory.Size = new System.Drawing.Size(397, 332);
            this.tabInventory.TabIndex = 2;
            this.tabInventory.Text = "Inventory";
            this.tabInventory.UseVisualStyleBackColor = true;
            // 
            // lblEqArmor
            // 
            this.lblEqArmor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEqArmor.Location = new System.Drawing.Point(254, 30);
            this.lblEqArmor.Name = "lblEqArmor";
            this.lblEqArmor.Size = new System.Drawing.Size(106, 23);
            this.lblEqArmor.TabIndex = 5;
            this.lblEqArmor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEqArmor.DoubleClick += new System.EventHandler(this.lblEqArmor_DoubleClick);
            // 
            // hdrEqArmor
            // 
            this.hdrEqArmor.AutoSize = true;
            this.hdrEqArmor.Location = new System.Drawing.Point(207, 34);
            this.hdrEqArmor.Name = "hdrEqArmor";
            this.hdrEqArmor.Size = new System.Drawing.Size(41, 15);
            this.hdrEqArmor.TabIndex = 4;
            this.hdrEqArmor.Text = "Armor";
            // 
            // lblEqWeapon
            // 
            this.lblEqWeapon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEqWeapon.Location = new System.Drawing.Point(63, 30);
            this.lblEqWeapon.Name = "lblEqWeapon";
            this.lblEqWeapon.Size = new System.Drawing.Size(106, 23);
            this.lblEqWeapon.TabIndex = 3;
            this.lblEqWeapon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEqWeapon.DoubleClick += new System.EventHandler(this.lblEqWeapon_DoubleClick);
            // 
            // hdrEqWeapon
            // 
            this.hdrEqWeapon.AutoSize = true;
            this.hdrEqWeapon.Location = new System.Drawing.Point(6, 34);
            this.hdrEqWeapon.Name = "hdrEqWeapon";
            this.hdrEqWeapon.Size = new System.Drawing.Size(51, 15);
            this.hdrEqWeapon.TabIndex = 1;
            this.hdrEqWeapon.Text = "Weapon";
            // 
            // gridInventory
            // 
            this.gridInventory.AllowUserToAddRows = false;
            this.gridInventory.AllowUserToDeleteRows = false;
            this.gridInventory.AllowUserToResizeRows = false;
            this.gridInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.gridInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridInventory.Location = new System.Drawing.Point(6, 84);
            this.gridInventory.MultiSelect = false;
            this.gridInventory.Name = "gridInventory";
            this.gridInventory.ReadOnly = true;
            this.gridInventory.RowHeadersVisible = false;
            this.gridInventory.RowTemplate.Height = 25;
            this.gridInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridInventory.Size = new System.Drawing.Size(385, 242);
            this.gridInventory.TabIndex = 0;
            this.gridInventory.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridInventory_CellMouseDoubleClick);
            // 
            // tabAbilities
            // 
            this.tabAbilities.Controls.Add(this.gridAbilities);
            this.tabAbilities.Location = new System.Drawing.Point(4, 24);
            this.tabAbilities.Name = "tabAbilities";
            this.tabAbilities.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbilities.Size = new System.Drawing.Size(397, 332);
            this.tabAbilities.TabIndex = 3;
            this.tabAbilities.Text = "Abilities";
            this.tabAbilities.UseVisualStyleBackColor = true;
            // 
            // gridAbilities
            // 
            this.gridAbilities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridAbilities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAbilities.Location = new System.Drawing.Point(6, 6);
            this.gridAbilities.Name = "gridAbilities";
            this.gridAbilities.ReadOnly = true;
            this.gridAbilities.RowHeadersVisible = false;
            this.gridAbilities.RowTemplate.Height = 25;
            this.gridAbilities.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAbilities.Size = new System.Drawing.Size(385, 323);
            this.gridAbilities.TabIndex = 0;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.textBoxLog);
            this.tabLog.Location = new System.Drawing.Point(4, 24);
            this.tabLog.Name = "tabLog";
            this.tabLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabLog.Size = new System.Drawing.Size(397, 332);
            this.tabLog.TabIndex = 1;
            this.tabLog.Text = "Log";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Location = new System.Drawing.Point(16, 4);
            this.textBoxLog.MaxLength = 3276700;
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.Size = new System.Drawing.Size(375, 328);
            this.textBoxLog.TabIndex = 0;
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
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 366);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainWindow";
            this.Text = "The Idle Scrolls";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabCharacter.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabInventory.ResumeLayout(false);
            this.tabInventory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridInventory)).EndInit();
            this.tabAbilities.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridAbilities)).EndInit();
            this.tabLog.ResumeLayout(false);
            this.tabLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private TabControl tabControl1;
        private TabPage tabCharacter;
        private Label lblCharName;
        private Label lblMobName;
        private Label lblMobHP;
        private Label lblArea;
        private Label lblCharXP;
        private Label lblCharLevel;
        private TabPage tabLog;
        private TextBox textBoxLog;
        private System.Windows.Forms.Timer timerTick;
        private TableLayoutPanel tableLayoutPanel1;
        private Label hdrRawDmg;
        private Label hdrDps;
        private Label lblAttackDps;
        private Label hdrCooldown;
        private Label lblAttackCooldown;
        private Label lblAttackRawDmg;
        private Label lblAttack;
        private TabPage tabInventory;
        private DataGridView gridInventory;
        private Label hdrEqWeapon;
        private ToolTip toolTip;
        private Label lblEqWeapon;
        private TabPage tabAbilities;
        private DataGridView gridAbilities;
        private Label lblTimeLimit;
        private CheckBox cbNextAfterWin;
        private Button btnAreaNext;
        private Button btnAreaPrev;
        private Label lblEqArmor;
        private Label hdrEqArmor;
        private Label lblDefEvasion;
        private Label lblDefArmor;
        private Label label3;
        private Label label1;
    }
}