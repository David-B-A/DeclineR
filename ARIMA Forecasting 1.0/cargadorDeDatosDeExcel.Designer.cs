namespace DeclineR.Pantalla_Principal
{
    partial class CargadorDeDatosDeExcel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CargadorDeDatosDeExcel));
            this.dataGridViewCargadorDeDatos = new System.Windows.Forms.DataGridView();
            this.labelSeparadorDecimal = new System.Windows.Forms.Label();
            this.labelSeparadorDeMiles = new System.Windows.Forms.Label();
            this.comboBoxSeparadorDecimal = new System.Windows.Forms.ComboBox();
            this.comboBoxSeparadorDeMiles = new System.Windows.Forms.ComboBox();
            this.buttonAñadirRangoDeFechas = new System.Windows.Forms.Button();
            this.labelDesdeFecha = new System.Windows.Forms.Label();
            this.buttonDesde = new System.Windows.Forms.Button();
            this.labelHastaFecha = new System.Windows.Forms.Label();
            this.buttonHasta = new System.Windows.Forms.Button();
            this.comboBoxFormatoDeFecha = new System.Windows.Forms.ComboBox();
            this.labelFormatoDeFecha = new System.Windows.Forms.Label();
            this.buttonCargar = new System.Windows.Forms.Button();
            this.buttonAñadirRenglon = new System.Windows.Forms.Button();
            this.buttonEliminarRenglon = new System.Windows.Forms.Button();
            this.checkBoxDefinirRangoDeFechas = new System.Windows.Forms.CheckBox();
            this.buttonBorrarDatos = new System.Windows.Forms.Button();
            this.buttonAceptar = new System.Windows.Forms.Button();
            this.buttonPegarDatos = new System.Windows.Forms.Button();
            this.labelIntervaloDeTiempo = new System.Windows.Forms.Label();
            this.comboBoxIntervaloDeTiempo = new System.Windows.Forms.ComboBox();
            this.radioButtonIntervaloEnDias = new System.Windows.Forms.RadioButton();
            this.radioButtonIntervaloEnMeses = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCargadorDeDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewCargadorDeDatos
            // 
            this.dataGridViewCargadorDeDatos.AllowUserToAddRows = false;
            this.dataGridViewCargadorDeDatos.AllowUserToResizeRows = false;
            this.dataGridViewCargadorDeDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewCargadorDeDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCargadorDeDatos.Location = new System.Drawing.Point(12, 284);
            this.dataGridViewCargadorDeDatos.Name = "dataGridViewCargadorDeDatos";
            this.dataGridViewCargadorDeDatos.Size = new System.Drawing.Size(321, 210);
            this.dataGridViewCargadorDeDatos.TabIndex = 0;
            this.dataGridViewCargadorDeDatos.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewCargadorDeDatos_KeyDown);
            // 
            // labelSeparadorDecimal
            // 
            this.labelSeparadorDecimal.AutoSize = true;
            this.labelSeparadorDecimal.Location = new System.Drawing.Point(48, 139);
            this.labelSeparadorDecimal.Name = "labelSeparadorDecimal";
            this.labelSeparadorDecimal.Size = new System.Drawing.Size(98, 13);
            this.labelSeparadorDecimal.TabIndex = 1;
            this.labelSeparadorDecimal.Text = "Separador decimal:";
            // 
            // labelSeparadorDeMiles
            // 
            this.labelSeparadorDeMiles.AutoSize = true;
            this.labelSeparadorDeMiles.Location = new System.Drawing.Point(48, 167);
            this.labelSeparadorDeMiles.Name = "labelSeparadorDeMiles";
            this.labelSeparadorDeMiles.Size = new System.Drawing.Size(100, 13);
            this.labelSeparadorDeMiles.TabIndex = 2;
            this.labelSeparadorDeMiles.Text = "Separador de miles:";
            // 
            // comboBoxSeparadorDecimal
            // 
            this.comboBoxSeparadorDecimal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSeparadorDecimal.FormattingEnabled = true;
            this.comboBoxSeparadorDecimal.Items.AddRange(new object[] {
            "\",\" - Coma",
            "\".\" - Punto",
            "Ninguno"});
            this.comboBoxSeparadorDecimal.Location = new System.Drawing.Point(173, 136);
            this.comboBoxSeparadorDecimal.Name = "comboBoxSeparadorDecimal";
            this.comboBoxSeparadorDecimal.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSeparadorDecimal.TabIndex = 3;
            this.comboBoxSeparadorDecimal.SelectedIndexChanged += new System.EventHandler(this.comboBoxSeparadorDecimal_SelectedIndexChanged);
            // 
            // comboBoxSeparadorDeMiles
            // 
            this.comboBoxSeparadorDeMiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSeparadorDeMiles.FormattingEnabled = true;
            this.comboBoxSeparadorDeMiles.Items.AddRange(new object[] {
            "\",\" - Coma",
            "\" \" - Espacio",
            "\".\" - Punto",
            "Ninguno"});
            this.comboBoxSeparadorDeMiles.Location = new System.Drawing.Point(173, 164);
            this.comboBoxSeparadorDeMiles.Name = "comboBoxSeparadorDeMiles";
            this.comboBoxSeparadorDeMiles.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSeparadorDeMiles.TabIndex = 4;
            this.comboBoxSeparadorDeMiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxSeparadorDeMiles_SelectedIndexChanged);
            // 
            // buttonAñadirRangoDeFechas
            // 
            this.buttonAñadirRangoDeFechas.Enabled = false;
            this.buttonAñadirRangoDeFechas.Location = new System.Drawing.Point(274, 85);
            this.buttonAñadirRangoDeFechas.Name = "buttonAñadirRangoDeFechas";
            this.buttonAñadirRangoDeFechas.Size = new System.Drawing.Size(59, 23);
            this.buttonAñadirRangoDeFechas.TabIndex = 7;
            this.buttonAñadirRangoDeFechas.Text = "Añadir";
            this.buttonAñadirRangoDeFechas.UseVisualStyleBackColor = true;
            this.buttonAñadirRangoDeFechas.Click += new System.EventHandler(this.buttonAñadirRangoDeFechas_Click);
            // 
            // labelDesdeFecha
            // 
            this.labelDesdeFecha.AutoSize = true;
            this.labelDesdeFecha.Enabled = false;
            this.labelDesdeFecha.Location = new System.Drawing.Point(9, 90);
            this.labelDesdeFecha.Name = "labelDesdeFecha";
            this.labelDesdeFecha.Size = new System.Drawing.Size(41, 13);
            this.labelDesdeFecha.TabIndex = 9;
            this.labelDesdeFecha.Text = "Desde:";
            // 
            // buttonDesde
            // 
            this.buttonDesde.Enabled = false;
            this.buttonDesde.Location = new System.Drawing.Point(56, 85);
            this.buttonDesde.Name = "buttonDesde";
            this.buttonDesde.Size = new System.Drawing.Size(73, 23);
            this.buttonDesde.TabIndex = 10;
            this.buttonDesde.Text = "Buscar";
            this.buttonDesde.UseVisualStyleBackColor = true;
            this.buttonDesde.Click += new System.EventHandler(this.buttonDesde_Click);
            // 
            // labelHastaFecha
            // 
            this.labelHastaFecha.AutoSize = true;
            this.labelHastaFecha.Enabled = false;
            this.labelHastaFecha.Location = new System.Drawing.Point(135, 90);
            this.labelHastaFecha.Name = "labelHastaFecha";
            this.labelHastaFecha.Size = new System.Drawing.Size(38, 13);
            this.labelHastaFecha.TabIndex = 11;
            this.labelHastaFecha.Text = "Hasta:";
            // 
            // buttonHasta
            // 
            this.buttonHasta.Enabled = false;
            this.buttonHasta.Location = new System.Drawing.Point(173, 85);
            this.buttonHasta.Name = "buttonHasta";
            this.buttonHasta.Size = new System.Drawing.Size(73, 23);
            this.buttonHasta.TabIndex = 12;
            this.buttonHasta.Text = "Buscar";
            this.buttonHasta.UseVisualStyleBackColor = true;
            this.buttonHasta.Click += new System.EventHandler(this.buttonHasta_Click);
            // 
            // comboBoxFormatoDeFecha
            // 
            this.comboBoxFormatoDeFecha.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFormatoDeFecha.FormattingEnabled = true;
            this.comboBoxFormatoDeFecha.Items.AddRange(new object[] {
            "dd/MM/yyyy",
            "dd-MM-yyyy",
            "yyyy/MM/dd",
            "yyyy-MM-dd"});
            this.comboBoxFormatoDeFecha.Location = new System.Drawing.Point(173, 192);
            this.comboBoxFormatoDeFecha.Name = "comboBoxFormatoDeFecha";
            this.comboBoxFormatoDeFecha.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFormatoDeFecha.TabIndex = 14;
            this.comboBoxFormatoDeFecha.SelectedIndexChanged += new System.EventHandler(this.comboBoxFormatoDeFecha_SelectedIndexChanged);
            // 
            // labelFormatoDeFecha
            // 
            this.labelFormatoDeFecha.AutoSize = true;
            this.labelFormatoDeFecha.Location = new System.Drawing.Point(48, 195);
            this.labelFormatoDeFecha.Name = "labelFormatoDeFecha";
            this.labelFormatoDeFecha.Size = new System.Drawing.Size(93, 13);
            this.labelFormatoDeFecha.TabIndex = 13;
            this.labelFormatoDeFecha.Text = "Formato de fecha:";
            // 
            // buttonCargar
            // 
            this.buttonCargar.Location = new System.Drawing.Point(59, 510);
            this.buttonCargar.Name = "buttonCargar";
            this.buttonCargar.Size = new System.Drawing.Size(89, 23);
            this.buttonCargar.TabIndex = 5;
            this.buttonCargar.Text = "Cargar datos";
            this.buttonCargar.UseVisualStyleBackColor = true;
            this.buttonCargar.Click += new System.EventHandler(this.buttonCargar_Click);
            // 
            // buttonAñadirRenglon
            // 
            this.buttonAñadirRenglon.Font = new System.Drawing.Font("Elephant", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAñadirRenglon.ForeColor = System.Drawing.Color.Green;
            this.buttonAñadirRenglon.Location = new System.Drawing.Point(55, 230);
            this.buttonAñadirRenglon.Name = "buttonAñadirRenglon";
            this.buttonAñadirRenglon.Size = new System.Drawing.Size(23, 23);
            this.buttonAñadirRenglon.TabIndex = 15;
            this.buttonAñadirRenglon.Text = "+";
            this.buttonAñadirRenglon.UseVisualStyleBackColor = true;
            this.buttonAñadirRenglon.Click += new System.EventHandler(this.buttonAñadirRenglon_Click);
            // 
            // buttonEliminarRenglon
            // 
            this.buttonEliminarRenglon.Font = new System.Drawing.Font("Elephant", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEliminarRenglon.ForeColor = System.Drawing.Color.Red;
            this.buttonEliminarRenglon.Location = new System.Drawing.Point(84, 230);
            this.buttonEliminarRenglon.Name = "buttonEliminarRenglon";
            this.buttonEliminarRenglon.Size = new System.Drawing.Size(23, 23);
            this.buttonEliminarRenglon.TabIndex = 16;
            this.buttonEliminarRenglon.Text = "-";
            this.buttonEliminarRenglon.UseVisualStyleBackColor = true;
            this.buttonEliminarRenglon.Click += new System.EventHandler(this.buttonEliminarRenglon_Click);
            // 
            // checkBoxDefinirRangoDeFechas
            // 
            this.checkBoxDefinirRangoDeFechas.AutoSize = true;
            this.checkBoxDefinirRangoDeFechas.Location = new System.Drawing.Point(12, 12);
            this.checkBoxDefinirRangoDeFechas.Name = "checkBoxDefinirRangoDeFechas";
            this.checkBoxDefinirRangoDeFechas.Size = new System.Drawing.Size(136, 17);
            this.checkBoxDefinirRangoDeFechas.TabIndex = 17;
            this.checkBoxDefinirRangoDeFechas.Text = "Definir rango de fechas";
            this.checkBoxDefinirRangoDeFechas.UseVisualStyleBackColor = true;
            this.checkBoxDefinirRangoDeFechas.CheckedChanged += new System.EventHandler(this.checkBoxDefinirRangoDeFechas_CheckedChanged);
            // 
            // buttonBorrarDatos
            // 
            this.buttonBorrarDatos.Location = new System.Drawing.Point(123, 230);
            this.buttonBorrarDatos.Name = "buttonBorrarDatos";
            this.buttonBorrarDatos.Size = new System.Drawing.Size(75, 23);
            this.buttonBorrarDatos.TabIndex = 21;
            this.buttonBorrarDatos.Text = "Borrar datos";
            this.buttonBorrarDatos.UseVisualStyleBackColor = true;
            this.buttonBorrarDatos.Click += new System.EventHandler(this.buttonBorrarDatos_Click);
            // 
            // buttonAceptar
            // 
            this.buttonAceptar.Enabled = false;
            this.buttonAceptar.Location = new System.Drawing.Point(194, 510);
            this.buttonAceptar.Name = "buttonAceptar";
            this.buttonAceptar.Size = new System.Drawing.Size(89, 23);
            this.buttonAceptar.TabIndex = 22;
            this.buttonAceptar.Text = "Aceptar";
            this.buttonAceptar.UseVisualStyleBackColor = true;
            this.buttonAceptar.Click += new System.EventHandler(this.buttonAceptar_Click);
            // 
            // buttonPegarDatos
            // 
            this.buttonPegarDatos.Location = new System.Drawing.Point(213, 230);
            this.buttonPegarDatos.Name = "buttonPegarDatos";
            this.buttonPegarDatos.Size = new System.Drawing.Size(75, 23);
            this.buttonPegarDatos.TabIndex = 23;
            this.buttonPegarDatos.Text = "Pegar datos";
            this.buttonPegarDatos.UseVisualStyleBackColor = true;
            this.buttonPegarDatos.Click += new System.EventHandler(this.buttonPegarDatos_Click);
            // 
            // labelIntervaloDeTiempo
            // 
            this.labelIntervaloDeTiempo.AutoSize = true;
            this.labelIntervaloDeTiempo.Enabled = false;
            this.labelIntervaloDeTiempo.Location = new System.Drawing.Point(9, 38);
            this.labelIntervaloDeTiempo.Name = "labelIntervaloDeTiempo";
            this.labelIntervaloDeTiempo.Size = new System.Drawing.Size(205, 13);
            this.labelIntervaloDeTiempo.TabIndex = 24;
            this.labelIntervaloDeTiempo.Text = "Seleccione el intervalo de tiempo en días:";
            // 
            // comboBoxIntervaloDeTiempo
            // 
            this.comboBoxIntervaloDeTiempo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIntervaloDeTiempo.Enabled = false;
            this.comboBoxIntervaloDeTiempo.FormattingEnabled = true;
            this.comboBoxIntervaloDeTiempo.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30"});
            this.comboBoxIntervaloDeTiempo.Location = new System.Drawing.Point(274, 35);
            this.comboBoxIntervaloDeTiempo.Name = "comboBoxIntervaloDeTiempo";
            this.comboBoxIntervaloDeTiempo.Size = new System.Drawing.Size(59, 21);
            this.comboBoxIntervaloDeTiempo.TabIndex = 25;
            this.comboBoxIntervaloDeTiempo.SelectedIndexChanged += new System.EventHandler(this.comboBoxIntervaloDeTiempo_SelectedIndexChanged);
            // 
            // radioButtonIntervaloEnDias
            // 
            this.radioButtonIntervaloEnDias.AutoSize = true;
            this.radioButtonIntervaloEnDias.Checked = true;
            this.radioButtonIntervaloEnDias.Enabled = false;
            this.radioButtonIntervaloEnDias.Location = new System.Drawing.Point(12, 62);
            this.radioButtonIntervaloEnDias.Name = "radioButtonIntervaloEnDias";
            this.radioButtonIntervaloEnDias.Size = new System.Drawing.Size(105, 17);
            this.radioButtonIntervaloEnDias.TabIndex = 26;
            this.radioButtonIntervaloEnDias.TabStop = true;
            this.radioButtonIntervaloEnDias.Text = "Intervalo en días";
            this.radioButtonIntervaloEnDias.UseVisualStyleBackColor = true;
            this.radioButtonIntervaloEnDias.CheckedChanged += new System.EventHandler(this.radioButtonIntervaloEnDias_CheckedChanged);
            // 
            // radioButtonIntervaloEnMeses
            // 
            this.radioButtonIntervaloEnMeses.AutoSize = true;
            this.radioButtonIntervaloEnMeses.Enabled = false;
            this.radioButtonIntervaloEnMeses.Location = new System.Drawing.Point(138, 62);
            this.radioButtonIntervaloEnMeses.Name = "radioButtonIntervaloEnMeses";
            this.radioButtonIntervaloEnMeses.Size = new System.Drawing.Size(118, 17);
            this.radioButtonIntervaloEnMeses.TabIndex = 27;
            this.radioButtonIntervaloEnMeses.Text = "Intervalo de un mes";
            this.radioButtonIntervaloEnMeses.UseVisualStyleBackColor = true;
            // 
            // CargadorDeDatosDeExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 545);
            this.Controls.Add(this.radioButtonIntervaloEnMeses);
            this.Controls.Add(this.radioButtonIntervaloEnDias);
            this.Controls.Add(this.comboBoxIntervaloDeTiempo);
            this.Controls.Add(this.labelIntervaloDeTiempo);
            this.Controls.Add(this.buttonPegarDatos);
            this.Controls.Add(this.buttonAceptar);
            this.Controls.Add(this.buttonBorrarDatos);
            this.Controls.Add(this.checkBoxDefinirRangoDeFechas);
            this.Controls.Add(this.buttonEliminarRenglon);
            this.Controls.Add(this.buttonAñadirRenglon);
            this.Controls.Add(this.comboBoxFormatoDeFecha);
            this.Controls.Add(this.labelFormatoDeFecha);
            this.Controls.Add(this.buttonHasta);
            this.Controls.Add(this.labelHastaFecha);
            this.Controls.Add(this.buttonDesde);
            this.Controls.Add(this.labelDesdeFecha);
            this.Controls.Add(this.buttonAñadirRangoDeFechas);
            this.Controls.Add(this.buttonCargar);
            this.Controls.Add(this.comboBoxSeparadorDeMiles);
            this.Controls.Add(this.comboBoxSeparadorDecimal);
            this.Controls.Add(this.labelSeparadorDeMiles);
            this.Controls.Add(this.labelSeparadorDecimal);
            this.Controls.Add(this.dataGridViewCargadorDeDatos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CargadorDeDatosDeExcel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Copiar datos";
            this.Load += new System.EventHandler(this.cargadorDeDatosDeExcel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCargadorDeDatos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewCargadorDeDatos;
        private System.Windows.Forms.Label labelSeparadorDecimal;
        private System.Windows.Forms.Label labelSeparadorDeMiles;
        private System.Windows.Forms.ComboBox comboBoxSeparadorDecimal;
        private System.Windows.Forms.ComboBox comboBoxSeparadorDeMiles;
        private System.Windows.Forms.Button buttonAñadirRangoDeFechas;
        private System.Windows.Forms.Label labelDesdeFecha;
        private System.Windows.Forms.Button buttonDesde;
        private System.Windows.Forms.Label labelHastaFecha;
        private System.Windows.Forms.Button buttonHasta;
        private System.Windows.Forms.ComboBox comboBoxFormatoDeFecha;
        private System.Windows.Forms.Label labelFormatoDeFecha;
        private System.Windows.Forms.Button buttonCargar;
        private System.Windows.Forms.Button buttonAñadirRenglon;
        private System.Windows.Forms.Button buttonEliminarRenglon;
        private System.Windows.Forms.CheckBox checkBoxDefinirRangoDeFechas;
        private System.Windows.Forms.Button buttonBorrarDatos;
        private System.Windows.Forms.Button buttonAceptar;
        private System.Windows.Forms.Button buttonPegarDatos;
        private System.Windows.Forms.Label labelIntervaloDeTiempo;
        private System.Windows.Forms.ComboBox comboBoxIntervaloDeTiempo;
        private System.Windows.Forms.RadioButton radioButtonIntervaloEnDias;
        private System.Windows.Forms.RadioButton radioButtonIntervaloEnMeses;
    }
}