using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier Abonnement héritant de Commande.
    /// </summary>
    public class Abonnement : Commande
    {
        /// <summary>
        /// Propriétés de la classe.
        /// </summary>
        public DateTime DateFinAbonnement { get; }

        public string IdRevue { get; }

        /// <summary>
        /// Constructeur de la classe.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="idRevue"></param>
        public Abonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue)
            : base(id, dateCommande, montant)
        {
            DateFinAbonnement = dateFinAbonnement;
            IdRevue = idRevue;
        }
    }
}
