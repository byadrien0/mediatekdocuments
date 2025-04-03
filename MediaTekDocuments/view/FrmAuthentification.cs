using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaTekDocuments.controller;
using MediaTekDocuments.model;


namespace MediaTekDocuments.view
{
    public partial class FrmAuthentification : Form
    {
        private FrmAuthentificationController controller;

        /// <summary>
        /// Constructeur de la fenêtre d'authentification.
        /// </summary>
        public FrmAuthentification()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// Initialisation de la fenêtre d'authentification.
        /// </summary>
        private void Init()
        {
            txtNomUtilisateur.Text = "";
            txtMotDePasse.Text = "";
            controller = new FrmAuthentificationController();
        }

        /// <summary>
        /// Évènement : Clique sur le bouton d'authentification 'btnValiderAuthentification' pour accéder à l'application.
        /// Actions : Vérification de l'authentification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderAuthentification_Click(object sender, EventArgs e)
        {
           if(controller.GetLogin(txtNomUtilisateur.Text, txtMotDePasse.Text))
            {
                this.Visible = false;

            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect", "Erreur d'authentification");
                txtNomUtilisateur.Text = "";
                txtMotDePasse.Text = "";
            }
        }
    }
}
