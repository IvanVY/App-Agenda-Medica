namespace AgendaMedicaApp.Forms
{
    partial class HomeForm
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
            this.label1lblWelcome = new System.Windows.Forms.Label();
            this.btnScheduleAppointment = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.dataGridViewAppointments = new System.Windows.Forms.DataGridView();
            this.btnDeleteAppointment = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAppointments)).BeginInit();
            this.SuspendLayout();
            // 
            // label1lblWelcome
            // 
            this.label1lblWelcome.AutoSize = true;
            this.label1lblWelcome.Location = new System.Drawing.Point(315, 161);
            this.label1lblWelcome.Name = "label1lblWelcome";
            this.label1lblWelcome.Size = new System.Drawing.Size(75, 16);
            this.label1lblWelcome.TabIndex = 0;
            this.label1lblWelcome.Text = "Bienvenido";
            // 
            // btnScheduleAppointment
            // 
            this.btnScheduleAppointment.Location = new System.Drawing.Point(172, 197);
            this.btnScheduleAppointment.Name = "btnScheduleAppointment";
            this.btnScheduleAppointment.Size = new System.Drawing.Size(117, 33);
            this.btnScheduleAppointment.TabIndex = 1;
            this.btnScheduleAppointment.Text = "Agendar Cita";
            this.btnScheduleAppointment.UseVisualStyleBackColor = true;
            this.btnScheduleAppointment.Click += new System.EventHandler(this.btnScheduleAppointment_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(280, 276);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(129, 33);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "Cerrar Sesion";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // dataGridViewAppointments
            // 
            this.dataGridViewAppointments.AllowUserToAddRows = false;
            this.dataGridViewAppointments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAppointments.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewAppointments.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAppointments.Name = "dataGridViewAppointments";
            this.dataGridViewAppointments.RowHeadersWidth = 51;
            this.dataGridViewAppointments.RowTemplate.Height = 24;
            this.dataGridViewAppointments.ShowEditingIcon = false;
            this.dataGridViewAppointments.Size = new System.Drawing.Size(689, 158);
            this.dataGridViewAppointments.TabIndex = 5;
            // 
            // btnDeleteAppointment
            // 
            this.btnDeleteAppointment.Location = new System.Drawing.Point(400, 197);
            this.btnDeleteAppointment.Name = "btnDeleteAppointment";
            this.btnDeleteAppointment.Size = new System.Drawing.Size(129, 33);
            this.btnDeleteAppointment.TabIndex = 6;
            this.btnDeleteAppointment.Text = "Eliminar Cita";
            this.btnDeleteAppointment.UseVisualStyleBackColor = true;
            this.btnDeleteAppointment.Click += new System.EventHandler(this.btnDeleteAppointment_Click);
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(689, 399);
            this.Controls.Add(this.btnDeleteAppointment);
            this.Controls.Add(this.dataGridViewAppointments);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnScheduleAppointment);
            this.Controls.Add(this.label1lblWelcome);
            this.Name = "HomeForm";
            this.Text = "HomeForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAppointments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1lblWelcome;
        private System.Windows.Forms.Button btnScheduleAppointment;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.DataGridView dataGridViewAppointments;
        private System.Windows.Forms.Button btnDeleteAppointment;
    }
}