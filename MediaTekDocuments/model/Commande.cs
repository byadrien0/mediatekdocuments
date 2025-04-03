using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier commande.
    /// </summary>
    public class Commande
    {
        /// <summary>
        /// Propriétés de la classe.
        /// </summary>
        public string Id { get; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; }

        /// <summary>
        /// Constructeur de la classe.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        public Commande(string id, DateTime dateCommande, double montant)
        {
            this.Id = id;
            this.DateCommande = dateCommande;
            this.Montant = montant;
        }
    }
}
