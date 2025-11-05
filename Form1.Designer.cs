namespace ElevatorControlSystem
{
    partial class Form1
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                
                // Dispose custom timers to prevent memory leaks
                elevatorMoveTimer?.Dispose();
                doorTimer?.Dispose();
                doorAutoCloseTimer?.Dispose();
                emergencyAlarmTimer?.Dispose();
                emergencyAudioTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// 
        /// REALISTIC ELEVATOR CONTROL PANEL GUI
        /// Matches the provided elevator panel image design
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelControlPanel = new System.Windows.Forms.Panel();
            this.lblFloorDisplay = new System.Windows.Forms.Label();
            this.btnFloor1 = new System.Windows.Forms.Button();
            this.btnFloorG = new System.Windows.Forms.Button();
            this.btnDoorClose = new System.Windows.Forms.Button();
            this.btnDoorOpen = new System.Windows.Forms.Button();
            this.btnAlarm = new System.Windows.Forms.Button();
            this.panelCallButtons = new System.Windows.Forms.Panel();
            this.lblCallButtons = new System.Windows.Forms.Label();
            this.btnCallUp = new System.Windows.Forms.Button();
            this.btnCallDown = new System.Windows.Forms.Button();
            this.panelElevatorShaft = new System.Windows.Forms.Panel();
            this.panelElevatorCab = new System.Windows.Forms.Panel();
            this.panelDoorLeft = new System.Windows.Forms.Panel();
            this.panelDoorRight = new System.Windows.Forms.Panel();
            this.lblDisplay = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnViewLog = new System.Windows.Forms.Button();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panelMain.SuspendLayout();
            this.panelControlPanel.SuspendLayout();
            this.panelCallButtons.SuspendLayout();
            this.panelElevatorShaft.SuspendLayout();
            this.panelElevatorCab.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.panelMain.Controls.Add(this.panelControlPanel);
            this.panelMain.Controls.Add(this.panelCallButtons);
            this.panelMain.Controls.Add(this.panelElevatorShaft);
            this.panelMain.Controls.Add(this.lblDisplay);
            this.panelMain.Controls.Add(this.panelBottom);
            this.panelMain.Controls.Add(this.lblTitle);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1253, 807);
            this.panelMain.TabIndex = 0;
            // 
            // panelControlPanel
            // 
            this.panelControlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(125)))), ((int)(((byte)(107)))));
            this.panelControlPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelControlPanel.Controls.Add(this.lblFloorDisplay);
            this.panelControlPanel.Controls.Add(this.btnFloor1);
            this.panelControlPanel.Controls.Add(this.btnFloorG);
            this.panelControlPanel.Controls.Add(this.btnDoorClose);
            this.panelControlPanel.Controls.Add(this.btnDoorOpen);
            this.panelControlPanel.Controls.Add(this.btnAlarm);
            this.panelControlPanel.Location = new System.Drawing.Point(40, 110);
            this.panelControlPanel.Name = "panelControlPanel";
            this.panelControlPanel.Size = new System.Drawing.Size(300, 580);
            this.panelControlPanel.TabIndex = 5;
            this.panelControlPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panelControlPanel_Paint);
            // 
            // lblFloorDisplay
            // 
            this.lblFloorDisplay.BackColor = System.Drawing.Color.Black;
            this.lblFloorDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFloorDisplay.Font = new System.Drawing.Font("Arial", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFloorDisplay.ForeColor = System.Drawing.Color.Lime;
            this.lblFloorDisplay.Location = new System.Drawing.Point(75, 30);
            this.lblFloorDisplay.Name = "lblFloorDisplay";
            this.lblFloorDisplay.Size = new System.Drawing.Size(150, 120);
            this.lblFloorDisplay.TabIndex = 0;
            this.lblFloorDisplay.Text = "1";
            this.lblFloorDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblFloorDisplay, "Current Floor Display");
            // 
            // btnFloor1
            // 
            this.btnFloor1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnFloor1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnFloor1.FlatAppearance.BorderSize = 3;
            this.btnFloor1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFloor1.Font = new System.Drawing.Font("Arial", 36F, System.Drawing.FontStyle.Bold);
            this.btnFloor1.ForeColor = System.Drawing.Color.White;
            this.btnFloor1.Location = new System.Drawing.Point(75, 170);
            this.btnFloor1.Name = "btnFloor1";
            this.btnFloor1.Size = new System.Drawing.Size(150, 85);
            this.btnFloor1.TabIndex = 1;
            this.btnFloor1.Text = "1";
            this.toolTip1.SetToolTip(this.btnFloor1, "Go to Floor 1");
            this.btnFloor1.UseVisualStyleBackColor = false;
            this.btnFloor1.Click += new System.EventHandler(this.btnFloor1_Click);
            // 
            // btnFloorG
            // 
            this.btnFloorG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnFloorG.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnFloorG.FlatAppearance.BorderSize = 3;
            this.btnFloorG.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFloorG.Font = new System.Drawing.Font("Arial", 36F, System.Drawing.FontStyle.Bold);
            this.btnFloorG.ForeColor = System.Drawing.Color.White;
            this.btnFloorG.Location = new System.Drawing.Point(75, 270);
            this.btnFloorG.Name = "btnFloorG";
            this.btnFloorG.Size = new System.Drawing.Size(150, 85);
            this.btnFloorG.TabIndex = 2;
            this.btnFloorG.Text = "G";
            this.toolTip1.SetToolTip(this.btnFloorG, "Go to Ground Floor");
            this.btnFloorG.UseVisualStyleBackColor = false;
            this.btnFloorG.Click += new System.EventHandler(this.btnFloorG_Click);
            // 
            // btnDoorClose
            // 
            this.btnDoorClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnDoorClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btnDoorClose.FlatAppearance.BorderSize = 2;
            this.btnDoorClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDoorClose.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.btnDoorClose.ForeColor = System.Drawing.Color.Black;
            this.btnDoorClose.Location = new System.Drawing.Point(25, 380);
            this.btnDoorClose.Name = "btnDoorClose";
            this.btnDoorClose.Size = new System.Drawing.Size(115, 85);
            this.btnDoorClose.TabIndex = 3;
            this.btnDoorClose.Text = "◄|►";
            this.toolTip1.SetToolTip(this.btnDoorClose, "Close Doors");
            this.btnDoorClose.UseVisualStyleBackColor = false;
            this.btnDoorClose.Click += new System.EventHandler(this.btnDoorClose_Click);
            // 
            // btnDoorOpen
            // 
            this.btnDoorOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnDoorOpen.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btnDoorOpen.FlatAppearance.BorderSize = 2;
            this.btnDoorOpen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDoorOpen.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.btnDoorOpen.ForeColor = System.Drawing.Color.Black;
            this.btnDoorOpen.Location = new System.Drawing.Point(160, 380);
            this.btnDoorOpen.Name = "btnDoorOpen";
            this.btnDoorOpen.Size = new System.Drawing.Size(115, 85);
            this.btnDoorOpen.TabIndex = 4;
            this.btnDoorOpen.Text = "►|◄";
            this.toolTip1.SetToolTip(this.btnDoorOpen, "Open Doors");
            this.btnDoorOpen.UseVisualStyleBackColor = false;
            this.btnDoorOpen.Click += new System.EventHandler(this.btnDoorOpen_Click);
            // 
            // btnAlarm
            // 
            this.btnAlarm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnAlarm.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnAlarm.FlatAppearance.BorderSize = 3;
            this.btnAlarm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlarm.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.btnAlarm.ForeColor = System.Drawing.Color.White;
            this.btnAlarm.Location = new System.Drawing.Point(75, 490);
            this.btnAlarm.Name = "btnAlarm";
            this.btnAlarm.Size = new System.Drawing.Size(150, 65);
            this.btnAlarm.TabIndex = 5;
            this.btnAlarm.Text = "🔔";
            this.toolTip1.SetToolTip(this.btnAlarm, "Emergency Alarm");
            this.btnAlarm.UseVisualStyleBackColor = false;
            this.btnAlarm.Click += new System.EventHandler(this.btnAlarm_Click);
            // 
            // panelCallButtons
            // 
            this.panelCallButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.panelCallButtons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCallButtons.Controls.Add(this.lblCallButtons);
            this.panelCallButtons.Controls.Add(this.btnCallUp);
            this.panelCallButtons.Controls.Add(this.btnCallDown);
            this.panelCallButtons.Location = new System.Drawing.Point(365, 280);
            this.panelCallButtons.Name = "panelCallButtons";
            this.panelCallButtons.Size = new System.Drawing.Size(100, 250);
            this.panelCallButtons.TabIndex = 6;
            // 
            // lblCallButtons
            // 
            this.lblCallButtons.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCallButtons.ForeColor = System.Drawing.Color.Black;
            this.lblCallButtons.Location = new System.Drawing.Point(5, 10);
            this.lblCallButtons.Name = "lblCallButtons";
            this.lblCallButtons.Size = new System.Drawing.Size(90, 30);
            this.lblCallButtons.TabIndex = 2;
            this.lblCallButtons.Text = "CALL ELEVATOR";
            this.lblCallButtons.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCallUp
            // 
            this.btnCallUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnCallUp.FlatAppearance.BorderSize = 0;
            this.btnCallUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCallUp.Font = new System.Drawing.Font("Arial", 32F, System.Drawing.FontStyle.Bold);
            this.btnCallUp.ForeColor = System.Drawing.Color.White;
            this.btnCallUp.Location = new System.Drawing.Point(10, 50);
            this.btnCallUp.Name = "btnCallUp";
            this.btnCallUp.Size = new System.Drawing.Size(80, 80);
            this.btnCallUp.TabIndex = 0;
            this.btnCallUp.Text = "▲";
            this.toolTip1.SetToolTip(this.btnCallUp, "Call Elevator Up");
            this.btnCallUp.UseVisualStyleBackColor = false;
            this.btnCallUp.Click += new System.EventHandler(this.btnCallUp_Click);
            // 
            // btnCallDown
            // 
            this.btnCallDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnCallDown.FlatAppearance.BorderSize = 0;
            this.btnCallDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCallDown.Font = new System.Drawing.Font("Arial", 32F, System.Drawing.FontStyle.Bold);
            this.btnCallDown.ForeColor = System.Drawing.Color.White;
            this.btnCallDown.Location = new System.Drawing.Point(10, 150);
            this.btnCallDown.Name = "btnCallDown";
            this.btnCallDown.Size = new System.Drawing.Size(80, 80);
            this.btnCallDown.TabIndex = 1;
            this.btnCallDown.Text = "▼";
            this.toolTip1.SetToolTip(this.btnCallDown, "Call Elevator Down");
            this.btnCallDown.UseVisualStyleBackColor = false;
            this.btnCallDown.Click += new System.EventHandler(this.btnCallDown_Click);
            // 
            // panelElevatorShaft
            // 
            this.panelElevatorShaft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.panelElevatorShaft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelElevatorShaft.Controls.Add(this.panelElevatorCab);
            this.panelElevatorShaft.Location = new System.Drawing.Point(490, 110);
            this.panelElevatorShaft.Name = "panelElevatorShaft";
            this.panelElevatorShaft.Size = new System.Drawing.Size(280, 580);
            this.panelElevatorShaft.TabIndex = 4;
            this.panelElevatorShaft.Paint += new System.Windows.Forms.PaintEventHandler(this.panelElevatorShaft_Paint);
            // 
            // panelElevatorCab
            // 
            this.panelElevatorCab.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.panelElevatorCab.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelElevatorCab.Controls.Add(this.panelDoorLeft);
            this.panelElevatorCab.Controls.Add(this.panelDoorRight);
            this.panelElevatorCab.Location = new System.Drawing.Point(15, 420);
            this.panelElevatorCab.Name = "panelElevatorCab";
            this.panelElevatorCab.Size = new System.Drawing.Size(250, 150);
            this.panelElevatorCab.TabIndex = 0;
            this.panelElevatorCab.Paint += new System.Windows.Forms.PaintEventHandler(this.panelElevatorCab_Paint);
            // 
            // panelDoorLeft
            // 
            this.panelDoorLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.panelDoorLeft.Location = new System.Drawing.Point(0, 0);
            this.panelDoorLeft.Name = "panelDoorLeft";
            this.panelDoorLeft.Size = new System.Drawing.Size(125, 150);
            this.panelDoorLeft.TabIndex = 0;
            this.panelDoorLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDoorLeft_Paint);
            // 
            // panelDoorRight
            // 
            this.panelDoorRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.panelDoorRight.Location = new System.Drawing.Point(125, 0);
            this.panelDoorRight.Name = "panelDoorRight";
            this.panelDoorRight.Size = new System.Drawing.Size(125, 150);
            this.panelDoorRight.TabIndex = 1;
            this.panelDoorRight.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDoorRight_Paint);
            // 
            // lblDisplay
            // 
            this.lblDisplay.BackColor = System.Drawing.Color.Black;
            this.lblDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDisplay.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold);
            this.lblDisplay.ForeColor = System.Drawing.Color.Lime;
            this.lblDisplay.Location = new System.Drawing.Point(800, 160);
            this.lblDisplay.Name = "lblDisplay";
            this.lblDisplay.Padding = new System.Windows.Forms.Padding(20);
            this.lblDisplay.Size = new System.Drawing.Size(410, 450);
            this.lblDisplay.TabIndex = 2;
            this.lblDisplay.Text = "ELEVATOR CONTROL SYSTEM\r\n\r\nCurrent Floor: 1\r\nStatus: IDLE\r\nDoors: CLOSED\r\n\r\nReady" +
    " for operation";
            this.toolTip1.SetToolTip(this.lblDisplay, "System Status Display");
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.panelBottom.Controls.Add(this.btnQuit);
            this.panelBottom.Controls.Add(this.btnClearLog);
            this.panelBottom.Controls.Add(this.btnViewLog);
            this.panelBottom.Controls.Add(this.lblDateTime);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 727);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1253, 80);
            this.panelBottom.TabIndex = 3;
            // 
            // btnQuit
            // 
            this.btnQuit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnQuit.FlatAppearance.BorderSize = 0;
            this.btnQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuit.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnQuit.ForeColor = System.Drawing.Color.White;
            this.btnQuit.Location = new System.Drawing.Point(750, 17);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(120, 45);
            this.btnQuit.TabIndex = 3;
            this.btnQuit.Text = "❌ Quit";
            this.toolTip1.SetToolTip(this.btnQuit, "Exit the application");
            this.btnQuit.UseVisualStyleBackColor = false;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnClearLog.FlatAppearance.BorderSize = 0;
            this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearLog.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnClearLog.ForeColor = System.Drawing.Color.Black;
            this.btnClearLog.Location = new System.Drawing.Point(595, 17);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(135, 45);
            this.btnClearLog.TabIndex = 2;
            this.btnClearLog.Text = "🗑️ Clear Log";
            this.toolTip1.SetToolTip(this.btnClearLog, "Clear operation history");
            this.btnClearLog.UseVisualStyleBackColor = false;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnViewLog
            // 
            this.btnViewLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnViewLog.FlatAppearance.BorderSize = 0;
            this.btnViewLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewLog.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnViewLog.ForeColor = System.Drawing.Color.White;
            this.btnViewLog.Location = new System.Drawing.Point(440, 17);
            this.btnViewLog.Name = "btnViewLog";
            this.btnViewLog.Size = new System.Drawing.Size(135, 45);
            this.btnViewLog.TabIndex = 1;
            this.btnViewLog.Text = "📋 View Log";
            this.toolTip1.SetToolTip(this.btnViewLog, "View operation history");
            this.btnViewLog.UseVisualStyleBackColor = false;
            this.btnViewLog.Click += new System.EventHandler(this.btnViewLog_Click);
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblDateTime.ForeColor = System.Drawing.Color.White;
            this.lblDateTime.Location = new System.Drawing.Point(30, 30);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(194, 20);
            this.lblDateTime.TabIndex = 0;
            this.lblDateTime.Text = "November 4, 2025 12:00 PM";
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(20, 15, 0, 15);
            this.lblTitle.Size = new System.Drawing.Size(1253, 70);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🏢 ELEVATOR CONTROL SYSTEM";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerClock
            // 
            this.timerClock.Enabled = true;
            this.timerClock.Interval = 1000;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1253, 807);
            this.Controls.Add(this.panelMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Elevator Control System";
            this.panelMain.ResumeLayout(false);
            this.panelControlPanel.ResumeLayout(false);
            this.panelCallButtons.ResumeLayout(false);
            this.panelElevatorShaft.ResumeLayout(false);
            this.panelElevatorCab.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelControlPanel;
        private System.Windows.Forms.Label lblFloorDisplay;
        private System.Windows.Forms.Button btnFloor1;
        private System.Windows.Forms.Button btnFloorG;
        private System.Windows.Forms.Button btnDoorClose;
        private System.Windows.Forms.Button btnDoorOpen;
        private System.Windows.Forms.Button btnAlarm;
        private System.Windows.Forms.Panel panelCallButtons;
        private System.Windows.Forms.Button btnCallUp;
        private System.Windows.Forms.Button btnCallDown;
        private System.Windows.Forms.Label lblCallButtons;
        private System.Windows.Forms.Panel panelElevatorShaft;
        private System.Windows.Forms.Panel panelElevatorCab;
        private System.Windows.Forms.Panel panelDoorLeft;
        private System.Windows.Forms.Panel panelDoorRight;
        private System.Windows.Forms.Label lblDisplay;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnViewLog;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Timer timerClock;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
