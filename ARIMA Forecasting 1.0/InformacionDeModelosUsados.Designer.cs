namespace DeclineR.Pantalla_Principal
{
    partial class InformacionDeModelosUsados
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InformacionDeModelosUsados));
            this.dataGridViewInformacionDeModelosUsados = new System.Windows.Forms.DataGridView();
            this.columnModelo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFechaInicial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnFechaUltimoDato = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnExpN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnDi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnProduccionInicial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnAIC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnR2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInformacionDeModelosUsados)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewInformacionDeModelosUsados
            // 
            this.dataGridViewInformacionDeModelosUsados.AllowUserToAddRows = false;
            this.dataGridViewInformacionDeModelosUsados.AllowUserToDeleteRows = false;
            this.dataGridViewInformacionDeModelosUsados.AllowUserToResizeRows = false;
            this.dataGridViewInformacionDeModelosUsados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewInformacionDeModelosUsados.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnModelo,
            this.columnFechaInicial,
            this.columnFechaUltimoDato,
            this.columnExpN,
            this.columnDi,
            this.columnProduccionInicial,
            this.columnAIC,
            this.columnR2});
            this.dataGridViewInformacionDeModelosUsados.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewInformacionDeModelosUsados.Name = "dataGridViewInformacionDeModelosUsados";
            this.dataGridViewInformacionDeModelosUsados.Size = new System.Drawing.Size(740, 147);
            this.dataGridViewInformacionDeModelosUsados.TabIndex = 0;
            this.dataGridViewInformacionDeModelosUsados.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewInformacionDeModelosUsados_KeyDown);
            // 
            // columnModelo
            // 
            this.columnModelo.HeaderText = "Modelo";
            this.columnModelo.Name = "columnModelo";
            this.columnModelo.ReadOnly = true;
            this.columnModelo.Width = 116;
            // 
            // columnFechaInicial
            // 
            this.columnFechaInicial.HeaderText = "Fecha primer dato usado";
            this.columnFechaInicial.Name = "columnFechaInicial";
            this.columnFechaInicial.ReadOnly = true;
            // 
            // columnFechaUltimoDato
            // 
            this.columnFechaUltimoDato.HeaderText = "Fecha ultimo dato usado";
            this.columnFechaUltimoDato.Name = "columnFechaUltimoDato";
            this.columnFechaUltimoDato.ReadOnly = true;
            // 
            // columnExpN
            // 
            this.columnExpN.HeaderText = "n";
            this.columnExpN.Name = "columnExpN";
            this.columnExpN.ReadOnly = true;
            this.columnExpN.Width = 116;
            // 
            // columnDi
            // 
            this.columnDi.HeaderText = "Di";
            this.columnDi.Name = "columnDi";
            this.columnDi.ReadOnly = true;
            this.columnDi.Width = 116;
            // 
            // columnProduccionInicial
            // 
            this.columnProduccionInicial.HeaderText = "Producción inicial";
            this.columnProduccionInicial.Name = "columnProduccionInicial";
            this.columnProduccionInicial.ReadOnly = true;
            this.columnProduccionInicial.Width = 117;
            // 
            // columnAIC
            // 
            this.columnAIC.HeaderText = "AIC";
            this.columnAIC.Name = "columnAIC";
            this.columnAIC.ReadOnly = true;
            this.columnAIC.Width = 116;
            // 
            // columnR2
            // 
            this.columnR2.HeaderText = "R cuadrado";
            this.columnR2.Name = "columnR2";
            this.columnR2.ReadOnly = true;
            this.columnR2.Width = 116;
            // 
            // InformacionDeModelosUsados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 147);
            this.Controls.Add(this.dataGridViewInformacionDeModelosUsados);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InformacionDeModelosUsados";
            this.Text = "Informacion de Modelos Usados";
            this.Load += new System.EventHandler(this.InformacionDeModelosUsados_Load);
            this.SizeChanged += new System.EventHandler(this.InformacionDeModelosUsados_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInformacionDeModelosUsados)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewInformacionDeModelosUsados;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnModelo;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFechaInicial;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnFechaUltimoDato;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnExpN;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnDi;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnProduccionInicial;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnAIC;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnR2;
    }
}