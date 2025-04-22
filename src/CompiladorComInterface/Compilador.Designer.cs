namespace CompiladorComInterface
{
    partial class Compilador
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
            txtCodigo = new TextBox();
            btnAnalisar = new Button();
            dgvTokens = new DataGridView();
            ColumnCodigo = new DataGridViewTextBoxColumn();
            ColumnLexema = new DataGridViewTextBoxColumn();
            ColumnLinha = new DataGridViewTextBoxColumn();
            lstErros = new ListBox();
            ((System.ComponentModel.ISupportInitialize)dgvTokens).BeginInit();
            SuspendLayout();
            // 
            // txtCodigo
            // 
            txtCodigo.Dock = DockStyle.Top;
            txtCodigo.Location = new Point(0, 0);
            txtCodigo.Multiline = true;
            txtCodigo.Name = "txtCodigo";
            txtCodigo.ScrollBars = ScrollBars.Vertical;
            txtCodigo.Size = new Size(1346, 280);
            txtCodigo.TabIndex = 0;
            // 
            // btnAnalisar
            // 
            btnAnalisar.Dock = DockStyle.Top;
            btnAnalisar.Location = new Point(0, 280);
            btnAnalisar.Name = "btnAnalisar";
            btnAnalisar.Size = new Size(1346, 23);
            btnAnalisar.TabIndex = 1;
            btnAnalisar.Text = "Analisar";
            btnAnalisar.UseVisualStyleBackColor = true;
            btnAnalisar.Click += btnAnalisar_Click;
            // 
            // dgvTokens
            // 
            dgvTokens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvTokens.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTokens.Columns.AddRange(new DataGridViewColumn[] { ColumnCodigo, ColumnLexema, ColumnLinha });
            dgvTokens.Cursor = Cursors.No;
            dgvTokens.Dock = DockStyle.Left;
            dgvTokens.Location = new Point(0, 303);
            dgvTokens.Name = "dgvTokens";
            dgvTokens.Size = new Size(674, 301);
            dgvTokens.TabIndex = 3;
            dgvTokens.CellContentClick += dataGridView1_CellContentClick;
            // 
            // ColumnCodigo
            // 
            ColumnCodigo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ColumnCodigo.HeaderText = "Código";
            ColumnCodigo.Name = "ColumnCodigo";
            // 
            // ColumnLexema
            // 
            ColumnLexema.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ColumnLexema.HeaderText = "Lexema";
            ColumnLexema.Name = "ColumnLexema";
            // 
            // ColumnLinha
            // 
            ColumnLinha.HeaderText = "Linha";
            ColumnLinha.Name = "ColumnLinha";
            ColumnLinha.Width = 61;
            // 
            // lstErros
            // 
            lstErros.Dock = DockStyle.Right;
            lstErros.FormattingEnabled = true;
            lstErros.ItemHeight = 15;
            lstErros.Location = new Point(680, 303);
            lstErros.Name = "lstErros";
            lstErros.Size = new Size(666, 301);
            lstErros.TabIndex = 4;
            // 
            // Compilador
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1346, 604);
            Controls.Add(lstErros);
            Controls.Add(dgvTokens);
            Controls.Add(btnAnalisar);
            Controls.Add(txtCodigo);
            Name = "Compilador";
            Text = "Compilador";
            ((System.ComponentModel.ISupportInitialize)dgvTokens).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtCodigo;
        private Button btnAnalisar;
        private DataGridView dgvTokens;
        private DataGridViewTextBoxColumn ColumnCodigo;
        private DataGridViewTextBoxColumn ColumnLexema;
        private DataGridViewTextBoxColumn ColumnLinha;
        private ListBox lstErros;
    }
}
