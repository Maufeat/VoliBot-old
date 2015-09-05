namespace VoliBot
{
    partial class VoliBot
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoliBot));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.accountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.championsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startVoliBotOLDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.exListBox1 = new VoliBots.exListBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.accountsToolStripMenuItem,
            this.donateToolStripMenuItem,
            this.championsToolStripMenuItem,
            this.groupsToolStripMenuItem,
            this.startVoliBotOLDToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(954, 39);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // accountsToolStripMenuItem
            // 
            this.accountsToolStripMenuItem.Enabled = false;
            this.accountsToolStripMenuItem.Image = global::VoliBot.Properties.Resources.profile7;
            this.accountsToolStripMenuItem.Name = "accountsToolStripMenuItem";
            this.accountsToolStripMenuItem.Size = new System.Drawing.Size(69, 35);
            this.accountsToolStripMenuItem.Text = "Accounts";
            this.accountsToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.accountsToolStripMenuItem.Click += new System.EventHandler(this.accountsToolStripMenuItem_Click);
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.donateToolStripMenuItem.Image = global::VoliBot.Properties.Resources.kings;
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            this.donateToolStripMenuItem.Size = new System.Drawing.Size(57, 35);
            this.donateToolStripMenuItem.Text = "Donate";
            this.donateToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.donateToolStripMenuItem_Click);
            // 
            // championsToolStripMenuItem
            // 
            this.championsToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.championsToolStripMenuItem.Enabled = false;
            this.championsToolStripMenuItem.Image = global::VoliBot.Properties.Resources.cogwheel8;
            this.championsToolStripMenuItem.Name = "championsToolStripMenuItem";
            this.championsToolStripMenuItem.Size = new System.Drawing.Size(96, 35);
            this.championsToolStripMenuItem.Text = "Default Config";
            this.championsToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.championsToolStripMenuItem.Click += new System.EventHandler(this.championsToolStripMenuItem_Click);
            // 
            // groupsToolStripMenuItem
            // 
            this.groupsToolStripMenuItem.Image = global::VoliBot.Properties.Resources.user73;
            this.groupsToolStripMenuItem.Name = "groupsToolStripMenuItem";
            this.groupsToolStripMenuItem.Size = new System.Drawing.Size(87, 35);
            this.groupsToolStripMenuItem.Text = "GroupQueue";
            this.groupsToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.groupsToolStripMenuItem.Click += new System.EventHandler(this.groupsToolStripMenuItem_Click);
            // 
            // startVoliBotOLDToolStripMenuItem
            // 
            this.startVoliBotOLDToolStripMenuItem.Enabled = false;
            this.startVoliBotOLDToolStripMenuItem.Image = global::VoliBot.Properties.Resources.personal5;
            this.startVoliBotOLDToolStripMenuItem.Name = "startVoliBotOLDToolStripMenuItem";
            this.startVoliBotOLDToolStripMenuItem.Size = new System.Drawing.Size(83, 35);
            this.startVoliBotOLDToolStripMenuItem.Text = "Console Bot";
            this.startVoliBotOLDToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.startVoliBotOLDToolStripMenuItem.Click += new System.EventHandler(this.startVoliBotOLDToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 646);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(954, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(90, 17);
            this.toolStripStatusLabel1.Text = "VoliBots Online:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(29, 17);
            this.toolStripStatusLabel2.Text = "N/A";
            // 
            // exListBox1
            // 
            this.exListBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.exListBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.exListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.exListBox1.FormattingEnabled = true;
            this.exListBox1.ItemHeight = 66;
            this.exListBox1.Location = new System.Drawing.Point(0, 39);
            this.exListBox1.Margin = new System.Windows.Forms.Padding(0);
            this.exListBox1.Name = "exListBox1";
            this.exListBox1.Size = new System.Drawing.Size(186, 607);
            this.exListBox1.TabIndex = 9;
            this.exListBox1.SelectedIndexChanged += new System.EventHandler(this.exListBox1_SelectedIndexChanged);
            this.exListBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.exListBox1_MouseDoubleClick);
            // 
            // VoliBot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 668);
            this.Controls.Add(this.exListBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "VoliBot";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VoliBot";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VoliBot_FormClosed);
            this.Load += new System.EventHandler(this.VoliBot_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem accountsToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private VoliBots.exListBox exListBox1;
        private System.Windows.Forms.ToolStripMenuItem championsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startVoliBotOLDToolStripMenuItem;
    }
}