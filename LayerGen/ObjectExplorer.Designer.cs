namespace LayerGen
{
    partial class ObjectExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectExplorer));
            this.clbTables = new System.Windows.Forms.CheckedListBox();
            this.clbViews = new System.Windows.Forms.CheckedListBox();
            this.btnTablesCheckAll = new System.Windows.Forms.Button();
            this.btnTablesDecheckAll = new System.Windows.Forms.Button();
            this.btnViewsCheckAll = new System.Windows.Forms.Button();
            this.btnViewsDecheckAll = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // clbTables
            // 
            this.clbTables.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.clbTables.CheckOnClick = true;
            this.clbTables.ForeColor = System.Drawing.Color.White;
            this.clbTables.FormattingEnabled = true;
            this.clbTables.Location = new System.Drawing.Point(19, 36);
            this.clbTables.Name = "clbTables";
            this.clbTables.Size = new System.Drawing.Size(285, 244);
            this.clbTables.TabIndex = 0;
            this.clbTables.ThreeDCheckBoxes = true;
            // 
            // clbViews
            // 
            this.clbViews.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.clbViews.CheckOnClick = true;
            this.clbViews.ForeColor = System.Drawing.Color.White;
            this.clbViews.FormattingEnabled = true;
            this.clbViews.Location = new System.Drawing.Point(324, 36);
            this.clbViews.Name = "clbViews";
            this.clbViews.Size = new System.Drawing.Size(285, 244);
            this.clbViews.TabIndex = 1;
            this.clbViews.ThreeDCheckBoxes = true;
            // 
            // btnTablesCheckAll
            // 
            this.btnTablesCheckAll.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTablesCheckAll.Location = new System.Drawing.Point(19, 286);
            this.btnTablesCheckAll.Name = "btnTablesCheckAll";
            this.btnTablesCheckAll.Size = new System.Drawing.Size(285, 23);
            this.btnTablesCheckAll.TabIndex = 2;
            this.btnTablesCheckAll.Text = "Select All";
            this.btnTablesCheckAll.UseVisualStyleBackColor = true;
            this.btnTablesCheckAll.Click += new System.EventHandler(this.btnTablesCheckAll_Click);
            // 
            // btnTablesDecheckAll
            // 
            this.btnTablesDecheckAll.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTablesDecheckAll.Location = new System.Drawing.Point(19, 315);
            this.btnTablesDecheckAll.Name = "btnTablesDecheckAll";
            this.btnTablesDecheckAll.Size = new System.Drawing.Size(285, 23);
            this.btnTablesDecheckAll.TabIndex = 3;
            this.btnTablesDecheckAll.Text = "Deselect All";
            this.btnTablesDecheckAll.UseVisualStyleBackColor = true;
            this.btnTablesDecheckAll.Click += new System.EventHandler(this.btnTablesDecheckAll_Click);
            // 
            // btnViewsCheckAll
            // 
            this.btnViewsCheckAll.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewsCheckAll.Location = new System.Drawing.Point(324, 286);
            this.btnViewsCheckAll.Name = "btnViewsCheckAll";
            this.btnViewsCheckAll.Size = new System.Drawing.Size(285, 23);
            this.btnViewsCheckAll.TabIndex = 4;
            this.btnViewsCheckAll.Text = "Select All";
            this.btnViewsCheckAll.UseVisualStyleBackColor = true;
            this.btnViewsCheckAll.Click += new System.EventHandler(this.btnViewsCheckAll_Click);
            // 
            // btnViewsDecheckAll
            // 
            this.btnViewsDecheckAll.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewsDecheckAll.Location = new System.Drawing.Point(324, 315);
            this.btnViewsDecheckAll.Name = "btnViewsDecheckAll";
            this.btnViewsDecheckAll.Size = new System.Drawing.Size(285, 23);
            this.btnViewsDecheckAll.TabIndex = 5;
            this.btnViewsDecheckAll.Text = "Deselect All";
            this.btnViewsDecheckAll.UseVisualStyleBackColor = true;
            this.btnViewsDecheckAll.Click += new System.EventHandler(this.btnViewsDecheckAll_Click);
            // 
            // btnDone
            // 
            this.btnDone.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDone.Location = new System.Drawing.Point(19, 344);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(590, 23);
            this.btnDone.TabIndex = 6;
            this.btnDone.Text = "Done!";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 16F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(128, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 26);
            this.label1.TabIndex = 7;
            this.label1.Text = "Tables";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 16F, ((System.Drawing.FontStyle)(((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic) 
                | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(422, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 26);
            this.label2.TabIndex = 8;
            this.label2.Text = "Views";
            // 
            // ObjectExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::LayerGen.Properties.Resources.AprilTile;
            this.ClientSize = new System.Drawing.Size(632, 382);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnViewsDecheckAll);
            this.Controls.Add(this.btnViewsCheckAll);
            this.Controls.Add(this.btnTablesDecheckAll);
            this.Controls.Add(this.btnTablesCheckAll);
            this.Controls.Add(this.clbViews);
            this.Controls.Add(this.clbTables);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ObjectExplorer";
            this.Text = "Object Explorer";
            this.Load += new System.EventHandler(this.ObjectExplorer_Load);
            this.Shown += new System.EventHandler(this.ObjectExplorer_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clbTables;
        private System.Windows.Forms.CheckedListBox clbViews;
        private System.Windows.Forms.Button btnTablesCheckAll;
        private System.Windows.Forms.Button btnTablesDecheckAll;
        private System.Windows.Forms.Button btnViewsCheckAll;
        private System.Windows.Forms.Button btnViewsDecheckAll;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}