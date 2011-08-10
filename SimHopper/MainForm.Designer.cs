namespace SimHopper
{
    partial class MainForm
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
            this.labelCurrentPool = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.labelElapsedTime = new System.Windows.Forms.Label();
            this.labelPropEarn = new System.Windows.Forms.Label();
            this.labelPropEff = new System.Windows.Forms.Label();
            this.labelPPSEarn = new System.Windows.Forms.Label();
            this.labelPoolInfo = new System.Windows.Forms.Label();
            this.labelPplnsEarn = new System.Windows.Forms.Label();
            this.labelPplnsEff = new System.Windows.Forms.Label();
            this.labelAdvPerTick = new System.Windows.Forms.Label();
            this.buttonSpeedDown = new System.Windows.Forms.Button();
            this.buttonSpeedUp = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTotalEarn = new System.Windows.Forms.Label();
            this.labelTotalEff = new System.Windows.Forms.Label();
            this.labelScoreEarn = new System.Windows.Forms.Label();
            this.labelScoreEff = new System.Windows.Forms.Label();
            this.buttonRound = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.checkBoxAuto = new System.Windows.Forms.CheckBox();
            this.labelSimulRound = new System.Windows.Forms.Label();
            this.labelHop = new System.Windows.Forms.Label();
            this.labelGeneration = new System.Windows.Forms.Label();
            this.dataGridPools = new System.Windows.Forms.DataGridView();
            this.Pool = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyShare = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RoundShareDelayed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RoundShareReal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DurationReal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Eff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Profit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPools)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCurrentPool
            // 
            this.labelCurrentPool.AutoSize = true;
            this.labelCurrentPool.Location = new System.Drawing.Point(12, 9);
            this.labelCurrentPool.Name = "labelCurrentPool";
            this.labelCurrentPool.Size = new System.Drawing.Size(83, 12);
            this.labelCurrentPool.TabIndex = 0;
            this.labelCurrentPool.Text = "Current Pool :";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // labelElapsedTime
            // 
            this.labelElapsedTime.AutoSize = true;
            this.labelElapsedTime.Location = new System.Drawing.Point(12, 67);
            this.labelElapsedTime.Name = "labelElapsedTime";
            this.labelElapsedTime.Size = new System.Drawing.Size(92, 12);
            this.labelElapsedTime.TabIndex = 1;
            this.labelElapsedTime.Text = "Elapsed Time :";
            // 
            // labelPropEarn
            // 
            this.labelPropEarn.AutoSize = true;
            this.labelPropEarn.Location = new System.Drawing.Point(12, 100);
            this.labelPropEarn.Name = "labelPropEarn";
            this.labelPropEarn.Size = new System.Drawing.Size(77, 12);
            this.labelPropEarn.TabIndex = 2;
            this.labelPropEarn.Text = "Prop. Earn : ";
            // 
            // labelPropEff
            // 
            this.labelPropEff.AutoSize = true;
            this.labelPropEff.Location = new System.Drawing.Point(12, 112);
            this.labelPropEff.Name = "labelPropEff";
            this.labelPropEff.Size = new System.Drawing.Size(65, 12);
            this.labelPropEff.TabIndex = 2;
            this.labelPropEff.Text = "Prop. Eff : ";
            // 
            // labelPPSEarn
            // 
            this.labelPPSEarn.AutoSize = true;
            this.labelPPSEarn.Location = new System.Drawing.Point(12, 184);
            this.labelPPSEarn.Name = "labelPPSEarn";
            this.labelPPSEarn.Size = new System.Drawing.Size(86, 12);
            this.labelPPSEarn.TabIndex = 2;
            this.labelPPSEarn.Text = "SMPPS Earn :";
            // 
            // labelPoolInfo
            // 
            this.labelPoolInfo.AutoSize = true;
            this.labelPoolInfo.Location = new System.Drawing.Point(12, 34);
            this.labelPoolInfo.Name = "labelPoolInfo";
            this.labelPoolInfo.Size = new System.Drawing.Size(30, 12);
            this.labelPoolInfo.TabIndex = 3;
            this.labelPoolInfo.Text = "info)";
            // 
            // labelPplnsEarn
            // 
            this.labelPplnsEarn.AutoSize = true;
            this.labelPplnsEarn.Location = new System.Drawing.Point(12, 136);
            this.labelPplnsEarn.Name = "labelPplnsEarn";
            this.labelPplnsEarn.Size = new System.Drawing.Size(87, 12);
            this.labelPplnsEarn.TabIndex = 2;
            this.labelPplnsEarn.Text = "PPLNS Earn : ";
            // 
            // labelPplnsEff
            // 
            this.labelPplnsEff.AutoSize = true;
            this.labelPplnsEff.Location = new System.Drawing.Point(12, 148);
            this.labelPplnsEff.Name = "labelPplnsEff";
            this.labelPplnsEff.Size = new System.Drawing.Size(75, 12);
            this.labelPplnsEff.TabIndex = 2;
            this.labelPplnsEff.Text = "PPLNS Eff : ";
            // 
            // labelAdvPerTick
            // 
            this.labelAdvPerTick.AutoSize = true;
            this.labelAdvPerTick.Location = new System.Drawing.Point(526, 235);
            this.labelAdvPerTick.Name = "labelAdvPerTick";
            this.labelAdvPerTick.Size = new System.Drawing.Size(29, 12);
            this.labelAdvPerTick.TabIndex = 4;
            this.labelAdvPerTick.Text = "1024";
            // 
            // buttonSpeedDown
            // 
            this.buttonSpeedDown.Location = new System.Drawing.Point(480, 229);
            this.buttonSpeedDown.Name = "buttonSpeedDown";
            this.buttonSpeedDown.Size = new System.Drawing.Size(40, 24);
            this.buttonSpeedDown.TabIndex = 5;
            this.buttonSpeedDown.Text = "<";
            this.buttonSpeedDown.UseVisualStyleBackColor = true;
            this.buttonSpeedDown.Click += new System.EventHandler(this.buttonSpeedDown_Click);
            // 
            // buttonSpeedUp
            // 
            this.buttonSpeedUp.Location = new System.Drawing.Point(570, 229);
            this.buttonSpeedUp.Name = "buttonSpeedUp";
            this.buttonSpeedUp.Size = new System.Drawing.Size(40, 24);
            this.buttonSpeedUp.TabIndex = 6;
            this.buttonSpeedUp.Text = ">";
            this.buttonSpeedUp.UseVisualStyleBackColor = true;
            this.buttonSpeedUp.Click += new System.EventHandler(this.buttonSpeedUp_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(502, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "Speed x";
            // 
            // labelTotalEarn
            // 
            this.labelTotalEarn.AutoSize = true;
            this.labelTotalEarn.Location = new System.Drawing.Point(12, 225);
            this.labelTotalEarn.Name = "labelTotalEarn";
            this.labelTotalEarn.Size = new System.Drawing.Size(71, 12);
            this.labelTotalEarn.TabIndex = 8;
            this.labelTotalEarn.Text = "Total Earn :";
            // 
            // labelTotalEff
            // 
            this.labelTotalEff.AutoSize = true;
            this.labelTotalEff.Location = new System.Drawing.Point(12, 241);
            this.labelTotalEff.Name = "labelTotalEff";
            this.labelTotalEff.Size = new System.Drawing.Size(59, 12);
            this.labelTotalEff.TabIndex = 9;
            this.labelTotalEff.Text = "Total Eff :";
            // 
            // labelScoreEarn
            // 
            this.labelScoreEarn.AutoSize = true;
            this.labelScoreEarn.Location = new System.Drawing.Point(260, 136);
            this.labelScoreEarn.Name = "labelScoreEarn";
            this.labelScoreEarn.Size = new System.Drawing.Size(80, 12);
            this.labelScoreEarn.TabIndex = 10;
            this.labelScoreEarn.Text = "Score Earn : ";
            // 
            // labelScoreEff
            // 
            this.labelScoreEff.AutoSize = true;
            this.labelScoreEff.Location = new System.Drawing.Point(260, 148);
            this.labelScoreEff.Name = "labelScoreEff";
            this.labelScoreEff.Size = new System.Drawing.Size(68, 12);
            this.labelScoreEff.TabIndex = 11;
            this.labelScoreEff.Text = "Score Eff : ";
            // 
            // buttonRound
            // 
            this.buttonRound.Enabled = false;
            this.buttonRound.Location = new System.Drawing.Point(479, 173);
            this.buttonRound.Name = "buttonRound";
            this.buttonRound.Size = new System.Drawing.Size(131, 23);
            this.buttonRound.TabIndex = 12;
            this.buttonRound.Text = "One Round";
            this.buttonRound.UseVisualStyleBackColor = true;
            this.buttonRound.Click += new System.EventHandler(this.buttonRound_Click);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Location = new System.Drawing.Point(12, 542);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(598, 103);
            this.textBoxLog.TabIndex = 13;
            // 
            // checkBoxAuto
            // 
            this.checkBoxAuto.AutoSize = true;
            this.checkBoxAuto.Checked = true;
            this.checkBoxAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAuto.Location = new System.Drawing.Point(480, 151);
            this.checkBoxAuto.Name = "checkBoxAuto";
            this.checkBoxAuto.Size = new System.Drawing.Size(75, 16);
            this.checkBoxAuto.TabIndex = 14;
            this.checkBoxAuto.Text = "Auto Run";
            this.checkBoxAuto.UseVisualStyleBackColor = true;
            this.checkBoxAuto.CheckedChanged += new System.EventHandler(this.checkBoxAuto_CheckedChanged);
            // 
            // labelSimulRound
            // 
            this.labelSimulRound.AutoSize = true;
            this.labelSimulRound.Location = new System.Drawing.Point(477, 127);
            this.labelSimulRound.Name = "labelSimulRound";
            this.labelSimulRound.Size = new System.Drawing.Size(114, 12);
            this.labelSimulRound.TabIndex = 15;
            this.labelSimulRound.Text = "Simulation Round #";
            // 
            // labelHop
            // 
            this.labelHop.AutoSize = true;
            this.labelHop.Location = new System.Drawing.Point(260, 184);
            this.labelHop.Name = "labelHop";
            this.labelHop.Size = new System.Drawing.Size(39, 12);
            this.labelHop.TabIndex = 16;
            this.labelHop.Text = "Hop : ";
            // 
            // labelGeneration
            // 
            this.labelGeneration.AutoSize = true;
            this.labelGeneration.Location = new System.Drawing.Point(477, 112);
            this.labelGeneration.Name = "labelGeneration";
            this.labelGeneration.Size = new System.Drawing.Size(94, 12);
            this.labelGeneration.TabIndex = 17;
            this.labelGeneration.Text = "Generation Title";
            // 
            // dataGridPools
            // 
            this.dataGridPools.AllowUserToAddRows = false;
            this.dataGridPools.AllowUserToDeleteRows = false;
            this.dataGridPools.AllowUserToResizeColumns = false;
            this.dataGridPools.AllowUserToResizeRows = false;
            this.dataGridPools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridPools.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridPools.CausesValidation = false;
            this.dataGridPools.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            this.dataGridPools.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridPools.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridPools.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Pool,
            this.MyShare,
            this.RoundShareDelayed,
            this.RoundShareReal,
            this.DurationReal,
            this.Eff,
            this.Profit});
            this.dataGridPools.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridPools.EnableHeadersVisualStyles = false;
            this.dataGridPools.Location = new System.Drawing.Point(12, 268);
            this.dataGridPools.MultiSelect = false;
            this.dataGridPools.Name = "dataGridPools";
            this.dataGridPools.ReadOnly = true;
            this.dataGridPools.RowHeadersVisible = false;
            this.dataGridPools.RowHeadersWidth = 20;
            this.dataGridPools.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridPools.RowTemplate.Height = 23;
            this.dataGridPools.ShowCellErrors = false;
            this.dataGridPools.ShowCellToolTips = false;
            this.dataGridPools.ShowEditingIcon = false;
            this.dataGridPools.ShowRowErrors = false;
            this.dataGridPools.Size = new System.Drawing.Size(598, 268);
            this.dataGridPools.TabIndex = 18;
            // 
            // Pool
            // 
            this.Pool.HeaderText = "Pool";
            this.Pool.Name = "Pool";
            this.Pool.ReadOnly = true;
            this.Pool.Width = 85;
            // 
            // MyShare
            // 
            this.MyShare.HeaderText = "MyShare";
            this.MyShare.Name = "MyShare";
            this.MyShare.ReadOnly = true;
            this.MyShare.Width = 86;
            // 
            // RoundShareDelayed
            // 
            this.RoundShareDelayed.HeaderText = "RS (delay)";
            this.RoundShareDelayed.Name = "RoundShareDelayed";
            this.RoundShareDelayed.ReadOnly = true;
            this.RoundShareDelayed.Width = 85;
            // 
            // RoundShareReal
            // 
            this.RoundShareReal.HeaderText = "RS (real)";
            this.RoundShareReal.Name = "RoundShareReal";
            this.RoundShareReal.ReadOnly = true;
            this.RoundShareReal.Width = 85;
            // 
            // DurationReal
            // 
            this.DurationReal.HeaderText = "Duration(R)";
            this.DurationReal.Name = "DurationReal";
            this.DurationReal.ReadOnly = true;
            this.DurationReal.Width = 85;
            // 
            // Eff
            // 
            this.Eff.HeaderText = "Eff";
            this.Eff.Name = "Eff";
            this.Eff.ReadOnly = true;
            this.Eff.Width = 86;
            // 
            // Profit
            // 
            this.Profit.HeaderText = "Profit";
            this.Profit.Name = "Profit";
            this.Profit.ReadOnly = true;
            this.Profit.Width = 85;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 657);
            this.Controls.Add(this.dataGridPools);
            this.Controls.Add(this.labelGeneration);
            this.Controls.Add(this.labelHop);
            this.Controls.Add(this.labelSimulRound);
            this.Controls.Add(this.checkBoxAuto);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.buttonRound);
            this.Controls.Add(this.labelScoreEff);
            this.Controls.Add(this.labelScoreEarn);
            this.Controls.Add(this.labelTotalEff);
            this.Controls.Add(this.labelTotalEarn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSpeedUp);
            this.Controls.Add(this.buttonSpeedDown);
            this.Controls.Add(this.labelAdvPerTick);
            this.Controls.Add(this.labelPoolInfo);
            this.Controls.Add(this.labelPPSEarn);
            this.Controls.Add(this.labelPropEff);
            this.Controls.Add(this.labelPplnsEff);
            this.Controls.Add(this.labelPplnsEarn);
            this.Controls.Add(this.labelPropEarn);
            this.Controls.Add(this.labelElapsedTime);
            this.Controls.Add(this.labelCurrentPool);
            this.Name = "MainForm";
            this.Text = "SimHopper by ryouiki";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPools)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCurrentPool;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label labelElapsedTime;
        private System.Windows.Forms.Label labelPropEarn;
        private System.Windows.Forms.Label labelPropEff;
        private System.Windows.Forms.Label labelPPSEarn;
        private System.Windows.Forms.Label labelPoolInfo;
        private System.Windows.Forms.Label labelPplnsEarn;
        private System.Windows.Forms.Label labelPplnsEff;
        private System.Windows.Forms.Label labelAdvPerTick;
        private System.Windows.Forms.Button buttonSpeedDown;
        private System.Windows.Forms.Button buttonSpeedUp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTotalEarn;
        private System.Windows.Forms.Label labelTotalEff;
        private System.Windows.Forms.Label labelScoreEarn;
        private System.Windows.Forms.Label labelScoreEff;
        private System.Windows.Forms.Button buttonRound;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.CheckBox checkBoxAuto;
        private System.Windows.Forms.Label labelSimulRound;
        private System.Windows.Forms.Label labelHop;
        private System.Windows.Forms.Label labelGeneration;
        private System.Windows.Forms.DataGridView dataGridPools;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pool;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyShare;
        private System.Windows.Forms.DataGridViewTextBoxColumn RoundShareDelayed;
        private System.Windows.Forms.DataGridViewTextBoxColumn RoundShareReal;
        private System.Windows.Forms.DataGridViewTextBoxColumn DurationReal;
        private System.Windows.Forms.DataGridViewTextBoxColumn Eff;
        private System.Windows.Forms.DataGridViewTextBoxColumn Profit;
    }
}

