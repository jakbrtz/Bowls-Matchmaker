namespace Matchmaker.UserInterface
{ 
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.CMSdeleteday = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BTNsettings = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BTNmainhistory = new System.Windows.Forms.Button();
            this.BTNmainPlayers = new System.Windows.Forms.Button();
            this.BTNmainMatch = new System.Windows.Forms.Button();
            this.CMNaddall = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.OFDexcel = new System.Windows.Forms.OpenFileDialog();
            this.SFDexcel = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.CMSdeleteplayer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deletePlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CMSeditHTML = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editHTMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OFDplayersinday = new System.Windows.Forms.OpenFileDialog();
            this.SFDplayersinday = new System.Windows.Forms.SaveFileDialog();
            this.tabControlMain = new CustomControls.TabControlWithoutHeader();
            this.pageMainMatch = new System.Windows.Forms.TabPage();
            this.tabControlMatches = new CustomControls.TabControlWithoutHeader();
            this.pagePlayers = new System.Windows.Forms.TabPage();
            this.CLBpagePlayers = new System.Windows.Forms.CheckedListBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.BTNimportplayersforday = new System.Windows.Forms.Button();
            this.BTNsorttagmatches = new System.Windows.Forms.Button();
            this.BTNsortnamematches = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TBXfilterPagePlayers = new CustomControls.WaterMarkTextBox();
            this.pageFixedMatches = new System.Windows.Forms.TabPage();
            this.WEBfixmatches = new System.Windows.Forms.WebBrowser();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.SCBfixedmatchplayers = new CustomControls.SearchableComboBox();
            this.BTNaddfixedmatch = new System.Windows.Forms.Button();
            this.pageConfirmBeforeMatches = new System.Windows.Forms.TabPage();
            this.TBXconfirmConsole = new System.Windows.Forms.TextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.BTNmaxfours = new System.Windows.Forms.Button();
            this.BTNmaxtrips = new System.Windows.Forms.Button();
            this.NUDfours = new System.Windows.Forms.NumericUpDown();
            this.BTNmaxpairs = new System.Windows.Forms.Button();
            this.NUDtriples = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.NUDpairs = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.LBLnummatcheswarning = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.BTNnewgame = new System.Windows.Forms.Button();
            this.pageViewMatch = new System.Windows.Forms.TabPage();
            this.SCTnewgames = new System.Windows.Forms.SplitContainer();
            this.WEBnewgames = new System.Windows.Forms.WebBrowser();
            this.TBXnewdayConsole = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.BTNnewgameProblems = new System.Windows.Forms.Button();
            this.BTNnewgamesgo = new System.Windows.Forms.Button();
            this.BTNnewgamesave = new System.Windows.Forms.Button();
            this.SideBarCreateDay = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.BTNplayers = new System.Windows.Forms.Button();
            this.BTNfixedMatches = new System.Windows.Forms.Button();
            this.BTNconfirmbeforecreating = new System.Windows.Forms.Button();
            this.pageMainSettings = new System.Windows.Forms.TabPage();
            this.FLPweights = new CustomControls.AnchorFlowLayoutPanel();
            this.WVWpairsplaytogether = new Matchmaker.UserInterface.Controls.WeightView();
            this.WVWenemies = new Matchmaker.UserInterface.Controls.WeightView();
            this.WVWmainposition = new Matchmaker.UserInterface.Controls.WeightView();
            this.WVWsecondaryposition = new Matchmaker.UserInterface.Controls.WeightView();
            this.WVWbadpositiongoodgrade = new Matchmaker.UserInterface.Controls.WeightView();
            this.WVWunbalancedplayers = new Matchmaker.UserInterface.Controls.WeightView();
            this.WVWunbalancedteams = new Matchmaker.UserInterface.Controls.WeightView();
            this.WVWteamsize = new Matchmaker.UserInterface.Controls.WeightView();
            this.BTNresetWeights = new System.Windows.Forms.Button();
            this.BFFmain = new CustomControls.BrowseForFile();
            this.BFFhtml = new CustomControls.BrowseForFile();
            this.BTNseedefaulthtml = new System.Windows.Forms.Button();
            this.pageMainPlayers = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tagDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PositionPrimary = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.GradePrimary = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.PositionSecondary = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.GradeSecondary = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.preferredTeamSizesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.playerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.BTNexcelexport = new System.Windows.Forms.Button();
            this.BTNexcelimport = new System.Windows.Forms.Button();
            this.BTNaddplayer = new System.Windows.Forms.Button();
            this.SideBarPlayers = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.RBNmemberorvisitor = new System.Windows.Forms.RadioButton();
            this.RBNmember = new System.Windows.Forms.RadioButton();
            this.RBNvisitor = new System.Windows.Forms.RadioButton();
            this.BTNsortnameplayers = new System.Windows.Forms.Button();
            this.BTNsorttagplayers = new System.Windows.Forms.Button();
            this.pageMainHistory = new System.Windows.Forms.TabPage();
            this.SCTNhistory = new System.Windows.Forms.SplitContainer();
            this.WEBhistory = new System.Windows.Forms.WebBrowser();
            this.TBXhtml = new System.Windows.Forms.TextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.BTNexportplayersinday = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.BTNprintHistory = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.LBXhistory = new System.Windows.Forms.ListBox();
            this.SFDhtml = new System.Windows.Forms.SaveFileDialog();
            this.CMSdeleteday.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.CMNaddall.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.CMSdeleteplayer.SuspendLayout();
            this.CMSeditHTML.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.pageMainMatch.SuspendLayout();
            this.tabControlMatches.SuspendLayout();
            this.pagePlayers.SuspendLayout();
            this.panel6.SuspendLayout();
            this.pageFixedMatches.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pageConfirmBeforeMatches.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDfours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDtriples)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDpairs)).BeginInit();
            this.panel9.SuspendLayout();
            this.pageViewMatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SCTnewgames)).BeginInit();
            this.SCTnewgames.Panel1.SuspendLayout();
            this.SCTnewgames.Panel2.SuspendLayout();
            this.SCTnewgames.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SideBarCreateDay.SuspendLayout();
            this.pageMainSettings.SuspendLayout();
            this.FLPweights.SuspendLayout();
            this.pageMainPlayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).BeginInit();
            this.panel2.SuspendLayout();
            this.SideBarPlayers.SuspendLayout();
            this.pageMainHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SCTNhistory)).BeginInit();
            this.SCTNhistory.Panel1.SuspendLayout();
            this.SCTNhistory.Panel2.SuspendLayout();
            this.SCTNhistory.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // CMSdeleteday
            // 
            this.CMSdeleteday.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.CMSdeleteday.Name = "CMNdelete";
            this.CMSdeleteday.Size = new System.Drawing.Size(131, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.deleteToolStripMenuItem.Text = "Delete Day";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.OliveDrab;
            this.panel1.Controls.Add(this.BTNsettings);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.BTNmainhistory);
            this.panel1.Controls.Add(this.BTNmainPlayers);
            this.panel1.Controls.Add(this.BTNmainMatch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(990, 86);
            this.panel1.TabIndex = 1;
            // 
            // BTNsettings
            // 
            this.BTNsettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTNsettings.Location = new System.Drawing.Point(572, 22);
            this.BTNsettings.Name = "BTNsettings";
            this.BTNsettings.Size = new System.Drawing.Size(128, 41);
            this.BTNsettings.TabIndex = 4;
            this.BTNsettings.Text = "Settings";
            this.BTNsettings.UseVisualStyleBackColor = true;
            this.BTNsettings.Click += new System.EventHandler(this.BTNsettings_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(132, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // BTNmainhistory
            // 
            this.BTNmainhistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTNmainhistory.Location = new System.Drawing.Point(840, 22);
            this.BTNmainhistory.Name = "BTNmainhistory";
            this.BTNmainhistory.Size = new System.Drawing.Size(128, 41);
            this.BTNmainhistory.TabIndex = 2;
            this.BTNmainhistory.Text = "History";
            this.BTNmainhistory.UseVisualStyleBackColor = true;
            this.BTNmainhistory.Click += new System.EventHandler(this.BTNmainhistory_Click);
            // 
            // BTNmainPlayers
            // 
            this.BTNmainPlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTNmainPlayers.Location = new System.Drawing.Point(706, 22);
            this.BTNmainPlayers.Name = "BTNmainPlayers";
            this.BTNmainPlayers.Size = new System.Drawing.Size(128, 41);
            this.BTNmainPlayers.TabIndex = 1;
            this.BTNmainPlayers.Text = "Member List";
            this.BTNmainPlayers.UseVisualStyleBackColor = true;
            this.BTNmainPlayers.Click += new System.EventHandler(this.BTNmainPlayers_Click);
            // 
            // BTNmainMatch
            // 
            this.BTNmainMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTNmainMatch.Location = new System.Drawing.Point(438, 22);
            this.BTNmainMatch.Name = "BTNmainMatch";
            this.BTNmainMatch.Size = new System.Drawing.Size(128, 41);
            this.BTNmainMatch.TabIndex = 0;
            this.BTNmainMatch.Text = "New Day";
            this.BTNmainMatch.UseVisualStyleBackColor = true;
            this.BTNmainMatch.Click += new System.EventHandler(this.BTNmainMatch_Click);
            // 
            // CMNaddall
            // 
            this.CMNaddall.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAllToolStripMenuItem});
            this.CMNaddall.Name = "CMNaddall";
            this.CMNaddall.Size = new System.Drawing.Size(114, 26);
            // 
            // addAllToolStripMenuItem
            // 
            this.addAllToolStripMenuItem.Name = "addAllToolStripMenuItem";
            this.addAllToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.addAllToolStripMenuItem.Text = "Add All";
            this.addAllToolStripMenuItem.Click += new System.EventHandler(this.AddAllToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 580);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(990, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Step = 1;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // OFDexcel
            // 
            this.OFDexcel.FileName = "players.xlsx";
            this.OFDexcel.Filter = "Excel Files(*.xlsx;*.xls)|*.xlsx;*.xls";
            // 
            // SFDexcel
            // 
            this.SFDexcel.FileName = "players.xlsx";
            this.SFDexcel.Filter = "Excel Files(*.xlsx;*.xls)|*.xlsx;*.xls";
            // 
            // CMSdeleteplayer
            // 
            this.CMSdeleteplayer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deletePlayerToolStripMenuItem});
            this.CMSdeleteplayer.Name = "CMSdeleteplayer";
            this.CMSdeleteplayer.Size = new System.Drawing.Size(143, 26);
            // 
            // deletePlayerToolStripMenuItem
            // 
            this.deletePlayerToolStripMenuItem.Name = "deletePlayerToolStripMenuItem";
            this.deletePlayerToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.deletePlayerToolStripMenuItem.Text = "Delete Player";
            this.deletePlayerToolStripMenuItem.Click += new System.EventHandler(this.DeletePlayerToolStripMenuItem_Click);
            // 
            // CMSeditHTML
            // 
            this.CMSeditHTML.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editHTMLToolStripMenuItem});
            this.CMSeditHTML.Name = "CMSeditHTML";
            this.CMSeditHTML.Size = new System.Drawing.Size(130, 26);
            // 
            // editHTMLToolStripMenuItem
            // 
            this.editHTMLToolStripMenuItem.Name = "editHTMLToolStripMenuItem";
            this.editHTMLToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.editHTMLToolStripMenuItem.Text = "Edit HTML";
            this.editHTMLToolStripMenuItem.Click += new System.EventHandler(this.EditHTMLToolStripMenuItem_Click);
            // 
            // OFDplayersinday
            // 
            this.OFDplayersinday.Filter = "Text Files(*.txt)|*.txt";
            // 
            // SFDplayersinday
            // 
            this.SFDplayersinday.Filter = "Text Files(*.txt)|*.txt";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.pageMainMatch);
            this.tabControlMain.Controls.Add(this.pageMainSettings);
            this.tabControlMain.Controls.Add(this.pageMainPlayers);
            this.tabControlMain.Controls.Add(this.pageMainHistory);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 86);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(990, 494);
            this.tabControlMain.TabIndex = 2;
            // 
            // pageMainMatch
            // 
            this.pageMainMatch.Controls.Add(this.tabControlMatches);
            this.pageMainMatch.Controls.Add(this.SideBarCreateDay);
            this.pageMainMatch.Location = new System.Drawing.Point(4, 22);
            this.pageMainMatch.Name = "pageMainMatch";
            this.pageMainMatch.Size = new System.Drawing.Size(982, 468);
            this.pageMainMatch.TabIndex = 0;
            this.pageMainMatch.Text = "Matches";
            this.pageMainMatch.UseVisualStyleBackColor = true;
            // 
            // tabControlMatches
            // 
            this.tabControlMatches.Controls.Add(this.pagePlayers);
            this.tabControlMatches.Controls.Add(this.pageFixedMatches);
            this.tabControlMatches.Controls.Add(this.pageConfirmBeforeMatches);
            this.tabControlMatches.Controls.Add(this.pageViewMatch);
            this.tabControlMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMatches.Location = new System.Drawing.Point(200, 0);
            this.tabControlMatches.Name = "tabControlMatches";
            this.tabControlMatches.SelectedIndex = 0;
            this.tabControlMatches.Size = new System.Drawing.Size(782, 468);
            this.tabControlMatches.TabIndex = 2;
            // 
            // pagePlayers
            // 
            this.pagePlayers.Controls.Add(this.CLBpagePlayers);
            this.pagePlayers.Controls.Add(this.panel6);
            this.pagePlayers.Location = new System.Drawing.Point(4, 22);
            this.pagePlayers.Name = "pagePlayers";
            this.pagePlayers.Size = new System.Drawing.Size(774, 442);
            this.pagePlayers.TabIndex = 0;
            this.pagePlayers.Text = "Players";
            this.pagePlayers.UseVisualStyleBackColor = true;
            // 
            // CLBpagePlayers
            // 
            this.CLBpagePlayers.CheckOnClick = true;
            this.CLBpagePlayers.ContextMenuStrip = this.CMNaddall;
            this.CLBpagePlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CLBpagePlayers.FormatString = "%name - %position";
            this.CLBpagePlayers.FormattingEnabled = true;
            this.CLBpagePlayers.IntegralHeight = false;
            this.CLBpagePlayers.Location = new System.Drawing.Point(0, 58);
            this.CLBpagePlayers.Name = "CLBpagePlayers";
            this.CLBpagePlayers.Size = new System.Drawing.Size(774, 384);
            this.CLBpagePlayers.TabIndex = 2;
            this.CLBpagePlayers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CLBpagePlayers_ItemCheck);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.BTNimportplayersforday);
            this.panel6.Controls.Add(this.BTNsorttagmatches);
            this.panel6.Controls.Add(this.BTNsortnamematches);
            this.panel6.Controls.Add(this.label6);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Controls.Add(this.TBXfilterPagePlayers);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(774, 58);
            this.panel6.TabIndex = 3;
            // 
            // BTNimportplayersforday
            // 
            this.BTNimportplayersforday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNimportplayersforday.Location = new System.Drawing.Point(541, 31);
            this.BTNimportplayersforday.Name = "BTNimportplayersforday";
            this.BTNimportplayersforday.Size = new System.Drawing.Size(128, 23);
            this.BTNimportplayersforday.TabIndex = 8;
            this.BTNimportplayersforday.Text = "Import from File";
            this.BTNimportplayersforday.UseVisualStyleBackColor = true;
            this.BTNimportplayersforday.Click += new System.EventHandler(this.BTNimportplayersforday_Click);
            // 
            // BTNsorttagmatches
            // 
            this.BTNsorttagmatches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNsorttagmatches.Location = new System.Drawing.Point(407, 31);
            this.BTNsorttagmatches.Name = "BTNsorttagmatches";
            this.BTNsorttagmatches.Size = new System.Drawing.Size(128, 23);
            this.BTNsorttagmatches.TabIndex = 7;
            this.BTNsorttagmatches.Text = "Sort by Tag Number";
            this.BTNsorttagmatches.UseVisualStyleBackColor = true;
            this.BTNsorttagmatches.Click += new System.EventHandler(this.BTNsorttagmatches_Click);
            // 
            // BTNsortnamematches
            // 
            this.BTNsortnamematches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNsortnamematches.Location = new System.Drawing.Point(273, 31);
            this.BTNsortnamematches.Name = "BTNsortnamematches";
            this.BTNsortnamematches.Size = new System.Drawing.Size(128, 23);
            this.BTNsortnamematches.TabIndex = 6;
            this.BTNsortnamematches.Text = "Sort by Name";
            this.BTNsortnamematches.UseVisualStyleBackColor = true;
            this.BTNsortnamematches.Click += new System.EventHandler(this.BTNsortnamematches_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(593, 25);
            this.label6.TabIndex = 3;
            this.label6.Text = "Check the boxes next to all players who will be playing today";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search for Player:";
            // 
            // TBXfilterPagePlayers
            // 
            this.TBXfilterPagePlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TBXfilterPagePlayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.TBXfilterPagePlayers.Location = new System.Drawing.Point(102, 33);
            this.TBXfilterPagePlayers.Name = "TBXfilterPagePlayers";
            this.TBXfilterPagePlayers.Size = new System.Drawing.Size(165, 20);
            this.TBXfilterPagePlayers.TabIndex = 1;
            this.TBXfilterPagePlayers.WaterMarkColor = System.Drawing.Color.Gray;
            this.TBXfilterPagePlayers.WaterMarkText = "Name or Tag Number";
            this.TBXfilterPagePlayers.TextChanged += new System.EventHandler(this.TBXfilterPagePlayers_TextChanged);
            // 
            // pageFixedMatches
            // 
            this.pageFixedMatches.Controls.Add(this.WEBfixmatches);
            this.pageFixedMatches.Controls.Add(this.panel3);
            this.pageFixedMatches.Location = new System.Drawing.Point(4, 22);
            this.pageFixedMatches.Name = "pageFixedMatches";
            this.pageFixedMatches.Size = new System.Drawing.Size(774, 442);
            this.pageFixedMatches.TabIndex = 1;
            this.pageFixedMatches.Text = "Fixed Matches";
            this.pageFixedMatches.UseVisualStyleBackColor = true;
            // 
            // WEBfixmatches
            // 
            this.WEBfixmatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WEBfixmatches.Location = new System.Drawing.Point(0, 87);
            this.WEBfixmatches.MinimumSize = new System.Drawing.Size(20, 20);
            this.WEBfixmatches.Name = "WEBfixmatches";
            this.WEBfixmatches.Size = new System.Drawing.Size(774, 355);
            this.WEBfixmatches.TabIndex = 16;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.SCBfixedmatchplayers);
            this.panel3.Controls.Add(this.BTNaddfixedmatch);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(774, 87);
            this.panel3.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(622, 50);
            this.label2.TabIndex = 20;
            this.label2.Text = "Select a player in the dropdown then click where they should go\r\nClick on two pla" +
    "yers to swap them";
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(137, 62);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(114, 13);
            this.label12.TabIndex = 13;
            this.label12.Text = "Select a player to Add:";
            // 
            // SCBfixedmatchplayers
            // 
            this.SCBfixedmatchplayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SCBfixedmatchplayers.FormattingEnabled = true;
            this.SCBfixedmatchplayers.IntegralHeight = false;
            this.SCBfixedmatchplayers.Location = new System.Drawing.Point(257, 59);
            this.SCBfixedmatchplayers.Name = "SCBfixedmatchplayers";
            this.SCBfixedmatchplayers.Size = new System.Drawing.Size(121, 21);
            this.SCBfixedmatchplayers.TabIndex = 19;
            this.SCBfixedmatchplayers.SelectedIndexChanged += new System.EventHandler(this.SCBfixedmatchplayers_SelectedIndexChanged);
            // 
            // BTNaddfixedmatch
            // 
            this.BTNaddfixedmatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNaddfixedmatch.Location = new System.Drawing.Point(3, 57);
            this.BTNaddfixedmatch.Name = "BTNaddfixedmatch";
            this.BTNaddfixedmatch.Size = new System.Drawing.Size(128, 23);
            this.BTNaddfixedmatch.TabIndex = 10;
            this.BTNaddfixedmatch.Text = "Add Match";
            this.BTNaddfixedmatch.UseVisualStyleBackColor = true;
            this.BTNaddfixedmatch.Click += new System.EventHandler(this.BTNaddfixedmatch_Click);
            // 
            // pageConfirmBeforeMatches
            // 
            this.pageConfirmBeforeMatches.Controls.Add(this.TBXconfirmConsole);
            this.pageConfirmBeforeMatches.Controls.Add(this.panel7);
            this.pageConfirmBeforeMatches.Controls.Add(this.panel9);
            this.pageConfirmBeforeMatches.Location = new System.Drawing.Point(4, 22);
            this.pageConfirmBeforeMatches.Name = "pageConfirmBeforeMatches";
            this.pageConfirmBeforeMatches.Size = new System.Drawing.Size(774, 442);
            this.pageConfirmBeforeMatches.TabIndex = 2;
            this.pageConfirmBeforeMatches.Text = "Confirm";
            this.pageConfirmBeforeMatches.UseVisualStyleBackColor = true;
            // 
            // TBXconfirmConsole
            // 
            this.TBXconfirmConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TBXconfirmConsole.Location = new System.Drawing.Point(3, 186);
            this.TBXconfirmConsole.Multiline = true;
            this.TBXconfirmConsole.Name = "TBXconfirmConsole";
            this.TBXconfirmConsole.ReadOnly = true;
            this.TBXconfirmConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TBXconfirmConsole.Size = new System.Drawing.Size(767, 253);
            this.TBXconfirmConsole.TabIndex = 10;
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.Controls.Add(this.BTNmaxfours);
            this.panel7.Controls.Add(this.BTNmaxtrips);
            this.panel7.Controls.Add(this.NUDfours);
            this.panel7.Controls.Add(this.BTNmaxpairs);
            this.panel7.Controls.Add(this.NUDtriples);
            this.panel7.Controls.Add(this.label10);
            this.panel7.Controls.Add(this.NUDpairs);
            this.panel7.Controls.Add(this.label8);
            this.panel7.Controls.Add(this.label9);
            this.panel7.Controls.Add(this.LBLnummatcheswarning);
            this.panel7.Location = new System.Drawing.Point(3, 93);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(767, 87);
            this.panel7.TabIndex = 11;
            // 
            // BTNmaxfours
            // 
            this.BTNmaxfours.Location = new System.Drawing.Point(110, 61);
            this.BTNmaxfours.Name = "BTNmaxfours";
            this.BTNmaxfours.Size = new System.Drawing.Size(75, 23);
            this.BTNmaxfours.TabIndex = 13;
            this.BTNmaxfours.Text = "max fours";
            this.BTNmaxfours.UseVisualStyleBackColor = true;
            this.BTNmaxfours.Click += new System.EventHandler(this.BTNmaxfours_Click);
            // 
            // BTNmaxtrips
            // 
            this.BTNmaxtrips.Location = new System.Drawing.Point(110, 32);
            this.BTNmaxtrips.Name = "BTNmaxtrips";
            this.BTNmaxtrips.Size = new System.Drawing.Size(75, 23);
            this.BTNmaxtrips.TabIndex = 12;
            this.BTNmaxtrips.Text = "max trips";
            this.BTNmaxtrips.UseVisualStyleBackColor = true;
            this.BTNmaxtrips.Click += new System.EventHandler(this.BTNmaxtrips_Click);
            // 
            // NUDfours
            // 
            this.NUDfours.Location = new System.Drawing.Point(45, 64);
            this.NUDfours.Name = "NUDfours";
            this.NUDfours.Size = new System.Drawing.Size(59, 20);
            this.NUDfours.TabIndex = 9;
            this.NUDfours.ValueChanged += new System.EventHandler(this.NUDnumMatches_ValueChanged);
            // 
            // BTNmaxpairs
            // 
            this.BTNmaxpairs.Location = new System.Drawing.Point(110, 3);
            this.BTNmaxpairs.Name = "BTNmaxpairs";
            this.BTNmaxpairs.Size = new System.Drawing.Size(75, 23);
            this.BTNmaxpairs.TabIndex = 11;
            this.BTNmaxpairs.Text = "max pairs";
            this.BTNmaxpairs.UseVisualStyleBackColor = true;
            this.BTNmaxpairs.Click += new System.EventHandler(this.BTNmaxpairs_Click);
            // 
            // NUDtriples
            // 
            this.NUDtriples.Location = new System.Drawing.Point(45, 35);
            this.NUDtriples.Name = "NUDtriples";
            this.NUDtriples.Size = new System.Drawing.Size(59, 20);
            this.NUDtriples.TabIndex = 9;
            this.NUDtriples.ValueChanged += new System.EventHandler(this.NUDnumMatches_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 66);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Fours:";
            // 
            // NUDpairs
            // 
            this.NUDpairs.Location = new System.Drawing.Point(45, 6);
            this.NUDpairs.Name = "NUDpairs";
            this.NUDpairs.Size = new System.Drawing.Size(59, 20);
            this.NUDpairs.TabIndex = 9;
            this.NUDpairs.ValueChanged += new System.EventHandler(this.NUDnumMatches_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Pairs:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Triples:";
            // 
            // LBLnummatcheswarning
            // 
            this.LBLnummatcheswarning.ForeColor = System.Drawing.Color.Red;
            this.LBLnummatcheswarning.Location = new System.Drawing.Point(191, 3);
            this.LBLnummatcheswarning.Name = "LBLnummatcheswarning";
            this.LBLnummatcheswarning.Size = new System.Drawing.Size(111, 81);
            this.LBLnummatcheswarning.TabIndex = 10;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.label4);
            this.panel9.Controls.Add(this.BTNnewgame);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(774, 87);
            this.panel9.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(385, 52);
            this.label4.TabIndex = 7;
            this.label4.Text = "Make sure all these details are right\r\nIf they are, press \"Continue\"";
            // 
            // BTNnewgame
            // 
            this.BTNnewgame.Location = new System.Drawing.Point(7, 58);
            this.BTNnewgame.Margin = new System.Windows.Forms.Padding(7);
            this.BTNnewgame.Name = "BTNnewgame";
            this.BTNnewgame.Size = new System.Drawing.Size(128, 23);
            this.BTNnewgame.TabIndex = 8;
            this.BTNnewgame.Text = "Continue";
            this.BTNnewgame.UseVisualStyleBackColor = true;
            this.BTNnewgame.Click += new System.EventHandler(this.BTNnewgame_Click);
            // 
            // pageViewMatch
            // 
            this.pageViewMatch.Controls.Add(this.SCTnewgames);
            this.pageViewMatch.Controls.Add(this.panel5);
            this.pageViewMatch.Location = new System.Drawing.Point(4, 22);
            this.pageViewMatch.Name = "pageViewMatch";
            this.pageViewMatch.Size = new System.Drawing.Size(774, 442);
            this.pageViewMatch.TabIndex = 3;
            this.pageViewMatch.Text = "New Games";
            this.pageViewMatch.UseVisualStyleBackColor = true;
            // 
            // SCTnewgames
            // 
            this.SCTnewgames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SCTnewgames.Location = new System.Drawing.Point(0, 87);
            this.SCTnewgames.Name = "SCTnewgames";
            this.SCTnewgames.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SCTnewgames.Panel1
            // 
            this.SCTnewgames.Panel1.Controls.Add(this.WEBnewgames);
            // 
            // SCTnewgames.Panel2
            // 
            this.SCTnewgames.Panel2.Controls.Add(this.TBXnewdayConsole);
            this.SCTnewgames.Panel2Collapsed = true;
            this.SCTnewgames.Size = new System.Drawing.Size(774, 355);
            this.SCTnewgames.SplitterDistance = 227;
            this.SCTnewgames.TabIndex = 12;
            // 
            // WEBnewgames
            // 
            this.WEBnewgames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WEBnewgames.IsWebBrowserContextMenuEnabled = false;
            this.WEBnewgames.Location = new System.Drawing.Point(0, 0);
            this.WEBnewgames.MinimumSize = new System.Drawing.Size(20, 20);
            this.WEBnewgames.Name = "WEBnewgames";
            this.WEBnewgames.Size = new System.Drawing.Size(774, 355);
            this.WEBnewgames.TabIndex = 13;
            this.WEBnewgames.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.WEBnewgames_DocumentCompleted);
            this.WEBnewgames.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WEBnewgames_Navigating);
            // 
            // TBXnewdayConsole
            // 
            this.TBXnewdayConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TBXnewdayConsole.Location = new System.Drawing.Point(0, 0);
            this.TBXnewdayConsole.Multiline = true;
            this.TBXnewdayConsole.Name = "TBXnewdayConsole";
            this.TBXnewdayConsole.ReadOnly = true;
            this.TBXnewdayConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TBXnewdayConsole.Size = new System.Drawing.Size(150, 46);
            this.TBXnewdayConsole.TabIndex = 10;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.BTNnewgameProblems);
            this.panel5.Controls.Add(this.BTNnewgamesgo);
            this.panel5.Controls.Add(this.BTNnewgamesave);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(774, 87);
            this.panel5.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(768, 52);
            this.label7.TabIndex = 7;
            this.label7.Text = "Click on two players to swap them\r\nWhen you\'re happy with the games click \"Confir" +
    "m\"";
            // 
            // BTNnewgameProblems
            // 
            this.BTNnewgameProblems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNnewgameProblems.Location = new System.Drawing.Point(137, 58);
            this.BTNnewgameProblems.Name = "BTNnewgameProblems";
            this.BTNnewgameProblems.Size = new System.Drawing.Size(128, 23);
            this.BTNnewgameProblems.TabIndex = 4;
            this.BTNnewgameProblems.Text = "Show Problems";
            this.BTNnewgameProblems.UseVisualStyleBackColor = true;
            this.BTNnewgameProblems.Click += new System.EventHandler(this.BTNnewgameProblems_Click);
            // 
            // BTNnewgamesgo
            // 
            this.BTNnewgamesgo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNnewgamesgo.Location = new System.Drawing.Point(3, 58);
            this.BTNnewgamesgo.Name = "BTNnewgamesgo";
            this.BTNnewgamesgo.Size = new System.Drawing.Size(128, 23);
            this.BTNnewgamesgo.TabIndex = 3;
            this.BTNnewgamesgo.Text = "Redo Selection";
            this.BTNnewgamesgo.UseVisualStyleBackColor = true;
            this.BTNnewgamesgo.Click += new System.EventHandler(this.BTNnewgamesgo_Click);
            // 
            // BTNnewgamesave
            // 
            this.BTNnewgamesave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNnewgamesave.Location = new System.Drawing.Point(271, 58);
            this.BTNnewgamesave.Name = "BTNnewgamesave";
            this.BTNnewgamesave.Size = new System.Drawing.Size(128, 23);
            this.BTNnewgamesave.TabIndex = 5;
            this.BTNnewgamesave.Text = "Confirm";
            this.BTNnewgamesave.UseVisualStyleBackColor = true;
            this.BTNnewgamesave.Click += new System.EventHandler(this.BTNnewgamesave_Click);
            // 
            // SideBarCreateDay
            // 
            this.SideBarCreateDay.AutoScroll = true;
            this.SideBarCreateDay.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.SideBarCreateDay.Controls.Add(this.label5);
            this.SideBarCreateDay.Controls.Add(this.BTNplayers);
            this.SideBarCreateDay.Controls.Add(this.BTNfixedMatches);
            this.SideBarCreateDay.Controls.Add(this.BTNconfirmbeforecreating);
            this.SideBarCreateDay.Dock = System.Windows.Forms.DockStyle.Left;
            this.SideBarCreateDay.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SideBarCreateDay.Location = new System.Drawing.Point(0, 0);
            this.SideBarCreateDay.Name = "SideBarCreateDay";
            this.SideBarCreateDay.Size = new System.Drawing.Size(200, 468);
            this.SideBarCreateDay.TabIndex = 1;
            this.SideBarCreateDay.WrapContents = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 9;
            // 
            // BTNplayers
            // 
            this.BTNplayers.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BTNplayers.Location = new System.Drawing.Point(7, 20);
            this.BTNplayers.Margin = new System.Windows.Forms.Padding(7);
            this.BTNplayers.Name = "BTNplayers";
            this.BTNplayers.Size = new System.Drawing.Size(183, 23);
            this.BTNplayers.TabIndex = 0;
            this.BTNplayers.Text = "Select Players";
            this.BTNplayers.UseVisualStyleBackColor = true;
            this.BTNplayers.Click += new System.EventHandler(this.BTNplayers_Click);
            // 
            // BTNfixedMatches
            // 
            this.BTNfixedMatches.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BTNfixedMatches.Location = new System.Drawing.Point(7, 57);
            this.BTNfixedMatches.Margin = new System.Windows.Forms.Padding(7);
            this.BTNfixedMatches.Name = "BTNfixedMatches";
            this.BTNfixedMatches.Size = new System.Drawing.Size(183, 23);
            this.BTNfixedMatches.TabIndex = 1;
            this.BTNfixedMatches.Text = "Fixed Matches";
            this.BTNfixedMatches.UseVisualStyleBackColor = true;
            this.BTNfixedMatches.Click += new System.EventHandler(this.BTNfixedMatches_Click);
            // 
            // BTNconfirmbeforecreating
            // 
            this.BTNconfirmbeforecreating.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.BTNconfirmbeforecreating.Location = new System.Drawing.Point(7, 94);
            this.BTNconfirmbeforecreating.Margin = new System.Windows.Forms.Padding(7);
            this.BTNconfirmbeforecreating.Name = "BTNconfirmbeforecreating";
            this.BTNconfirmbeforecreating.Size = new System.Drawing.Size(183, 23);
            this.BTNconfirmbeforecreating.TabIndex = 2;
            this.BTNconfirmbeforecreating.Text = "Create Matches";
            this.BTNconfirmbeforecreating.UseVisualStyleBackColor = true;
            this.BTNconfirmbeforecreating.Click += new System.EventHandler(this.BTNconfirmbeforecreating_Click);
            // 
            // pageMainSettings
            // 
            this.pageMainSettings.Controls.Add(this.FLPweights);
            this.pageMainSettings.Location = new System.Drawing.Point(4, 22);
            this.pageMainSettings.Name = "pageMainSettings";
            this.pageMainSettings.Size = new System.Drawing.Size(982, 468);
            this.pageMainSettings.TabIndex = 3;
            this.pageMainSettings.Text = "Settings";
            this.pageMainSettings.UseVisualStyleBackColor = true;
            // 
            // FLPweights
            // 
            this.FLPweights.AutoScroll = true;
            this.FLPweights.AutoSize = true;
            this.FLPweights.Controls.Add(this.WVWpairsplaytogether);
            this.FLPweights.Controls.Add(this.WVWenemies);
            this.FLPweights.Controls.Add(this.WVWmainposition);
            this.FLPweights.Controls.Add(this.WVWsecondaryposition);
            this.FLPweights.Controls.Add(this.WVWbadpositiongoodgrade);
            this.FLPweights.Controls.Add(this.WVWunbalancedplayers);
            this.FLPweights.Controls.Add(this.WVWunbalancedteams);
            this.FLPweights.Controls.Add(this.WVWteamsize);
            this.FLPweights.Controls.Add(this.BTNresetWeights);
            this.FLPweights.Controls.Add(this.BFFmain);
            this.FLPweights.Controls.Add(this.BFFhtml);
            this.FLPweights.Controls.Add(this.BTNseedefaulthtml);
            this.FLPweights.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FLPweights.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FLPweights.Location = new System.Drawing.Point(0, 0);
            this.FLPweights.Name = "FLPweights";
            this.FLPweights.Size = new System.Drawing.Size(982, 468);
            this.FLPweights.TabIndex = 12;
            this.FLPweights.WrapContents = false;
            // 
            // WVWpairsplaytogether
            // 
            this.WVWpairsplaytogether.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WVWpairsplaytogether.Description = "Pairs of players should not have multiple games as teammates";
            this.WVWpairsplaytogether.Location = new System.Drawing.Point(3, 3);
            this.WVWpairsplaytogether.Name = "WVWpairsplaytogether";
            this.WVWpairsplaytogether.Size = new System.Drawing.Size(959, 70);
            this.WVWpairsplaytogether.TabIndex = 0;
            this.WVWpairsplaytogether.WeightChanged += new System.EventHandler(this.WVW_WeightChanged);
            // 
            // WVWenemies
            // 
            this.WVWenemies.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WVWenemies.Description = "Pairs of players should not have multiple games against each other";
            this.WVWenemies.Location = new System.Drawing.Point(3, 79);
            this.WVWenemies.Name = "WVWenemies";
            this.WVWenemies.Size = new System.Drawing.Size(959, 70);
            this.WVWenemies.TabIndex = 4;
            this.WVWenemies.WeightChanged += new System.EventHandler(this.WVW_WeightChanged);
            // 
            // WVWmainposition
            // 
            this.WVWmainposition.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WVWmainposition.Description = "Players should not get positions they don\'t play";
            this.WVWmainposition.Location = new System.Drawing.Point(3, 155);
            this.WVWmainposition.Name = "WVWmainposition";
            this.WVWmainposition.Size = new System.Drawing.Size(959, 70);
            this.WVWmainposition.TabIndex = 1;
            this.WVWmainposition.WeightChanged += new System.EventHandler(this.WVW_WeightChanged);
            // 
            // WVWsecondaryposition
            // 
            this.WVWsecondaryposition.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WVWsecondaryposition.Description = "Players should not get their secondary position";
            this.WVWsecondaryposition.Location = new System.Drawing.Point(3, 231);
            this.WVWsecondaryposition.Name = "WVWsecondaryposition";
            this.WVWsecondaryposition.Size = new System.Drawing.Size(959, 70);
            this.WVWsecondaryposition.TabIndex = 2;
            this.WVWsecondaryposition.WeightChanged += new System.EventHandler(this.WVW_WeightChanged);
            // 
            // WVWbadpositiongoodgrade
            // 
            this.WVWbadpositiongoodgrade.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WVWbadpositiongoodgrade.Description = "Good players should not get moved to other positions";
            this.WVWbadpositiongoodgrade.Location = new System.Drawing.Point(3, 307);
            this.WVWbadpositiongoodgrade.Name = "WVWbadpositiongoodgrade";
            this.WVWbadpositiongoodgrade.Size = new System.Drawing.Size(959, 70);
            this.WVWbadpositiongoodgrade.TabIndex = 8;
            this.WVWbadpositiongoodgrade.WeightChanged += new System.EventHandler(this.WVW_WeightChanged);
            // 
            // WVWunbalancedplayers
            // 
            this.WVWunbalancedplayers.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WVWunbalancedplayers.Description = "Players should be put against other players of equal skill";
            this.WVWunbalancedplayers.Location = new System.Drawing.Point(3, 383);
            this.WVWunbalancedplayers.Name = "WVWunbalancedplayers";
            this.WVWunbalancedplayers.Size = new System.Drawing.Size(959, 70);
            this.WVWunbalancedplayers.TabIndex = 6;
            this.WVWunbalancedplayers.WeightChanged += new System.EventHandler(this.WVW_WeightChanged);
            // 
            // WVWunbalancedteams
            // 
            this.WVWunbalancedteams.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WVWunbalancedteams.Description = "Teams should be put against other teams of equal skill";
            this.WVWunbalancedteams.Location = new System.Drawing.Point(3, 459);
            this.WVWunbalancedteams.Name = "WVWunbalancedteams";
            this.WVWunbalancedteams.Size = new System.Drawing.Size(959, 70);
            this.WVWunbalancedteams.TabIndex = 7;
            this.WVWunbalancedteams.WeightChanged += new System.EventHandler(this.WVW_WeightChanged);
            // 
            // WVWteamsize
            // 
            this.WVWteamsize.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WVWteamsize.Description = "Players should be in their preferred team size";
            this.WVWteamsize.Location = new System.Drawing.Point(3, 535);
            this.WVWteamsize.Name = "WVWteamsize";
            this.WVWteamsize.Size = new System.Drawing.Size(959, 70);
            this.WVWteamsize.TabIndex = 3;
            this.WVWteamsize.WeightChanged += new System.EventHandler(this.WVW_WeightChanged);
            // 
            // BTNresetWeights
            // 
            this.BTNresetWeights.Location = new System.Drawing.Point(7, 615);
            this.BTNresetWeights.Margin = new System.Windows.Forms.Padding(7);
            this.BTNresetWeights.Name = "BTNresetWeights";
            this.BTNresetWeights.Size = new System.Drawing.Size(183, 23);
            this.BTNresetWeights.TabIndex = 5;
            this.BTNresetWeights.Text = "Reset to Defaults";
            this.BTNresetWeights.UseVisualStyleBackColor = true;
            this.BTNresetWeights.Click += new System.EventHandler(this.BTNresetWeights_Click);
            // 
            // BFFmain
            // 
            this.BFFmain.FileName = "";
            this.BFFmain.Location = new System.Drawing.Point(3, 648);
            this.BFFmain.Name = "BFFmain";
            this.BFFmain.Size = new System.Drawing.Size(561, 29);
            this.BFFmain.TabIndex = 9;
            this.BFFmain.Visible = false;
            this.BFFmain.FileNameChanged += new System.EventHandler(this.BFFmain_FileNameChanged);
            // 
            // BFFhtml
            // 
            this.BFFhtml.FileName = "";
            this.BFFhtml.Location = new System.Drawing.Point(3, 683);
            this.BFFhtml.Name = "BFFhtml";
            this.BFFhtml.Size = new System.Drawing.Size(561, 29);
            this.BFFhtml.TabIndex = 10;
            this.BFFhtml.Visible = false;
            this.BFFhtml.FileNameChanged += new System.EventHandler(this.BFFhtml_FileNameChanged);
            // 
            // BTNseedefaulthtml
            // 
            this.BTNseedefaulthtml.Location = new System.Drawing.Point(7, 722);
            this.BTNseedefaulthtml.Margin = new System.Windows.Forms.Padding(7);
            this.BTNseedefaulthtml.Name = "BTNseedefaulthtml";
            this.BTNseedefaulthtml.Size = new System.Drawing.Size(183, 23);
            this.BTNseedefaulthtml.TabIndex = 11;
            this.BTNseedefaulthtml.Text = "See default layout";
            this.BTNseedefaulthtml.UseVisualStyleBackColor = true;
            this.BTNseedefaulthtml.Visible = false;
            this.BTNseedefaulthtml.Click += new System.EventHandler(this.BTNseedefaulthtml_Click);
            // 
            // pageMainPlayers
            // 
            this.pageMainPlayers.Controls.Add(this.dataGridView1);
            this.pageMainPlayers.Controls.Add(this.panel2);
            this.pageMainPlayers.Controls.Add(this.SideBarPlayers);
            this.pageMainPlayers.Location = new System.Drawing.Point(4, 22);
            this.pageMainPlayers.Name = "pageMainPlayers";
            this.pageMainPlayers.Size = new System.Drawing.Size(982, 468);
            this.pageMainPlayers.TabIndex = 1;
            this.pageMainPlayers.Text = "Players";
            this.pageMainPlayers.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tagDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.PositionPrimary,
            this.GradePrimary,
            this.PositionSecondary,
            this.GradeSecondary,
            this.preferredTeamSizesDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.playerBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(200, 31);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(782, 437);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            // 
            // tagDataGridViewTextBoxColumn
            // 
            this.tagDataGridViewTextBoxColumn.DataPropertyName = "TagNumberForTable";
            this.tagDataGridViewTextBoxColumn.HeaderText = "#";
            this.tagDataGridViewTextBoxColumn.Name = "tagDataGridViewTextBoxColumn";
            this.tagDataGridViewTextBoxColumn.Width = 30;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "NameForTable";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.Width = 200;
            // 
            // PositionPrimary
            // 
            this.PositionPrimary.DataPropertyName = "PositionPrimaryForTable";
            this.PositionPrimary.HeaderText = "Position Primary";
            this.PositionPrimary.Name = "PositionPrimary";
            this.PositionPrimary.Width = 110;
            // 
            // GradePrimary
            // 
            this.GradePrimary.DataPropertyName = "GradePrimaryForTable";
            this.GradePrimary.HeaderText = "Grade Primary";
            this.GradePrimary.Name = "GradePrimary";
            // 
            // PositionSecondary
            // 
            this.PositionSecondary.DataPropertyName = "PositionSecondaryForTable";
            this.PositionSecondary.HeaderText = "Position Secondary";
            this.PositionSecondary.Name = "PositionSecondary";
            this.PositionSecondary.Width = 110;
            // 
            // GradeSecondary
            // 
            this.GradeSecondary.DataPropertyName = "GradeSecondaryForTable";
            this.GradeSecondary.HeaderText = "Grade Secondary";
            this.GradeSecondary.Name = "GradeSecondary";
            // 
            // preferredTeamSizesDataGridViewTextBoxColumn
            // 
            this.preferredTeamSizesDataGridViewTextBoxColumn.DataPropertyName = "PreferredTeamSizesForTable";
            this.preferredTeamSizesDataGridViewTextBoxColumn.HeaderText = "Preferred Team Sizes";
            this.preferredTeamSizesDataGridViewTextBoxColumn.Name = "preferredTeamSizesDataGridViewTextBoxColumn";
            this.preferredTeamSizesDataGridViewTextBoxColumn.Width = 120;
            // 
            // playerBindingSource
            // 
            this.playerBindingSource.DataSource = typeof(Matchmaker.Data.Player);
            this.playerBindingSource.AddingNew += new System.ComponentModel.AddingNewEventHandler(this.PlayerBindingSource_AddingNew);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.BTNexcelexport);
            this.panel2.Controls.Add(this.BTNexcelimport);
            this.panel2.Controls.Add(this.BTNaddplayer);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(200, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(782, 31);
            this.panel2.TabIndex = 3;
            // 
            // BTNexcelexport
            // 
            this.BTNexcelexport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNexcelexport.Location = new System.Drawing.Point(274, 5);
            this.BTNexcelexport.Name = "BTNexcelexport";
            this.BTNexcelexport.Size = new System.Drawing.Size(128, 23);
            this.BTNexcelexport.TabIndex = 2;
            this.BTNexcelexport.Text = "Export to Excel";
            this.BTNexcelexport.UseVisualStyleBackColor = true;
            this.BTNexcelexport.Click += new System.EventHandler(this.BTNexcelexport_Click);
            // 
            // BTNexcelimport
            // 
            this.BTNexcelimport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNexcelimport.Location = new System.Drawing.Point(140, 5);
            this.BTNexcelimport.Name = "BTNexcelimport";
            this.BTNexcelimport.Size = new System.Drawing.Size(128, 23);
            this.BTNexcelimport.TabIndex = 1;
            this.BTNexcelimport.Text = "Import from Excel";
            this.BTNexcelimport.UseVisualStyleBackColor = true;
            this.BTNexcelimport.Click += new System.EventHandler(this.BTNexcelimport_Click);
            // 
            // BTNaddplayer
            // 
            this.BTNaddplayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNaddplayer.Location = new System.Drawing.Point(6, 5);
            this.BTNaddplayer.Name = "BTNaddplayer";
            this.BTNaddplayer.Size = new System.Drawing.Size(128, 23);
            this.BTNaddplayer.TabIndex = 0;
            this.BTNaddplayer.Text = "New Player";
            this.BTNaddplayer.UseVisualStyleBackColor = true;
            this.BTNaddplayer.Click += new System.EventHandler(this.BTNaddplayer_Click);
            // 
            // SideBarPlayers
            // 
            this.SideBarPlayers.AutoScroll = true;
            this.SideBarPlayers.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.SideBarPlayers.Controls.Add(this.label3);
            this.SideBarPlayers.Controls.Add(this.RBNmemberorvisitor);
            this.SideBarPlayers.Controls.Add(this.RBNmember);
            this.SideBarPlayers.Controls.Add(this.RBNvisitor);
            this.SideBarPlayers.Controls.Add(this.BTNsortnameplayers);
            this.SideBarPlayers.Controls.Add(this.BTNsorttagplayers);
            this.SideBarPlayers.Dock = System.Windows.Forms.DockStyle.Left;
            this.SideBarPlayers.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SideBarPlayers.Location = new System.Drawing.Point(0, 0);
            this.SideBarPlayers.Name = "SideBarPlayers";
            this.SideBarPlayers.Size = new System.Drawing.Size(200, 468);
            this.SideBarPlayers.TabIndex = 2;
            this.SideBarPlayers.WrapContents = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Filters:";
            // 
            // RBNmemberorvisitor
            // 
            this.RBNmemberorvisitor.AutoSize = true;
            this.RBNmemberorvisitor.Checked = true;
            this.RBNmemberorvisitor.Location = new System.Drawing.Point(7, 34);
            this.RBNmemberorvisitor.Margin = new System.Windows.Forms.Padding(7);
            this.RBNmemberorvisitor.Name = "RBNmemberorvisitor";
            this.RBNmemberorvisitor.Size = new System.Drawing.Size(116, 17);
            this.RBNmemberorvisitor.TabIndex = 3;
            this.RBNmemberorvisitor.TabStop = true;
            this.RBNmemberorvisitor.Text = "Members or Visitors";
            this.RBNmemberorvisitor.UseVisualStyleBackColor = true;
            this.RBNmemberorvisitor.CheckedChanged += new System.EventHandler(this.RBNmemberorvisitor_CheckedChanged);
            this.RBNmemberorvisitor.Click += new System.EventHandler(this.RBNmemberorvisitor_CheckedChanged);
            // 
            // RBNmember
            // 
            this.RBNmember.AutoSize = true;
            this.RBNmember.Location = new System.Drawing.Point(7, 65);
            this.RBNmember.Margin = new System.Windows.Forms.Padding(7);
            this.RBNmember.Name = "RBNmember";
            this.RBNmember.Size = new System.Drawing.Size(68, 17);
            this.RBNmember.TabIndex = 1;
            this.RBNmember.Text = "Members";
            this.RBNmember.UseVisualStyleBackColor = true;
            this.RBNmember.CheckedChanged += new System.EventHandler(this.RBNmember_CheckedChanged);
            this.RBNmember.Click += new System.EventHandler(this.RBNmember_CheckedChanged);
            // 
            // RBNvisitor
            // 
            this.RBNvisitor.AutoSize = true;
            this.RBNvisitor.Location = new System.Drawing.Point(7, 96);
            this.RBNvisitor.Margin = new System.Windows.Forms.Padding(7);
            this.RBNvisitor.Name = "RBNvisitor";
            this.RBNvisitor.Size = new System.Drawing.Size(58, 17);
            this.RBNvisitor.TabIndex = 2;
            this.RBNvisitor.Text = "Visitors";
            this.RBNvisitor.UseVisualStyleBackColor = true;
            this.RBNvisitor.CheckedChanged += new System.EventHandler(this.RBNvisitor_CheckedChanged);
            this.RBNvisitor.Click += new System.EventHandler(this.RBNvisitor_CheckedChanged);
            // 
            // BTNsortnameplayers
            // 
            this.BTNsortnameplayers.Location = new System.Drawing.Point(3, 123);
            this.BTNsortnameplayers.Name = "BTNsortnameplayers";
            this.BTNsortnameplayers.Size = new System.Drawing.Size(191, 23);
            this.BTNsortnameplayers.TabIndex = 4;
            this.BTNsortnameplayers.Text = "Sort by Name";
            this.BTNsortnameplayers.UseVisualStyleBackColor = true;
            this.BTNsortnameplayers.Click += new System.EventHandler(this.BTNsortnameplayers_Click);
            // 
            // BTNsorttagplayers
            // 
            this.BTNsorttagplayers.Location = new System.Drawing.Point(3, 152);
            this.BTNsorttagplayers.Name = "BTNsorttagplayers";
            this.BTNsorttagplayers.Size = new System.Drawing.Size(191, 23);
            this.BTNsorttagplayers.TabIndex = 5;
            this.BTNsorttagplayers.Text = "Sort by Tag Number";
            this.BTNsorttagplayers.UseVisualStyleBackColor = true;
            this.BTNsorttagplayers.Click += new System.EventHandler(this.BTNsorttagplayers_Click);
            // 
            // pageMainHistory
            // 
            this.pageMainHistory.Controls.Add(this.SCTNhistory);
            this.pageMainHistory.Controls.Add(this.panel8);
            this.pageMainHistory.Controls.Add(this.panel4);
            this.pageMainHistory.Location = new System.Drawing.Point(4, 22);
            this.pageMainHistory.Name = "pageMainHistory";
            this.pageMainHistory.Size = new System.Drawing.Size(982, 468);
            this.pageMainHistory.TabIndex = 2;
            this.pageMainHistory.Text = "History";
            this.pageMainHistory.UseVisualStyleBackColor = true;
            // 
            // SCTNhistory
            // 
            this.SCTNhistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SCTNhistory.Location = new System.Drawing.Point(200, 62);
            this.SCTNhistory.Name = "SCTNhistory";
            this.SCTNhistory.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SCTNhistory.Panel1
            // 
            this.SCTNhistory.Panel1.Controls.Add(this.WEBhistory);
            // 
            // SCTNhistory.Panel2
            // 
            this.SCTNhistory.Panel2.Controls.Add(this.TBXhtml);
            this.SCTNhistory.Panel2Collapsed = true;
            this.SCTNhistory.Size = new System.Drawing.Size(782, 406);
            this.SCTNhistory.SplitterDistance = 203;
            this.SCTNhistory.TabIndex = 5;
            // 
            // WEBhistory
            // 
            this.WEBhistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WEBhistory.IsWebBrowserContextMenuEnabled = false;
            this.WEBhistory.Location = new System.Drawing.Point(0, 0);
            this.WEBhistory.MinimumSize = new System.Drawing.Size(20, 20);
            this.WEBhistory.Name = "WEBhistory";
            this.WEBhistory.Size = new System.Drawing.Size(782, 406);
            this.WEBhistory.TabIndex = 3;
            // 
            // TBXhtml
            // 
            this.TBXhtml.AcceptsTab = true;
            this.TBXhtml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TBXhtml.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TBXhtml.Location = new System.Drawing.Point(0, 0);
            this.TBXhtml.Multiline = true;
            this.TBXhtml.Name = "TBXhtml";
            this.TBXhtml.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TBXhtml.Size = new System.Drawing.Size(150, 46);
            this.TBXhtml.TabIndex = 0;
            this.TBXhtml.Text = "  ";
            this.TBXhtml.Leave += new System.EventHandler(this.TBXhtml_Leave);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.BTNexportplayersinday);
            this.panel8.Controls.Add(this.label11);
            this.panel8.Controls.Add(this.BTNprintHistory);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(200, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(782, 62);
            this.panel8.TabIndex = 4;
            // 
            // BTNexportplayersinday
            // 
            this.BTNexportplayersinday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNexportplayersinday.Location = new System.Drawing.Point(140, 33);
            this.BTNexportplayersinday.Name = "BTNexportplayersinday";
            this.BTNexportplayersinday.Size = new System.Drawing.Size(128, 23);
            this.BTNexportplayersinday.TabIndex = 9;
            this.BTNexportplayersinday.Text = "Export Players";
            this.BTNexportplayersinday.UseVisualStyleBackColor = true;
            this.BTNexportplayersinday.Click += new System.EventHandler(this.BTNexportplayersinday_Click);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(3, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(776, 27);
            this.label11.TabIndex = 8;
            this.label11.Text = "Select days from the left to look at them";
            // 
            // BTNprintHistory
            // 
            this.BTNprintHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BTNprintHistory.ContextMenuStrip = this.CMSeditHTML;
            this.BTNprintHistory.Location = new System.Drawing.Point(6, 33);
            this.BTNprintHistory.Name = "BTNprintHistory";
            this.BTNprintHistory.Size = new System.Drawing.Size(128, 23);
            this.BTNprintHistory.TabIndex = 7;
            this.BTNprintHistory.Text = "Print";
            this.BTNprintHistory.UseVisualStyleBackColor = true;
            this.BTNprintHistory.Click += new System.EventHandler(this.BTNprintHistory_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.LBXhistory);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(200, 468);
            this.panel4.TabIndex = 2;
            // 
            // LBXhistory
            // 
            this.LBXhistory.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.LBXhistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LBXhistory.ContextMenuStrip = this.CMSdeleteday;
            this.LBXhistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LBXhistory.FormattingEnabled = true;
            this.LBXhistory.IntegralHeight = false;
            this.LBXhistory.Location = new System.Drawing.Point(0, 0);
            this.LBXhistory.Name = "LBXhistory";
            this.LBXhistory.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.LBXhistory.Size = new System.Drawing.Size(200, 468);
            this.LBXhistory.TabIndex = 6;
            this.LBXhistory.SelectedIndexChanged += new System.EventHandler(this.LBXhistory_SelectedIndexChanged);
            // 
            // SFDhtml
            // 
            this.SFDhtml.FileName = "table.html";
            this.SFDhtml.Filter = "HTML file(*.html)|*.html, *.htm";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 602);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "Bowls Matchmaker";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.CMSdeleteday.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.CMNaddall.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.CMSdeleteplayer.ResumeLayout(false);
            this.CMSeditHTML.ResumeLayout(false);
            this.tabControlMain.ResumeLayout(false);
            this.pageMainMatch.ResumeLayout(false);
            this.tabControlMatches.ResumeLayout(false);
            this.pagePlayers.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.pageFixedMatches.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.pageConfirmBeforeMatches.ResumeLayout(false);
            this.pageConfirmBeforeMatches.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDfours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDtriples)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDpairs)).EndInit();
            this.panel9.ResumeLayout(false);
            this.pageViewMatch.ResumeLayout(false);
            this.SCTnewgames.Panel1.ResumeLayout(false);
            this.SCTnewgames.Panel2.ResumeLayout(false);
            this.SCTnewgames.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SCTnewgames)).EndInit();
            this.SCTnewgames.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.SideBarCreateDay.ResumeLayout(false);
            this.SideBarCreateDay.PerformLayout();
            this.pageMainSettings.ResumeLayout(false);
            this.pageMainSettings.PerformLayout();
            this.FLPweights.ResumeLayout(false);
            this.pageMainPlayers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).EndInit();
            this.panel2.ResumeLayout(false);
            this.SideBarPlayers.ResumeLayout(false);
            this.SideBarPlayers.PerformLayout();
            this.pageMainHistory.ResumeLayout(false);
            this.SCTNhistory.Panel1.ResumeLayout(false);
            this.SCTNhistory.Panel2.ResumeLayout(false);
            this.SCTNhistory.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SCTNhistory)).EndInit();
            this.SCTNhistory.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BTNnewgamesave;
        private System.Windows.Forms.Button BTNnewgamesgo;
        private System.Windows.Forms.ListBox LBXhistory;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ContextMenuStrip CMSdeleteday;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BTNmainhistory;
        private System.Windows.Forms.Button BTNmainPlayers;
        private System.Windows.Forms.Button BTNmainMatch;
        private CustomControls.TabControlWithoutHeader tabControlMain;
        private System.Windows.Forms.TabPage pageMainMatch;
        private CustomControls.TabControlWithoutHeader tabControlMatches;
        private System.Windows.Forms.TabPage pagePlayers;
        private System.Windows.Forms.TabPage pageFixedMatches;
        private System.Windows.Forms.TabPage pageConfirmBeforeMatches;
        private System.Windows.Forms.TabPage pageViewMatch;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.FlowLayoutPanel SideBarCreateDay;
        private System.Windows.Forms.Button BTNconfirmbeforecreating;
        private System.Windows.Forms.Button BTNfixedMatches;
        private System.Windows.Forms.Button BTNplayers;
        private System.Windows.Forms.TabPage pageMainPlayers;
        private System.Windows.Forms.FlowLayoutPanel SideBarPlayers;
        private System.Windows.Forms.TabPage pageMainHistory;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.CheckedListBox CLBpagePlayers;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label1;
        private CustomControls.WaterMarkTextBox TBXfilterPagePlayers;
        private System.Windows.Forms.Button BTNnewgame;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button BTNnewgameProblems;
        private System.Windows.Forms.WebBrowser WEBhistory;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource playerBindingSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer SCTnewgames;
        private System.Windows.Forms.WebBrowser WEBnewgames;
        private System.Windows.Forms.TextBox TBXnewdayConsole;
        private System.Windows.Forms.Button BTNprintHistory;
        private System.Windows.Forms.RadioButton RBNmember;
        private System.Windows.Forms.RadioButton RBNvisitor;
        private System.Windows.Forms.RadioButton RBNmemberorvisitor;
        private System.Windows.Forms.ContextMenuStrip CMNaddall;
        private System.Windows.Forms.ToolStripMenuItem addAllToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button BTNaddplayer;
        private System.Windows.Forms.Button BTNsortnameplayers;
        private System.Windows.Forms.Button BTNsorttagplayers;
        private System.Windows.Forms.TextBox TBXconfirmConsole;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.NumericUpDown NUDpairs;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown NUDtriples;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown NUDfours;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label LBLnummatcheswarning;
        private System.Windows.Forms.Button BTNsorttagmatches;
        private System.Windows.Forms.Button BTNsortnamematches;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button BTNexcelexport;
        private System.Windows.Forms.Button BTNexcelimport;
        private System.Windows.Forms.OpenFileDialog OFDexcel;
        private System.Windows.Forms.SaveFileDialog SFDexcel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip CMSdeleteplayer;
        private System.Windows.Forms.ToolStripMenuItem deletePlayerToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn tagDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn PositionPrimary;
        private System.Windows.Forms.DataGridViewComboBoxColumn GradePrimary;
        private System.Windows.Forms.DataGridViewComboBoxColumn PositionSecondary;
        private System.Windows.Forms.DataGridViewComboBoxColumn GradeSecondary;
        private System.Windows.Forms.DataGridViewComboBoxColumn preferredTeamSizesDataGridViewTextBoxColumn;
        private System.Windows.Forms.SplitContainer SCTNhistory;
        private System.Windows.Forms.TextBox TBXhtml;
        private System.Windows.Forms.WebBrowser WEBfixmatches;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button BTNaddfixedmatch;
        private CustomControls.SearchableComboBox SCBfixedmatchplayers;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BTNsettings;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage pageMainSettings;
        private CustomControls.AnchorFlowLayoutPanel FLPweights;
        private Controls.WeightView WVWpairsplaytogether;
        private Controls.WeightView WVWenemies;
        private Controls.WeightView WVWmainposition;
        private Controls.WeightView WVWsecondaryposition;
        private Controls.WeightView WVWbadpositiongoodgrade;
        private Controls.WeightView WVWunbalancedplayers;
        private Controls.WeightView WVWunbalancedteams;
        private Controls.WeightView WVWteamsize;
        private System.Windows.Forms.Button BTNresetWeights;
        private CustomControls.BrowseForFile BFFmain;
        private CustomControls.BrowseForFile BFFhtml;
        private System.Windows.Forms.Button BTNmaxfours;
        private System.Windows.Forms.Button BTNmaxtrips;
        private System.Windows.Forms.Button BTNmaxpairs;
        private System.Windows.Forms.Button BTNimportplayersforday;
        private System.Windows.Forms.Button BTNexportplayersinday;
        private System.Windows.Forms.OpenFileDialog OFDplayersinday;
        private System.Windows.Forms.SaveFileDialog SFDplayersinday;
        private System.Windows.Forms.ContextMenuStrip CMSeditHTML;
        private System.Windows.Forms.ToolStripMenuItem editHTMLToolStripMenuItem;
        private System.Windows.Forms.Button BTNseedefaulthtml;
        private System.Windows.Forms.SaveFileDialog SFDhtml;
    }
}

