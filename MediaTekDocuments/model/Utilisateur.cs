using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier 'Utilisateur'.
    /// </summary>
    public class Utilisateur
    {
        /// <summary>
        /// Propriétés de la classe. 
        /// </summary>
        public string Id { get; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Mail { get; set; }
        public string idService { get; set; }
        public string Service { get; set; }

        /// <summary>
        /// Constructeur de la classe.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nom"></param>
        /// <param name="prenom"></param>
        /// <param name="mail"></param>
        /// <param name="idService"></param>
        /// <param name="service"></param>
        public Utilisateur(string id, string nom, string prenom, string mail, string idService, string service)
        {
            this.Id = id;
            this.Nom = nom;
            this.Prenom = prenom;
            this.Mail = mail;
            this.idService = idService;
            this.Service = service;
        }
    }
}
