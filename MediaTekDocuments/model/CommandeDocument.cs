using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier 'commandeDocument' héritant de la classe 'commande'.
    /// </summary>
    public class CommandeDocument : Commande
    {
        /// <summary>
        /// Propriétés de la classe.
        /// </summary>
        public int NbExemplaire { get; }
        public string IdLivreDvd { get; }
        public int IdSuivi { get; }
        public string Etat { get; }

        /// <summary>
        /// Constructeur de la classe.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="nbExemplaire"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="idSuivi"></param>
        /// <param name="etat"></param>
        public CommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaire, string idLivreDvd, int idSuivi, string etat) : base(id, dateCommande, montant)
        {
            this.NbExemplaire = nbExemplaire;
            this.IdLivreDvd = idLivreDvd;
            this.IdSuivi = idSuivi;
            this.Etat = etat;
        }
    }
}
