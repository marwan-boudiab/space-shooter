﻿namespace SpaceShooter
{
    partial class Game
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
            components = new System.ComponentModel.Container();
            GameTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // GameTimer
            // 
            GameTimer.Interval = 16;
            GameTimer.Tick += GameTimer_Tick;
            // 
            // Game
            // 
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(552, 253);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Game";
            Text = "Form1";
            Paint += GamePaintEvent;
            KeyDown += KeyIsDown;
            KeyUp += KeyIsUp;
            MouseDown += Game_MouseDown;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer GameTimer;
    }
}
