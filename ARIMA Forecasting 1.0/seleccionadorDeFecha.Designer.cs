namespace DeclineR.Pantalla_Principal
{
    partial class seleccionadorDeFecha
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(seleccionadorDeFecha));
            this.monthCalendar = new System.Windows.Forms.MonthCalendar();
            this.buttonSeleccionarFecha = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // monthCalendar
            // 
            this.monthCalendar.Location = new System.Drawing.Point(0, 0);
            this.monthCalendar.MaxSelectionCount = 1;
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.TabIndex = 0;
            // 
            // buttonSeleccionarFecha
            // 
            this.buttonSeleccionarFecha.Location = new System.Drawing.Point(74, 174);
            this.buttonSeleccionarFecha.Name = "buttonSeleccionarFecha";
            this.buttonSeleccionarFecha.Size = new System.Drawing.Size(75, 23);
            this.buttonSeleccionarFecha.TabIndex = 1;
            this.buttonSeleccionarFecha.Text = "Seleccionar Fecha";
            this.buttonSeleccionarFecha.UseVisualStyleBackColor = true;
            this.buttonSeleccionarFecha.Click += new System.EventHandler(this.buttonSeleccionarFecha_Click);
            // 
            // seleccionadorDeFecha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 211);
            this.Controls.Add(this.buttonSeleccionarFecha);
            this.Controls.Add(this.monthCalendar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "seleccionadorDeFecha";
            this.Load += new System.EventHandler(this.seleccionadorDeFecha_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendar;
        private System.Windows.Forms.Button buttonSeleccionarFecha;
    }
}