namespace DeclineR.Pantalla_Principal
{
    partial class CargadorDeArchivos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CargadorDeArchivos));
            this.buttonBuscarArchivo = new System.Windows.Forms.Button();
            this.labelNombreArchivo = new System.Windows.Forms.Label();
            this.openFileDialogBuscarArchivo = new System.Windows.Forms.OpenFileDialog();
            this.buttonBorrarDatos = new System.Windows.Forms.Button();
            this.buttonEliminarRenglon = new System.Windows.Forms.Button();
            this.buttonAñadirRenglon = new System.Windows.Forms.Button();
            this.comboBoxFormatoDeFecha = new System.Windows.Forms.ComboBox();
            this.labelFormatoDeFecha = new System.Windows.Forms.Label();
            this.comboBoxSeparadorDeMiles = new System.Windows.Forms.ComboBox();
            this.comboBoxSeparadorDecimal = new System.Windows.Forms.ComboBox();
            this.labelSeparadorDeMiles = new System.Windows.Forms.Label();
            this.labelSeparadorDecimal = new System.Windows.Forms.Label();
            this.dataGridViewCargadorDeDatos = new System.Windows.Forms.DataGridView();
            this.buttonCargar = new System.Windows.Forms.Button();
            this.labelColumnaDeFechas = new System.Windows.Forms.Label();
            this.labelColumnaDeDatos = new System.Windows.Forms.Label();
            this.comboBoxSeparadorColumnas = new System.Windows.Forms.ComboBox();
            this.labelSeparadorColumnas = new System.Windows.Forms.Label();
            this.textBoxOtroSeparadorColumnas = new System.Windows.Forms.TextBox();
            this.comboBoxColumnaFechas = new System.Windows.Forms.ComboBox();
            this.comboBoxColumnaDatos = new System.Windows.Forms.ComboBox();
            this.checkBoxTitulos = new System.Windows.Forms.CheckBox();
            this.labelOtroSeparadorColumnas = new System.Windows.Forms.Label();
            this.buttonVisualizar = new System.Windows.Forms.Button();
            this.buttonAceptar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCargadorDeDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonBuscarArchivo
            // 
            this.buttonBuscarArchivo.Location = new System.Drawing.Point(12, 12);
            this.buttonBuscarArchivo.Name = "buttonBuscarArchivo";
            this.buttonBuscarArchivo.Size = new System.Drawing.Size(117, 25);
            this.buttonBuscarArchivo.TabIndex = 0;
            this.buttonBuscarArchivo.Text = "Buscar archivo";
            this.buttonBuscarArchivo.UseVisualStyleBackColor = true;
            this.buttonBuscarArchivo.Click += new System.EventHandler(this.buttonBuscarArchivo_Click);
            // 
            // labelNombreArchivo
            // 
            this.labelNombreArchivo.AllowDrop = true;
            this.labelNombreArchivo.Location = new System.Drawing.Point(135, 15);
            this.labelNombreArchivo.Name = "labelNombreArchivo";
            this.labelNombreArchivo.Size = new System.Drawing.Size(201, 19);
            this.labelNombreArchivo.TabIndex = 1;
            this.labelNombreArchivo.Text = "No se ha seleccionado ningún archivo";
            this.labelNombreArchivo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // openFileDialogBuscarArchivo
            // 
            this.openFileDialogBuscarArchivo.DefaultExt = "txt";
            this.openFileDialogBuscarArchivo.ShowReadOnly = true;
            this.openFileDialogBuscarArchivo.Title = "Buscar archivo";
            // 
            // buttonBorrarDatos
            // 
            this.buttonBorrarDatos.Location = new System.Drawing.Point(175, 302);
            this.buttonBorrarDatos.Name = "buttonBorrarDatos";
            this.buttonBorrarDatos.Size = new System.Drawing.Size(75, 23);
            this.buttonBorrarDatos.TabIndex = 31;
            this.buttonBorrarDatos.Text = "Borrar datos";
            this.buttonBorrarDatos.UseVisualStyleBackColor = true;
            this.buttonBorrarDatos.Click += new System.EventHandler(this.buttonBorrarDatos_Click);
            // 
            // buttonEliminarRenglon
            // 
            this.buttonEliminarRenglon.Font = new System.Drawing.Font("Elephant", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEliminarRenglon.ForeColor = System.Drawing.Color.Red;
            this.buttonEliminarRenglon.Location = new System.Drawing.Point(127, 302);
            this.buttonEliminarRenglon.Name = "buttonEliminarRenglon";
            this.buttonEliminarRenglon.Size = new System.Drawing.Size(23, 23);
            this.buttonEliminarRenglon.TabIndex = 30;
            this.buttonEliminarRenglon.Text = "-";
            this.buttonEliminarRenglon.UseVisualStyleBackColor = true;
            this.buttonEliminarRenglon.Click += new System.EventHandler(this.buttonEliminarRenglon_Click);
            // 
            // buttonAñadirRenglon
            // 
            this.buttonAñadirRenglon.Font = new System.Drawing.Font("Elephant", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAñadirRenglon.ForeColor = System.Drawing.Color.Green;
            this.buttonAñadirRenglon.Location = new System.Drawing.Point(98, 302);
            this.buttonAñadirRenglon.Name = "buttonAñadirRenglon";
            this.buttonAñadirRenglon.Size = new System.Drawing.Size(23, 23);
            this.buttonAñadirRenglon.TabIndex = 29;
            this.buttonAñadirRenglon.Text = "+";
            this.buttonAñadirRenglon.UseVisualStyleBackColor = true;
            this.buttonAñadirRenglon.Click += new System.EventHandler(this.buttonAñadirRenglon_Click);
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
            this.comboBoxFormatoDeFecha.Location = new System.Drawing.Point(175, 272);
            this.comboBoxFormatoDeFecha.Name = "comboBoxFormatoDeFecha";
            this.comboBoxFormatoDeFecha.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFormatoDeFecha.TabIndex = 28;
            this.comboBoxFormatoDeFecha.SelectedIndexChanged += new System.EventHandler(this.comboBoxFormatoDeFecha_SelectedIndexChanged);
            // 
            // labelFormatoDeFecha
            // 
            this.labelFormatoDeFecha.AutoSize = true;
            this.labelFormatoDeFecha.Location = new System.Drawing.Point(50, 275);
            this.labelFormatoDeFecha.Name = "labelFormatoDeFecha";
            this.labelFormatoDeFecha.Size = new System.Drawing.Size(93, 13);
            this.labelFormatoDeFecha.TabIndex = 27;
            this.labelFormatoDeFecha.Text = "Formato de fecha:";
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
            this.comboBoxSeparadorDeMiles.Location = new System.Drawing.Point(175, 244);
            this.comboBoxSeparadorDeMiles.Name = "comboBoxSeparadorDeMiles";
            this.comboBoxSeparadorDeMiles.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSeparadorDeMiles.TabIndex = 26;
            this.comboBoxSeparadorDeMiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxSeparadorDeMiles_SelectedIndexChanged);
            // 
            // comboBoxSeparadorDecimal
            // 
            this.comboBoxSeparadorDecimal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSeparadorDecimal.FormattingEnabled = true;
            this.comboBoxSeparadorDecimal.Items.AddRange(new object[] {
            "\",\" - Coma",
            "\".\" - Punto",
            "Ninguno"});
            this.comboBoxSeparadorDecimal.Location = new System.Drawing.Point(175, 216);
            this.comboBoxSeparadorDecimal.Name = "comboBoxSeparadorDecimal";
            this.comboBoxSeparadorDecimal.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSeparadorDecimal.TabIndex = 25;
            this.comboBoxSeparadorDecimal.SelectedIndexChanged += new System.EventHandler(this.comboBoxSeparadorDecimal_SelectedIndexChanged);
            // 
            // labelSeparadorDeMiles
            // 
            this.labelSeparadorDeMiles.AutoSize = true;
            this.labelSeparadorDeMiles.Location = new System.Drawing.Point(50, 247);
            this.labelSeparadorDeMiles.Name = "labelSeparadorDeMiles";
            this.labelSeparadorDeMiles.Size = new System.Drawing.Size(100, 13);
            this.labelSeparadorDeMiles.TabIndex = 24;
            this.labelSeparadorDeMiles.Text = "Separador de miles:";
            // 
            // labelSeparadorDecimal
            // 
            this.labelSeparadorDecimal.AutoSize = true;
            this.labelSeparadorDecimal.Location = new System.Drawing.Point(50, 219);
            this.labelSeparadorDecimal.Name = "labelSeparadorDecimal";
            this.labelSeparadorDecimal.Size = new System.Drawing.Size(98, 13);
            this.labelSeparadorDecimal.TabIndex = 23;
            this.labelSeparadorDecimal.Text = "Separador decimal:";
            // 
            // dataGridViewCargadorDeDatos
            // 
            this.dataGridViewCargadorDeDatos.AllowUserToAddRows = false;
            this.dataGridViewCargadorDeDatos.AllowUserToResizeRows = false;
            this.dataGridViewCargadorDeDatos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewCargadorDeDatos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCargadorDeDatos.Location = new System.Drawing.Point(12, 334);
            this.dataGridViewCargadorDeDatos.Name = "dataGridViewCargadorDeDatos";
            this.dataGridViewCargadorDeDatos.Size = new System.Drawing.Size(324, 156);
            this.dataGridViewCargadorDeDatos.TabIndex = 22;
            // 
            // buttonCargar
            // 
            this.buttonCargar.Location = new System.Drawing.Point(61, 503);
            this.buttonCargar.Name = "buttonCargar";
            this.buttonCargar.Size = new System.Drawing.Size(89, 23);
            this.buttonCargar.TabIndex = 32;
            this.buttonCargar.Text = "Cargar datos";
            this.buttonCargar.UseVisualStyleBackColor = true;
            this.buttonCargar.Click += new System.EventHandler(this.buttonCargar_Click);
            // 
            // labelColumnaDeFechas
            // 
            this.labelColumnaDeFechas.AutoSize = true;
            this.labelColumnaDeFechas.Location = new System.Drawing.Point(50, 166);
            this.labelColumnaDeFechas.Name = "labelColumnaDeFechas";
            this.labelColumnaDeFechas.Size = new System.Drawing.Size(101, 13);
            this.labelColumnaDeFechas.TabIndex = 34;
            this.labelColumnaDeFechas.Text = "Columna de fechas:";
            // 
            // labelColumnaDeDatos
            // 
            this.labelColumnaDeDatos.AutoSize = true;
            this.labelColumnaDeDatos.Location = new System.Drawing.Point(50, 192);
            this.labelColumnaDeDatos.Name = "labelColumnaDeDatos";
            this.labelColumnaDeDatos.Size = new System.Drawing.Size(95, 13);
            this.labelColumnaDeDatos.TabIndex = 35;
            this.labelColumnaDeDatos.Text = "Columna de datos:";
            // 
            // comboBoxSeparadorColumnas
            // 
            this.comboBoxSeparadorColumnas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSeparadorColumnas.FormattingEnabled = true;
            this.comboBoxSeparadorColumnas.Items.AddRange(new object[] {
            "Tabulación",
            "Espacio",
            "\",\" - Coma",
            "\";\" - Punto y coma",
            "Otro"});
            this.comboBoxSeparadorColumnas.Location = new System.Drawing.Point(175, 47);
            this.comboBoxSeparadorColumnas.Name = "comboBoxSeparadorColumnas";
            this.comboBoxSeparadorColumnas.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSeparadorColumnas.TabIndex = 37;
            this.comboBoxSeparadorColumnas.SelectedIndexChanged += new System.EventHandler(this.comboBoxSeparadorColumnas_SelectedIndexChanged);
            // 
            // labelSeparadorColumnas
            // 
            this.labelSeparadorColumnas.AutoSize = true;
            this.labelSeparadorColumnas.Location = new System.Drawing.Point(50, 50);
            this.labelSeparadorColumnas.Name = "labelSeparadorColumnas";
            this.labelSeparadorColumnas.Size = new System.Drawing.Size(107, 13);
            this.labelSeparadorColumnas.TabIndex = 36;
            this.labelSeparadorColumnas.Text = "Separador columnas:";
            // 
            // textBoxOtroSeparadorColumnas
            // 
            this.textBoxOtroSeparadorColumnas.Location = new System.Drawing.Point(175, 74);
            this.textBoxOtroSeparadorColumnas.Name = "textBoxOtroSeparadorColumnas";
            this.textBoxOtroSeparadorColumnas.Size = new System.Drawing.Size(121, 20);
            this.textBoxOtroSeparadorColumnas.TabIndex = 38;
            this.textBoxOtroSeparadorColumnas.Visible = false;
            this.textBoxOtroSeparadorColumnas.TextChanged += new System.EventHandler(this.textBoxOtroSeparadorColumnas_TextChanged);
            // 
            // comboBoxColumnaFechas
            // 
            this.comboBoxColumnaFechas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxColumnaFechas.FormattingEnabled = true;
            this.comboBoxColumnaFechas.Location = new System.Drawing.Point(175, 162);
            this.comboBoxColumnaFechas.Name = "comboBoxColumnaFechas";
            this.comboBoxColumnaFechas.Size = new System.Drawing.Size(121, 21);
            this.comboBoxColumnaFechas.TabIndex = 39;
            this.comboBoxColumnaFechas.SelectedIndexChanged += new System.EventHandler(this.comboBoxColumnaFechas_SelectedIndexChanged);
            // 
            // comboBoxColumnaDatos
            // 
            this.comboBoxColumnaDatos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxColumnaDatos.FormattingEnabled = true;
            this.comboBoxColumnaDatos.Location = new System.Drawing.Point(175, 189);
            this.comboBoxColumnaDatos.Name = "comboBoxColumnaDatos";
            this.comboBoxColumnaDatos.Size = new System.Drawing.Size(121, 21);
            this.comboBoxColumnaDatos.TabIndex = 40;
            this.comboBoxColumnaDatos.SelectedIndexChanged += new System.EventHandler(this.comboBoxColumnaDatos_SelectedIndexChanged);
            // 
            // checkBoxTitulos
            // 
            this.checkBoxTitulos.AutoSize = true;
            this.checkBoxTitulos.Location = new System.Drawing.Point(86, 100);
            this.checkBoxTitulos.Name = "checkBoxTitulos";
            this.checkBoxTitulos.Size = new System.Drawing.Size(178, 17);
            this.checkBoxTitulos.TabIndex = 41;
            this.checkBoxTitulos.Text = "Títulos contenidos en el archivo";
            this.checkBoxTitulos.UseVisualStyleBackColor = true;
            this.checkBoxTitulos.CheckedChanged += new System.EventHandler(this.checkBoxTitulos_CheckedChanged);
            // 
            // labelOtroSeparadorColumnas
            // 
            this.labelOtroSeparadorColumnas.AutoSize = true;
            this.labelOtroSeparadorColumnas.Location = new System.Drawing.Point(50, 77);
            this.labelOtroSeparadorColumnas.Name = "labelOtroSeparadorColumnas";
            this.labelOtroSeparadorColumnas.Size = new System.Drawing.Size(43, 13);
            this.labelOtroSeparadorColumnas.TabIndex = 42;
            this.labelOtroSeparadorColumnas.Text = "¿Cuál?:";
            this.labelOtroSeparadorColumnas.Visible = false;
            // 
            // buttonVisualizar
            // 
            this.buttonVisualizar.Enabled = false;
            this.buttonVisualizar.Location = new System.Drawing.Point(137, 125);
            this.buttonVisualizar.Name = "buttonVisualizar";
            this.buttonVisualizar.Size = new System.Drawing.Size(75, 23);
            this.buttonVisualizar.TabIndex = 43;
            this.buttonVisualizar.Text = "Visualizar";
            this.buttonVisualizar.UseVisualStyleBackColor = true;
            this.buttonVisualizar.Click += new System.EventHandler(this.buttonVisualizar_Click);
            // 
            // buttonAceptar
            // 
            this.buttonAceptar.Enabled = false;
            this.buttonAceptar.Location = new System.Drawing.Point(199, 503);
            this.buttonAceptar.Name = "buttonAceptar";
            this.buttonAceptar.Size = new System.Drawing.Size(89, 23);
            this.buttonAceptar.TabIndex = 44;
            this.buttonAceptar.Text = "Aceptar";
            this.buttonAceptar.UseVisualStyleBackColor = true;
            this.buttonAceptar.Click += new System.EventHandler(this.buttonAceptar_Click);
            // 
            // CargadorDeArchivos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 538);
            this.Controls.Add(this.buttonAceptar);
            this.Controls.Add(this.buttonVisualizar);
            this.Controls.Add(this.labelOtroSeparadorColumnas);
            this.Controls.Add(this.checkBoxTitulos);
            this.Controls.Add(this.comboBoxColumnaDatos);
            this.Controls.Add(this.comboBoxColumnaFechas);
            this.Controls.Add(this.textBoxOtroSeparadorColumnas);
            this.Controls.Add(this.comboBoxSeparadorColumnas);
            this.Controls.Add(this.labelSeparadorColumnas);
            this.Controls.Add(this.labelColumnaDeDatos);
            this.Controls.Add(this.labelColumnaDeFechas);
            this.Controls.Add(this.buttonCargar);
            this.Controls.Add(this.buttonBorrarDatos);
            this.Controls.Add(this.buttonEliminarRenglon);
            this.Controls.Add(this.buttonAñadirRenglon);
            this.Controls.Add(this.comboBoxFormatoDeFecha);
            this.Controls.Add(this.labelFormatoDeFecha);
            this.Controls.Add(this.comboBoxSeparadorDeMiles);
            this.Controls.Add(this.comboBoxSeparadorDecimal);
            this.Controls.Add(this.labelSeparadorDeMiles);
            this.Controls.Add(this.labelSeparadorDecimal);
            this.Controls.Add(this.dataGridViewCargadorDeDatos);
            this.Controls.Add(this.labelNombreArchivo);
            this.Controls.Add(this.buttonBuscarArchivo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CargadorDeArchivos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cargar Archivo";
            this.Load += new System.EventHandler(this.CargadorDeArchivos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCargadorDeDatos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBuscarArchivo;
        private System.Windows.Forms.Label labelNombreArchivo;
        private System.Windows.Forms.OpenFileDialog openFileDialogBuscarArchivo;
        private System.Windows.Forms.Button buttonBorrarDatos;
        private System.Windows.Forms.Button buttonEliminarRenglon;
        private System.Windows.Forms.Button buttonAñadirRenglon;
        private System.Windows.Forms.ComboBox comboBoxFormatoDeFecha;
        private System.Windows.Forms.Label labelFormatoDeFecha;
        private System.Windows.Forms.ComboBox comboBoxSeparadorDeMiles;
        private System.Windows.Forms.ComboBox comboBoxSeparadorDecimal;
        private System.Windows.Forms.Label labelSeparadorDeMiles;
        private System.Windows.Forms.Label labelSeparadorDecimal;
        private System.Windows.Forms.DataGridView dataGridViewCargadorDeDatos;
        private System.Windows.Forms.Button buttonCargar;
        private System.Windows.Forms.Label labelColumnaDeFechas;
        private System.Windows.Forms.Label labelColumnaDeDatos;
        private System.Windows.Forms.ComboBox comboBoxSeparadorColumnas;
        private System.Windows.Forms.Label labelSeparadorColumnas;
        private System.Windows.Forms.TextBox textBoxOtroSeparadorColumnas;
        private System.Windows.Forms.ComboBox comboBoxColumnaFechas;
        private System.Windows.Forms.ComboBox comboBoxColumnaDatos;
        private System.Windows.Forms.CheckBox checkBoxTitulos;
        private System.Windows.Forms.Label labelOtroSeparadorColumnas;
        private System.Windows.Forms.Button buttonVisualizar;
        private System.Windows.Forms.Button buttonAceptar;
    }
}