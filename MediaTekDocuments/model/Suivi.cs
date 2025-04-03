using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier 'Suivi'.
    /// </summary>
    public class Suivi
    {
        /// <summary>
        /// Propriétés de la classe.
        /// </summary>
        public int Id { get; }
        public string Etat { get; }

        /// <summary>
        /// Constructeur de la classe.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="etat"></param>
        public Suivi(int id, string etat)
        {
            this.Id = id;
            this.Etat = etat;
        }

        /// <summary>
        /// Récupération du libellé des etats de la commmandes.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Etat;
        }
    }
}
