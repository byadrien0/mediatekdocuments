using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using System;
using Newtonsoft.Json;


namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }


        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Récupération de toutes les commandes d'un livre.
        /// </summary>
        /// <param name="idLivre"></param>
        /// <returns></returns>
        public List<CommandeDocument> GetCommandesLivre(string idLivre)
        {
            return access.GetCommandesLivres(idLivre);
        }

        /// <summary>
        /// Récupération de tous les états possibles d'une commande.
        /// </summary>
        /// <returns></returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// Ajout d'une commande passée en paramètre.
        /// </summary>
        /// <param name="commande"></param>
        public void AjouterCommande(CommandeDocument commande)
        {
            access.AjouterEnregistrement("commandedocument", JsonConvert.SerializeObject(commande));
        }

        /// <summary>
        /// Modification d'une commande passée en paramètre.
        /// </summary>
        /// <param name="commande"></param>
        public void ModifierCommande(CommandeDocument commande)
        {
            access.ModifierEnregistrement("commandedocument", commande.Id, JsonConvert.SerializeObject(commande));
        }

        /// <summary>
        /// Suppression d'une commande passée en paramètre.
        /// </summary>
        /// <param name="commande"></param>
        public void SupprimerCommande(CommandeDocument commande)
        {
            access.SupprimerEnregistrement("commandedocument", JsonConvert.SerializeObject(commande));
        }

        /// <summary>
        /// Récuperation de l'id maximal des commandes.
        /// </summary>
        public string GetMaxIdCommande()
        {
            string maxId = access.getMaxId("maxcommande");
            return maxId;
        }

        /// <summary>
        /// Récupérations de tous les abonnements.
        /// </summary>
        /// <param name="idRevue"></param>
        /// <returns></returns>
        public List<Abonnement> GetAbonnements(string idRevue)
        {
            return access.GetAbonnements(idRevue);
        }

        /// <summary>
        /// Ajouter un abonnement.
        /// </summary>
        /// <param name="abonnement"></param>
        public void AjouterAbonnement(Abonnement abonnement)
        {
            access.AjouterEnregistrement("abonnement", JsonConvert.SerializeObject(abonnement));
        }

        /// <summary>
        /// Modifier un abonement.
        /// </summary>
        /// <param name="abonnement"></param>
        public void ModifierAbonnement(Abonnement abonnement)
        {
            access.ModifierEnregistrement("abonnement", abonnement.Id, JsonConvert.SerializeObject(abonnement));
        }

        /// <summary>
        /// Supprimer un abonnement.
        /// </summary>
        /// <param name="abonnement"></param>
        public void SupprimerAbonnement(Abonnement abonnement)
        {
            access.SupprimerEnregistrement("abonnement", JsonConvert.SerializeObject(abonnement));
        }

        /// <summary>
        ///  Retourne vrai si le service de l'utilisateur est autorisé, non si il est interdit.
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        public bool verifDroitAccueil(Utilisateur utilisateur)
        {
            Console.WriteLine(utilisateur.Nom);
            List<string> services = new List<string> { "Comptabilité", "Bibliothèque", "Accueil" };
            if (services.Contains(utilisateur.Service))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

}
