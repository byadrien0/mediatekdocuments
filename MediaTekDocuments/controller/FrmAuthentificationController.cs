using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaTekDocuments.dal;
using MediaTekDocuments.model;
using MediaTekDocuments.view;


namespace MediaTekDocuments.controller
{
    class FrmAuthentificationController
    {
        private readonly Access access;
        private Utilisateur utilisateur = null;
        private FrmMediatek mediatek;

        /// <summary>
        /// Constructeur de la classe.
        /// </summary>
        public FrmAuthentificationController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// Initialisation d'une nouvelle fenêtre.
        /// </summary>
        private void init()
        {
            mediatek = new FrmMediatek(utilisateur);
            mediatek.Show();
        }

        /// <summary>
        /// Récupération du login.
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool GetLogin(string mail, string password)
        {

            utilisateur = access.GetLogin(mail, password);
            if (utilisateur != null)
            {
                init();
                return true;
            }

            return false;
        }

    }

    
}
