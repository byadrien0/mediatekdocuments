
namespace MediaTekDocuments.view
{
    partial class FrmAuthentification
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
            this.txtNomUtilisateur = new System.Windows.Forms.TextBox();
            this.txtMotDePasse = new System.Windows.Forms.TextBox();
            this.lblNomUtilisateur = new System.Windows.Forms.Label();
            this.lblMotDePasse = new System.Windows.Forms.Label();
            this.btnValiderAuthentification = new System.Windows.Forms.Button();
            this.lblTitreAuthentification = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtNomUtilisateur
            // 
            this.txtNomUtilisateur.Location = new System.Drawing.Point(207, 129);
            this.txtNomUtilisateur.Name = "txtNomUtilisateur";
            this.txtNomUtilisateur.Size = new System.Drawing.Size(307, 20);
            this.txtNomUtilisateur.TabIndex = 0;
            // 
            // txtMotDePasse
            // 
            this.txtMotDePasse.Location = new System.Drawing.Point(207, 178);
            this.txtMotDePasse.Name = "txtMotDePasse";
            this.txtMotDePasse.Size = new System.Drawing.Size(307, 20);
            this.txtMotDePasse.TabIndex = 1;
            // 
            // lblNomUtilisateur
            // 
            this.lblNomUtilisateur.AutoSize = true;
            this.lblNomUtilisateur.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblNomUtilisateur.Location = new System.Drawing.Point(204, 113);
            this.lblNomUtilisateur.Name = "lblNomUtilisateur";
            this.lblNomUtilisateur.Size = new System.Drawing.Size(84, 13);
            this.lblNomUtilisateur.TabIndex = 2;
            this.lblNomUtilisateur.Text = "Nom d\'utilisateur";
            // 
            // lblMotDePasse
            // 
            this.lblMotDePasse.AutoSize = true;
            this.lblMotDePasse.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblMotDePasse.Location = new System.Drawing.Point(204, 162);
            this.lblMotDePasse.Name = "lblMotDePasse";
            this.lblMotDePasse.Size = new System.Drawing.Size(71, 13);
            this.lblMotDePasse.TabIndex = 3;
            this.lblMotDePasse.Text = "Mot de passe";
            // 
            // btnValiderAuthentification
            // 
            this.btnValiderAuthentification.Location = new System.Drawing.Point(301, 216);
            this.btnValiderAuthentification.Name = "btnValiderAuthentification";
            this.btnValiderAuthentification.Size = new System.Drawing.Size(112, 23);
            this.btnValiderAuthentification.TabIndex = 4;
            this.btnValiderAuthentification.Text = "Se connecter";
            this.btnValiderAuthentification.UseVisualStyleBackColor = true;
            this.btnValiderAuthentification.Click += new System.EventHandler(this.btnValiderAuthentification_Click);
            // 
            // lblTitreAuthentification
            // 
            this.lblTitreAuthentification.AutoSize = true;
            this.lblTitreAuthentification.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitreAuthentification.ForeColor = System.Drawing.Color.White;
            this.lblTitreAuthentification.Location = new System.Drawing.Point(214, 40);
            this.lblTitreAuthentification.Name = "lblTitreAuthentification";
            this.lblTitreAuthentification.Size = new System.Drawing.Size(294, 46);
            this.lblTitreAuthentification.TabIndex = 5;
            this.lblTitreAuthentification.Text = "Authentification";
            // 
            // FrmAuthentification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(735, 285);
            this.Controls.Add(this.lblTitreAuthentification);
            this.Controls.Add(this.btnValiderAuthentification);
            this.Controls.Add(this.lblMotDePasse);
            this.Controls.Add(this.lblNomUtilisateur);
            this.Controls.Add(this.txtMotDePasse);
            this.Controls.Add(this.txtNomUtilisateur);
            this.Name = "FrmAuthentification";
            this.Text = "Authentification";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtNomUtilisateur;
        private System.Windows.Forms.TextBox txtMotDePasse;
        private System.Windows.Forms.Label lblNomUtilisateur;
        private System.Windows.Forms.Label lblMotDePasse;
        private System.Windows.Forms.Button btnValiderAuthentification;
        private System.Windows.Forms.Label lblTitreAuthentification;
    }
}