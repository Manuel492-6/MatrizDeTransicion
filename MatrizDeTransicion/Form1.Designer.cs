namespace MatrizDeTransicion
{
    partial class Form1
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
            dgvMatriz = new DataGridView();
            txtEntrada = new TextBox();
            btnAnalizar = new Button();
            lstLog = new ListBox();
            lblEstado = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvMatriz).BeginInit();
            SuspendLayout();
            // 
            // dgvMatriz
            // 
            dgvMatriz.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMatriz.Location = new Point(12, 88);
            dgvMatriz.Name = "dgvMatriz";
            dgvMatriz.RowHeadersWidth = 51;
            dgvMatriz.Size = new Size(1279, 344);
            dgvMatriz.TabIndex = 0;
            // 
            // txtEntrada
            // 
            txtEntrada.Location = new Point(12, 41);
            txtEntrada.Name = "txtEntrada";
            txtEntrada.Size = new Size(524, 27);
            txtEntrada.TabIndex = 1;
            // 
            // btnAnalizar
            // 
            btnAnalizar.Location = new Point(542, 39);
            btnAnalizar.Name = "btnAnalizar";
            btnAnalizar.Size = new Size(94, 29);
            btnAnalizar.TabIndex = 2;
            btnAnalizar.Text = "Analizar";
            btnAnalizar.UseVisualStyleBackColor = true;
            btnAnalizar.Click += btnAnalizar_Click;
            // 
            // lstLog
            // 
            lstLog.FormattingEnabled = true;
            lstLog.Location = new Point(12, 438);
            lstLog.Name = "lstLog";
            lstLog.Size = new Size(413, 164);
            lstLog.TabIndex = 3;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.Location = new Point(12, 617);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(50, 20);
            lblEstado.TabIndex = 4;
            lblEstado.Text = "label1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(245, 20);
            label1.TabIndex = 5;
            label1.Text = "Ingrese el identificador para validar";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1295, 646);
            Controls.Add(label1);
            Controls.Add(lblEstado);
            Controls.Add(lstLog);
            Controls.Add(btnAnalizar);
            Controls.Add(txtEntrada);
            Controls.Add(dgvMatriz);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dgvMatriz).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvMatriz;
        private TextBox txtEntrada;
        private Button btnAnalizar;
        private ListBox lstLog;
        private Label lblEstado;
        private Label label1;
    }
}
