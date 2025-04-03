using MediaTekDocuments.controller;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun

        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        private Utilisateur utilisateur;



        /// <summary>
        /// Constructeur : création du contrôleur.
        /// </summary>
        internal FrmMediatek(Utilisateur utilisateur)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.utilisateur = utilisateur;
            verifDroitAccueil(utilisateur);

        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Vérification si la valeur passée en paramètre peut être transtypée en nombre entier.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool estNombreEntier(string input)
        {
            return int.TryParse(input, out _);
        }

        /// <summary>
        /// Vérification si la valeur peut être transtypée en nombre à virgule.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool estNombreVirgule(string input)
        {
            return float.TryParse(input, out _);
        }

        /// <summary>
        /// Vérification des droits.
        /// </summary>
        /// <param name="lutilisateur"></param>
        private void verifDroitAccueil(Utilisateur lutilisateur)
        {
            if (!controller.verifDroitAccueil(lutilisateur))
            {
                MessageBox.Show("Droits insuffisant");
                Application.Exit();
            }
        }


        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();

            AfficherAbonnementFiniMoinTrenteJours();

        }

        bool fait = false;
        /// <summary>
        /// Affiche revue dont au moin un abonnement fini dans moin de 30 jours.
        /// </summary>
        private void AfficherAbonnementFiniMoinTrenteJours()
        {

            if(fait == false)
            {
                List<Revue> revues = controller.GetAllRevues();
                string alerteRevues = "Revues dont l'abonnement se termine dans moins de 30 jours : \n";

                foreach (Revue revue in revues)
                {
                    List<Abonnement> abonnements = controller.GetAbonnements(revue.Id);
                    if (abonnements.FindAll(o => (o.DateFinAbonnement <= DateTime.Now.AddMonths(1)) && (o.DateFinAbonnement >= DateTime.Now)).Count > 0)
                    {
                        alerteRevues = string.Concat(alerteRevues, "  -" + revue.Titre + "\n");
                        MessageBox.Show(alerteRevues);
                    }
                }
                fait = true;
            }
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }
        #endregion

        #region Onglet Commandes de livres 



        /* ---------- ---------- ---------- Déclarations et Initialisations ---------- ---------- ---------- */



        List<Livre> lesLivresCommandes = new List<Livre>();
        string actionCommandeLivre;

        private readonly BindingSource bdgListeLivres_Commandes = new BindingSource();
        private readonly BindingSource bdgCommandesLivre_Commandes = new BindingSource();
        private readonly BindingSource bdgSuivis = new BindingSource();

        

        /* ---------- ---------- ---------- Méthodes ---------- ---------- ---------- */



        /// <summary>
        /// Remplit le DataGridView 'dgvListeLivres' avec la liste de livres fournie en paramètre.
        /// </summary>
        /// <param name="livres"></param>
        private void RemplirListeLivres_Commandes(List<Livre> livres)
        {
            bdgListeLivres_Commandes.DataSource = livres;
            dgvListeLivres.DataSource = bdgListeLivres_Commandes;


            dgvListeLivres.Columns["isbn"].Visible = false;
            dgvListeLivres.Columns["image"].Visible = false;
            dgvListeLivres.Columns["idGenre"].Visible = false;
            dgvListeLivres.Columns["idPublic"].Visible = false;
            dgvListeLivres.Columns["idRayon"].Visible = false;

            dgvListeLivres.Columns["id"].DisplayIndex = 0;
            dgvListeLivres.Columns["Titre"].DisplayIndex = 1;
            dgvListeLivres.Columns["Auteur"].DisplayIndex = 2;

            dgvListeLivres.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvListeLivres.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        /// <summary>
        /// Remplit le DataGridView 'dgvListeLivres' avec tous les livres.
        /// </summary>
        private void RemplirListeCompleteLivres_Commandes()
        {
            RemplirListeLivres_Commandes(lesLivresCommandes);
        }

        /// <summary>
        /// Remplit le DataGridView 'dgvListeCommandes' avec la liste de commandes passée en paramètre.
        /// </summary>
        /// <param name="commandesLivre"></param>
        private void RemplirListeCommandesLivre_Commandes(List<CommandeDocument> commandesLivre)
        {
            bdgCommandesLivre_Commandes.DataSource = commandesLivre;
            dgvListeCommandes.DataSource = bdgCommandesLivre_Commandes;

            dgvListeCommandes.Columns["idLivreDvd"].Visible = false;
            dgvListeCommandes.Columns["idSuivi"].Visible = false;

            dgvListeCommandes.Columns["id"].DisplayIndex = 0;
            dgvListeCommandes.Columns["Montant"].DisplayIndex = 2;

            dgvListeCommandes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        /// <summary>
        /// Récupère les commandes d'un livre passé en paramètre et les affiche.
        /// </summary>
        /// <param name="commandesLivre"></param>
        private void AfficherCommandesLivre_Commandes(Livre livre)
        {
            txtNumeroLivre.Text = livre.Id;

            ViderZoneDetailsCommandeLivre_Commande();
            DesactiverZoneDetailsCommandeLivre_Commande();

            List<CommandeDocument> commandesLivre = controller.GetCommandesLivre(livre.Id);

            if (commandesLivre.Count == 0)
            {
                ViderZoneDetailsCommandeLivre_Commande();
            }

            RemplirListeCommandesLivre_Commandes(commandesLivre);
        }

        /// <summary>
        /// Afficher les détails d'une commande passée en paramètre.
        /// </summary>
        /// <param name="commande"></param>
        private void AfficherDetailsCommandeLivre_Commande(CommandeDocument commande)
        {
            txtNumeroCommande.Text = commande.Id;
            txtNombreExemplaires.Text = commande.NbExemplaire.ToString();
            txtMontantCommande.Text = commande.Montant.ToString();
            cmbEtatCommande.SelectedIndex = cmbEtatCommande.FindString(commande.Etat);
            dtpDateCommande.Value = commande.DateCommande;
        }

        /// <summary>
        /// Valider l'ajout, la modification, et la suppression. //tag*
        /// </summary>
        public void ValiderActionCommandeLivre()
        {

            string id = "";
            DateTime dateCommande;
            float montant = -1;
            int nbExemplaire = -1;
            string idLivreDvd;
            int idSuivi = -1;
            string etat = "";


            if (actionCommandeLivre == "ajouter")
            {
                string maxId = controller.GetMaxIdCommande();
                if (maxId == null)
                {
                    id = "1";
                    txtNumeroCommande.Text = id;
                }
                else
                {
                    int maxIdInt = int.Parse(maxId);
                    maxIdInt++;
                    id = maxIdInt.ToString();
                }

                if (txtNombreExemplaires.Text == "" || txtMontantCommande.Text == "" || cmbEtatCommande.SelectedIndex < 0)
                {
                    MessageBox.Show("Veuillez remplir tous les champs.", "Erreur d'ajout");

                    Livre livre = (Livre)bdgListeLivres_Commandes.List[bdgListeLivres_Commandes.Position];
                    AfficherCommandesLivre_Commandes(livre);

                    ViderZoneDetailsCommandeLivre_Commande();
                    dgvListeCommandes.ClearSelection();
                }
                else
                {
                    if (!estNombreEntier(txtNombreExemplaires.Text) || !estNombreVirgule(txtMontantCommande.Text))
                    {
                        MessageBox.Show("Veuillez respecter le typage des données saisies.", "Erreur d'ajout");

                        Livre livre = (Livre)bdgListeLivres_Commandes.List[bdgListeLivres_Commandes.Position];
                        AfficherCommandesLivre_Commandes(livre);

                        ViderZoneDetailsCommandeLivre_Commande();
                        dgvListeCommandes.ClearSelection();

                    }
                    else
                    {
                        if (cmbEtatCommande.SelectedIndex == 2 || cmbEtatCommande.SelectedIndex == 3 || cmbEtatCommande.SelectedIndex == 4)
                        {
                            MessageBox.Show("Une nouvelle commande ne peut pas être marquée comme 'Relancée' 'Livrée' ou 'Réglée'.", "Erreur d'ajout");

                            Livre livre = (Livre)bdgListeLivres_Commandes.List[bdgListeLivres_Commandes.Position];
                            AfficherCommandesLivre_Commandes(livre);

                            ViderZoneDetailsCommandeLivre_Commande();
                            dgvListeCommandes.ClearSelection();
                        }
                        else
                        {
                            dateCommande = dtpDateCommande.Value;
                            montant = float.Parse(txtMontantCommande.Text);
                            nbExemplaire = int.Parse(txtNombreExemplaires.Text);
                            idLivreDvd = txtNumeroLivre.Text;
                            Suivi suivi = (Suivi)cmbEtatCommande.SelectedItem;
                            idSuivi = suivi.Id;
                            etat = suivi.Etat;

                            CommandeDocument commande = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                            controller.AjouterCommande(commande);
                            grpActionsCommande.Text = "Ajout réussie";

                            Livre livre = (Livre)bdgListeLivres_Commandes.List[bdgListeLivres_Commandes.Position];
                            AfficherCommandesLivre_Commandes(livre);

                            ViderZoneDetailsCommandeLivre_Commande();
                            dgvListeCommandes.ClearSelection();
                        }

                    }
                }
            }
            else
            {
                if (actionCommandeLivre == "modifier")
                {
                    if (dgvListeCommandes.SelectedRows.Count > 0)
                    {

                        if (txtNombreExemplaires.Text == "" || txtMontantCommande.Text == "" || cmbEtatCommande.SelectedIndex < 0)
                        {
                            MessageBox.Show("Veuillez remplir tous les champs.", "Erreur d'ajout");

                            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivre_Commandes[bdgCommandesLivre_Commandes.Position];
                            AfficherDetailsCommandeLivre_Commande(commandeDocument);

                            ViderZoneDetailsCommandeLivre_Commande();
                            dgvListeCommandes.ClearSelection();
                        }
                        else
                        {
                            if (!estNombreEntier(txtNombreExemplaires.Text) || !estNombreVirgule(txtMontantCommande.Text))
                            {
                                MessageBox.Show("Veuillez respecter le typage des données saisies.", "Erreur d'ajout");

                                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivre_Commandes[bdgCommandesLivre_Commandes.Position];
                                AfficherDetailsCommandeLivre_Commande(commandeDocument);

                                ViderZoneDetailsCommandeLivre_Commande();
                                dgvListeCommandes.ClearSelection();
                            }
                            else
                            {
                                id = txtNumeroCommande.Text;
                                dateCommande = dtpDateCommande.Value;
                                montant = float.Parse(txtMontantCommande.Text);
                                nbExemplaire = int.Parse(txtNombreExemplaires.Text);
                                idLivreDvd = txtNumeroLivre.Text;
                                Suivi suivi = (Suivi)cmbEtatCommande.SelectedItem;
                                idSuivi = suivi.Id;
                                etat = suivi.Etat;

                                CommandeDocument commande = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                                controller.ModifierCommande(commande);
                                grpActionsCommande.Text = "Modification réussie";

                                Livre livre = (Livre)bdgListeLivres_Commandes.List[bdgListeLivres_Commandes.Position];
                                AfficherCommandesLivre_Commandes(livre);

                                ViderZoneDetailsCommandeLivre_Commande();
                                dgvListeCommandes.ClearSelection();

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Veuillez séléctionner une commande.", "Erreur de modification");
                    }
                }
                else
                {
                    if (actionCommandeLivre == "supprimer")
                    {
                        if (dgvListeCommandes.SelectedRows.Count > 0)
                        {
                            id = txtNumeroCommande.Text;
                            dateCommande = dtpDateCommande.Value;
                            montant = float.Parse(txtMontantCommande.Text);
                            nbExemplaire = int.Parse(txtNombreExemplaires.Text);
                            idLivreDvd = txtNumeroLivre.Text;
                            Suivi suivi = (Suivi)cmbEtatCommande.SelectedItem;
                            idSuivi = suivi.Id;
                            MessageBox.Show(idSuivi.ToString());
                            etat = suivi.Etat;

                            if (idSuivi == 4)
                            {
                                MessageBox.Show("Impossible de supprimer une commande déjà livrée. ", "Erreur de suppression");

                                Livre livre = (Livre)bdgListeLivres_Commandes.List[bdgListeLivres_Commandes.Position];
                                AfficherCommandesLivre_Commandes(livre);

                                ViderZoneDetailsCommandeLivre_Commande();
                                dgvListeCommandes.ClearSelection();
                            }
                            else
                            {

                                CommandeDocument commande = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                                controller.SupprimerCommande(commande);
                                grpActionsCommande.Text = "Suppression réussie";

                                Livre livre = (Livre)bdgListeLivres_Commandes.List[bdgListeLivres_Commandes.Position];
                                AfficherCommandesLivre_Commandes(livre);

                                ViderZoneDetailsCommandeLivre_Commande();
                                dgvListeCommandes.ClearSelection();
                            }

                        }
                        else
                        {
                            MessageBox.Show("Veuillez sélectionner une commande.", "Erreur de suppression");
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Vider la zone des détails d'une commande.
        /// </summary>
        public void ViderZoneDetailsCommandeLivre_Commande()
        {
            txtNumeroCommande.Text = "";
            txtNombreExemplaires.Text = "";
            txtMontantCommande.Text = "";
            cmbEtatCommande.SelectedIndex = -1;
        }

        /// <summary>
        /// Désactiver la zone de détails d'une commande.
        /// </summary>
        public void DesactiverZoneDetailsCommandeLivre_Commande()
        {
            txtNombreExemplaires.Enabled = false;
            txtMontantCommande.Enabled = false;
            cmbEtatCommande.Enabled = false;
            dtpDateCommande.Enabled = false;
        }

        /// <summary>
        /// Activer la zone de détails d'une commande.
        /// </summary>
        public void ActiverZoneDetailsCommandeLivre_Commande()
        {
            txtNombreExemplaires.Enabled = true;
            txtMontantCommande.Enabled = true;
            cmbEtatCommande.Enabled = true;
            dtpDateCommande.Enabled = true;
        }

        /// <summary>
        /// Desactiver les boutons d'actions sur les commandes de livre.
        /// </summary>
        public void DesactiverBoutonsActionCommandeLivre_Commande()
        {
            btnAjouterCommande.Enabled = false;
            btnModifierCommande.Enabled = false;
            btnSupprimerCommande.Enabled = false;
            btnValider.Enabled = false;
        }

        /// <summary>
        /// Activer les boutons d'actions sur les commandes de livre.
        /// </summary>
        public void ActiverBoutonsActionCommandeLivre_Commande()
        {
            btnAjouterCommande.Enabled = true;
            btnModifierCommande.Enabled = true;
            btnSupprimerCommande.Enabled = true;
            btnValider.Enabled = true;
        }

        /// <summary>
        /// Réinitialisation des champs de recherche de livres.
        /// </summary>
        public void ViderZoneRechercheLivres_Commande()
        {
            txtRechercherNumeroLivre.Text = "";
            txtRechercherTitreLivre.Text = "";
        }

        /// <summary>
        /// Réinitialisation des ComboBox de filtres par catégories : 'cmbGenresLivres', 'cmbRayonsLivres', et 'cmbPublicsLivres'.
        /// </summary>
        private void ReinitialiserComboBoxFiltresLivresCategories()
        {
            cmbGenresLivres.SelectedIndex = -1;
            cmbRayonsLivres.SelectedIndex = -1;
            cmbPublicsLivres.SelectedIndex = -1;
        }

        /// <summary>
        /// Remplit le ComboBox avec les différents états de la commande.
        /// </summary>
        /// <param name="suivis"></param>
        /// <param name="bdgSuivi"></param>
        /// <param name="cbxSuivi"></param>
        public void RemplirComboEtatSuivisCommandeLivre_Commande(List<Suivi> suivis, BindingSource bdgSuivi, ComboBox cmbSuivi)
        {
            bdgSuivi.DataSource = suivis;
            cmbSuivi.DataSource = bdgSuivi;
            if (cmbSuivi.Items.Count > 0)
            {
                cmbSuivi.SelectedIndex = -1;
            }
        }



        /*---------- ---------- ---------- Évènements  ---------- ---------- ----------*/



        /// <summary>
        /// Événement : Ouverture de l'onglet 'Commandes de livres'.
        /// Actions : Initialisation de l'onglet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesLivres_Enter(object sender, EventArgs e)
        {

            lesLivresCommandes = controller.GetAllLivres();

            RemplirListeCompleteLivres_Commandes();

            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cmbGenresLivres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cmbPublicsLivres);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cmbRayonsLivres);

            RemplirComboEtatSuivisCommandeLivre_Commande(controller.GetAllSuivis(), bdgSuivis, cmbEtatCommande);

            ViderZoneRechercheLivres_Commande();

            DesactiverZoneDetailsCommandeLivre_Commande();
            DesactiverBoutonsActionCommandeLivre_Commande();
   
        }

        /// <summary>
        /// Événement : Changement de la valeur du ComboBox de filtre par genres "cmbGenresLivres".
        /// Actions : Réinitialisation des deux autres comboBox de filtres (cmbPublicsLivres, cmbRayonsLivres), Recherche des livres correspondants au genre sélectionné, Affichage des livres correspondants dans le DataGridView,  Vide les champs de recherche par titre et numéro de livre (txtRechercherNumeroLivre, txtRechercherTitreLivre).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbGenresLivres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGenresLivres.SelectedIndex > 0){

                cmbPublicsLivres.SelectedIndex = -1;
                cmbRayonsLivres.SelectedIndex = -1;

                Genre genre = (Genre)cmbGenresLivres.SelectedItem;
                List<Livre> livres = lesLivresCommandes.FindAll(x => x.Genre.Equals(genre.Libelle));

                RemplirListeLivres_Commandes(livres);
            }

        }

        /// <summary>
        /// Événement : Changement de la valeur du ComboBox de filtre par publics "cmbPublicsLivres".
        /// Actions : Réinitialisation des deux autres comboBox de filtres (cmbGenresLivres, cmbRayonsLivres), Recherche des livres correspondants au public sélectionné, Affichage des livres correspondants dans le DataGridView,  Vide les champs de recherche par titre et numéro de livre (txtRechercherNumeroLivre, txtRechercherTitreLivre).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPublicsLivres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPublicsLivres.SelectedIndex > 0)
            {

                cmbGenresLivres.SelectedIndex = -1;
                cmbRayonsLivres.SelectedIndex = -1;

                Public publics = (Public)cmbPublicsLivres.SelectedItem;
                List<Livre> livres = lesLivresCommandes.FindAll(x => x.Public.Equals(publics.Libelle));

                RemplirListeLivres_Commandes(livres);

                ViderZoneRechercheLivres_Commande();
            }

        }

        /// <summary>
        /// Événement : Changement de la valeur du ComboBox de filtre par rayons "cmbRayonsLivres".
        /// Actions : Réinitialisation des deux autres comboBox de filtres (cmbGenresLivres, cmbPublicsLivres), Recherche des livres correspondants au rayon sélectionné, Affichage des livres correspondants dans le DataGridView,  Vide les champs de recherche par titre et numéro de livre (txtRechercherNumeroLivre, txtRechercherTitreLivre).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRayonsLivres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRayonsLivres.SelectedIndex > 0)
            {

                cmbGenresLivres.SelectedIndex = -1;
                cmbPublicsLivres.SelectedIndex = -1;

                Rayon rayon = (Rayon)cmbRayonsLivres.SelectedItem;
                List<Livre> livres = lesLivresCommandes.FindAll(x => x.Rayon.Equals(rayon.Libelle));

                RemplirListeLivres_Commandes(livres);

                ViderZoneRechercheLivres_Commande();
            }
            
        }

        /// <summary>
        /// Événement : Clique sur le bouton d'annulation de filtres sur les livres selon le genre "btnAnnulerGenres".
        /// Actions : Remplis le DataGrideView dgvListeLivres avec tous les livres, Reinitialisation des filtres par categories.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerGenres_Click(object sender, EventArgs e)
        {
            RemplirListeCompleteLivres_Commandes();
            ReinitialiserComboBoxFiltresLivresCategories();
        }

        /// <summary>
        /// Événement : Clique sur le bouton d'annulation de filtres sur les livres selon le public "btnAnnulerPublics".
        /// Actions : Remplis le DataGrideView dgvListeLivres avec tous les livres, Reinitialisation des filtres par categories.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerPublics_Click(object sender, EventArgs e)
        {
            RemplirListeCompleteLivres_Commandes();
            ReinitialiserComboBoxFiltresLivresCategories();
        }

        /// <summary>
        /// Évènement : Clique sur le bouton d'annulation de filtres sur les livres selon le genre "btnAnnulerRayons".
        /// Actions : Remplis le DataGrideView dgvListeLivres avec tous les livres, Reinitialisation des filtres par categories.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerRayons_Click(object sender, EventArgs e)
        {
            RemplirListeCompleteLivres_Commandes();
            ReinitialiserComboBoxFiltresLivresCategories();
        }

        /// <summary>
        /// Évènement : Clique sur le bouton de recherche de livre par son numéro 'btnRechercherNumeroLivre'.
        /// Actions : Affiche le livre dans le DataGridView 'dgvListeLivres' qui a le numéro de livre saisi dans le TextBox 'txtRechercherNumeroLivre'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRechercherNumeroLivre_Click(object sender, EventArgs e)
        {
            dgvListeLivres.ClearSelection();

            if (txtRechercherNumeroLivre.Text != "")
            {

                Livre livre = lesLivresCommandes.Find(x => x.Id.Equals(txtRechercherNumeroLivre.Text));
                List<Livre> livres = new List<Livre>();

                ReinitialiserComboBoxFiltresLivresCategories();

                if (livre != null)
                {
                    txtRechercherTitreLivre.Text = "";
                    RemplirListeCompleteLivres_Commandes(); 
                    livres.Add(livre);
                    RemplirListeLivres_Commandes(livres);   
                }
                else
                {
                    MessageBox.Show("Numéro de livre introuvable");
                    RemplirListeCompleteLivres_Commandes();
                    ViderZoneRechercheLivres_Commande();
                }
            }
            else
            {
                txtRechercherTitreLivre.Text = "";
                ReinitialiserComboBoxFiltresLivresCategories();
                RemplirListeCompleteLivres_Commandes();
            }
        }

        /// <summary>
        /// Évènement : Saisie dans le TextBox de recherche de livre par titre 'txtRechercherTitreLivre'.
        /// Action : Affiche des livres dans le DataGrideView 'dgvListeLivres' qui contiennent dans leur titre la chaine de caractère saisie dans le TextBox 'txtRechercherTitreLivre'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRechercherTitreLivre_TextChanged(object sender, EventArgs e)
        {
            txtRechercherNumeroLivre.Text = "";
            ReinitialiserComboBoxFiltresLivresCategories();

            List<Livre> livres = new List<Livre>();
            livres = lesLivresCommandes.FindAll(x => x.Titre.ToLower().Contains(txtRechercherTitreLivre.Text.ToLower()));
            RemplirListeLivres_Commandes(livres);
        }

        /// <summary>
        /// Évènement : Sélection d'une ligne (un livre) dans le DataGridView 'dgvListeLivres'.
        /// Actions : Affiche less commandes du livre sélectionné dans le DataGrideView des commandes 'dgvListeCommandes'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeLivres_SelectionChanged(object sender, EventArgs e)
        {     
            if (dgvListeLivres.SelectedRows.Count > 0)
            {
                Livre livre = (Livre)bdgListeLivres_Commandes.List[bdgListeLivres_Commandes.Position];
                AfficherCommandesLivre_Commandes(livre);

                btnAjouterCommande.Enabled = true; // nouveau
                btnValider.Enabled = true; // nouveau

                btnModifierCommande.Enabled = false; // nouveau
                btnSupprimerCommande.Enabled = false; // nouveau

                dgvListeCommandes.ClearSelection(); // nouveau

            }

            grpActionsCommande.Text = "Actions";
        }

        /// <summary>
        /// Évènement : Sélection d'une commande dans le DataGridView des commandes d'un livre 'dgvListeCommandes'
        /// Actions : Affiche les détails de la commande dans la section dédiée aux détails d'une commande.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeCommandes_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvListeCommandes.SelectedRows.Count > 0)
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivre_Commandes[bdgCommandesLivre_Commandes.Position];
                AfficherDetailsCommandeLivre_Commande(commandeDocument);

                btnModifierCommande.Enabled = true; // nouveau 
                btnSupprimerCommande.Enabled = true; // nouveau
                btnValider.Enabled = true; // nouveau

                btnAjouterCommande.Enabled = false; // nouveau

                dgvListeLivres.ClearSelection(); // nouveau
                
            }
        }

        /// <summary>
        /// Évènement : Clique sur le bouton d'ajout d'une commande 'btnAjouterCommande'.
        /// Actions : Préparation de l'ajout d'une commande.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterCommande_Click(object sender, EventArgs e)
        {

            ViderZoneDetailsCommandeLivre_Commande();
            ActiverZoneDetailsCommandeLivre_Commande();

            cmbEtatCommande.SelectedIndex = 0;
            cmbEtatCommande.Enabled = false;

            actionCommandeLivre = "ajouter";
            grpActionsCommande.Text = "Action : ajouter";
            
        }

        /// <summary>
        /// Évènement : Clique sur le bouton de modification d'une commande 'btnModifierCommande'.
        /// Actions : Préparation de la modification d'une commande.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierCommande_Click(object sender, EventArgs e)
        {

            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivre_Commandes[bdgCommandesLivre_Commandes.Position];
            string etat = commandeDocument.Etat;

            ActiverZoneDetailsCommandeLivre_Commande();

            actionCommandeLivre = "modifier";
            grpActionsCommande.Text = "Action : modifier";
        }

        /// <summary>
        /// Évènement : Clique sur le bouton de suppression d'une commande 'btnSupprimerCommande'.
        /// Actions : Préparation a la suppression d'une commande.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerCommande_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivre_Commandes[bdgCommandesLivre_Commandes.Position];
            string etat = commandeDocument.Etat;

            DesactiverZoneDetailsCommandeLivre_Commande();
            actionCommandeLivre = "supprimer";

            grpActionsCommande.Text = "Action : supprimer";
        }

        /// <summary>
        /// Évènement : Clique sur le bouton de validation d'action 'btnValider'.
        /// Action : Valider l'action en cours.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValider_Click(object sender, EventArgs e)
        {
            ValiderActionCommandeLivre();
        }

        /// <summary>
        /// Évènement : Clique sur le haut d'une des colonne.
        /// Action : Tris sur la colonnes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeLivres_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titre = dgvListeLivres.Columns[e.ColumnIndex].HeaderText;
            List<Livre> listeLivresTri = new List<Livre>();

            switch (titre)
            {
                case "Id":
                    listeLivresTri = lesLivresCommandes.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    listeLivresTri = lesLivresCommandes.OrderBy(o => o.Titre).ToList();
                    break;
                case "Auteur":
                    listeLivresTri = lesLivresCommandes.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Collection":
                    listeLivresTri = lesLivresCommandes.OrderBy(o => o.Collection).ToList();
                    break;
                case "Genre":
                    listeLivresTri = lesLivresCommandes.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    listeLivresTri = lesLivresCommandes.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    listeLivresTri = lesLivresCommandes.OrderBy(o => o.Rayon).ToList();
                    break;
            }

            RemplirListeLivres_Commandes(listeLivresTri);
        }

        #endregion

        #region Onglet Commandes de DVD

        /* ---------- ---------- ---------- ---------- Déclarations et Initialisations ---------- ---------- ---------- ---------- */

        List<Dvd> lesDvd_Commandes = new List<Dvd>();
        string ActionCommandeDvd = "";

        BindingSource bdgDvd_Commandes = new BindingSource();
        BindingSource bdgDvdCommandes_Commandes = new BindingSource();

        /* ---------- ---------- ---------- ---------- Méthodes ---------- ---------- ---------- ---------- */

        /// <summary>
        /// Remplir le DataGridView 'dgvListeDvd_Commandes' avec la liste des DVD passée en paramètre.
        /// </summary>
        /// <param name="dvd"></param>
        private void RemplirDvdListe_Commandes(List<Dvd> dvd)
        {
            bdgDvd_Commandes.DataSource = dvd;
            dgvListeDvd_Commandes.DataSource = bdgDvd_Commandes;

            dgvListeDvd_Commandes.Columns["Image"].Visible = false;
            dgvListeDvd_Commandes.Columns["IdGenre"].Visible = false;
            dgvListeDvd_Commandes.Columns["IdPublic"].Visible = false;
            dgvListeDvd_Commandes.Columns["IdRayon"].Visible = false;
            dgvListeDvd_Commandes.Columns["Synopsis"].Visible = false;

            dgvListeDvd_Commandes.Columns["Id"].DisplayIndex = 0;
            dgvListeDvd_Commandes.Columns["Titre"].DisplayIndex = 1;

           dgvListeDvd_Commandes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
           dgvListeDvd_Commandes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Remplir le DataGridView 'dgvListeDvd_Commandes' avec la liste complète des DVD passée en paramètre.
        /// </summary>
        /// <param name="dvd"></param>
        private void RemplirDvdListeComplete_Commandes()
        {
            RemplirDvdListe_Commandes(lesDvd_Commandes);
        }

        /// <summary>
        /// Remplir le DataGridView 'dgvListeCommandesDvd' avec la liste de commandes de DVD passée en paramètre.
        /// </summary>
        /// <param name="commandeDocuments"></param>
        public void RemplirCommandesDvdList(List<CommandeDocument> commandeDocuments)
        {
            bdgDvdCommandes_Commandes.DataSource = commandeDocuments;
            dgvListeCommandesDvd.DataSource = bdgDvdCommandes_Commandes;

            dgvListeCommandesDvd.Columns["IdLivreDvd"].Visible = false;
            dgvListeCommandesDvd.Columns["IdSuivi"].Visible = false;

            dgvListeCommandesDvd.Columns["Id"].DisplayIndex = 0;
            dgvListeCommandesDvd.Columns["Montant"].DisplayIndex = 2;

            dgvListeCommandesDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        /// <summary>
        /// Affichage des commandes du DVD correspondant à l'identifiant passé en paramètre.
        /// </summary>
        /// <param name="idDvd"></param>
        public void AfficherCommandesDvd(string idDvd)
        {
            ViderZoneDetailsCommandeDvd();
            DesactiverZoneDetailsCommandeDvd();

            txtNumeroDvd.Text = idDvd;

            List<CommandeDocument> commandeDocuments = new List<CommandeDocument>();
            commandeDocuments = controller.GetCommandesLivre(idDvd);
            RemplirCommandesDvdList(commandeDocuments);

        }

        /// <summary>
        /// Désactivation de la zone de détails des DVD.
        /// </summary>
        public void DesactiverZoneDetailsCommandeDvd()
        {

            txtNumeroCommandeDvd.Enabled = false;
            txtNumeroDvd.Enabled = false;
            txtNombreExemplaireDvd.Enabled = false;
            txtMontantCommandeDvd.Enabled = false;
            cmbEtatCommandeDvd.Enabled = false;
            dtpDateCommandeDvd.Enabled = false;
        }

        /// <summary>
        /// Désactivation des boutons d'action des commandes de DVD.
        /// </summary>
        public void DesactiverBoutonsActionsCommandeDvd()
        {
            btnAjouterCommandeDvd.Enabled = false;
            btnModifierCommandeDvd.Enabled = false;
            btnSupprimerCommandeDvd.Enabled = false;
            btnValiderActionCommandeDvd.Enabled = false;
        }

        /// <summary>
        /// Affichage des détails de la commande de DVD passée en paramètre dans la zone de détails.
        /// </summary>
        /// <param name="commandeDocument"></param>
        public void AfficherDetailsCommandeDvd(CommandeDocument commandeDocument)
        {
            txtNumeroCommandeDvd.Text = commandeDocument.Id;
            txtNombreExemplaireDvd.Text = commandeDocument.NbExemplaire.ToString();
            txtMontantCommandeDvd.Text = commandeDocument.Montant.ToString();
            cmbEtatCommandeDvd.SelectedIndex = cmbEtatCommandeDvd.FindString(commandeDocument.Etat);
            dtpDateCommandeDvd.Value = commandeDocument.DateCommande;
        }

        /// <summary>
        /// Vider la zone de détails des commandes des DVD.
        /// </summary>
        public void ViderZoneDetailsCommandeDvd()
        {
            txtNumeroCommandeDvd.Text = "";
            txtNombreExemplaireDvd.Text = "";
            txtMontantCommandeDvd.Text = "";
            cmbEtatCommandeDvd.SelectedIndex = -1;
            dtpDateCommandeDvd.Value = DateTime.Now;
        }

        /// <summary>
        /// Valider l'action en cours (Ajouter, Modifier, Supprimer).
        /// </summary>
        public void ValiderActionCommandeDvd()
        {

            string id = "";
            DateTime dateCommande;
            float montant = -1;
            int nbExemplaire = -1;
            string idLivreDvd;
            int idSuivi = -1;
            string etat = "";


            if (ActionCommandeDvd == "AjouterCommandeDvd")
            {
                string maxId = controller.GetMaxIdCommande();

                MessageBox.Show(maxId);

                if (maxId == null)
                {
                    id = "1";
                    txtNumeroCommande.Text = id;
                }
                else
                {
                    int maxIdInt = int.Parse(maxId);
                    maxIdInt++;
                    id = maxIdInt.ToString();
                }

                if (txtNombreExemplaireDvd.Text == "" || txtMontantCommandeDvd.Text == "" || cmbEtatCommandeDvd.SelectedIndex < 0)
                {
                    MessageBox.Show("Veuillez remplir tous les champs.", "Erreur d'ajout");

                    Dvd dvd = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                    AfficherCommandesDvd(dvd.Id);

                    ViderZoneDetailsCommandeDvd();
                    dgvListeCommandesDvd.ClearSelection();
                }
                else
                {
                    if (!estNombreEntier(txtNombreExemplaireDvd.Text) || !estNombreEntier(txtMontantCommandeDvd.Text))
                    {
                        MessageBox.Show("Veuillez respecter le typage des données saisies.", "Erreur d'ajout");

                        Dvd dvd = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                        AfficherCommandesDvd(dvd.Id);

                        ViderZoneDetailsCommandeDvd();
                        dgvListeCommandesDvd.ClearSelection();
                    }
                    else
                    {
                        if (cmbEtatCommandeDvd.SelectedIndex == 1)
                        {
                            MessageBox.Show("Une nouvelle commande ne peut pas être marquée comme 'livrée' avant d'avoir le statut 'en cours'.", "Erreur d'ajout");

                            Dvd dvd = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                            AfficherCommandesDvd(dvd.Id);

                            ViderZoneDetailsCommandeDvd();
                            dgvListeCommandesDvd.ClearSelection();

                        }
                        else
                        {
                            dateCommande = dtpDateCommandeDvd.Value;
                            montant = float.Parse(txtMontantCommandeDvd.Text);
                            nbExemplaire = int.Parse(txtNombreExemplaireDvd.Text);
                            idLivreDvd = txtNumeroDvd.Text;
                            Suivi suivi = (Suivi)cmbEtatCommandeDvd.SelectedItem;
                            idSuivi = suivi.Id;
                            etat = suivi.Etat;

                            CommandeDocument commande = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                            controller.AjouterCommande(commande);

                            Dvd dvd = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                            AfficherCommandesDvd(dvd.Id);

                            ViderZoneDetailsCommandeDvd();
                            dgvListeCommandesDvd.ClearSelection();

                        }
                    }
                }
            }
            else
            {
                if (ActionCommandeDvd == "ModifierCommandeDvd")
                {
                    if(dgvListeCommandesDvd.SelectedRows.Count > 0)
                    {
                        if (txtNombreExemplaireDvd.Text == "" || txtMontantCommandeDvd.Text == "" || cmbEtatCommandeDvd.SelectedIndex < 0)
                        {
                            MessageBox.Show("Veuillez remplir tous les champs.", "Erreur d'ajout");

                            Dvd dvd = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                            AfficherCommandesDvd(dvd.Id);

                            ViderZoneDetailsCommandeDvd();
                            dgvListeCommandesDvd.ClearSelection();
                        }
                        else
                        {
                            if (!estNombreEntier(txtNombreExemplaireDvd.Text) || !estNombreVirgule(txtMontantCommandeDvd.Text))
                            {
                                MessageBox.Show("Veuillez respecter le typage des données saisies.", "Erreur d'ajout");

                                Dvd dvd = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                                AfficherCommandesDvd(dvd.Id);

                                ViderZoneDetailsCommandeDvd();
                                dgvListeCommandesDvd.ClearSelection();

                            }
                            else
                            {
                                id = txtNumeroCommandeDvd.Text;
                                dateCommande = dtpDateCommandeDvd.Value;
                                montant = float.Parse(txtMontantCommandeDvd.Text);
                                nbExemplaire = int.Parse(txtNombreExemplaireDvd.Text);
                                idLivreDvd = txtNumeroDvd.Text;
                                Suivi suivi = (Suivi)cmbEtatCommandeDvd.SelectedItem;
                                idSuivi = suivi.Id;
                                etat = suivi.Etat;

                                CommandeDocument commande = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                                controller.ModifierCommande(commande);

                                Dvd dvd = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                                AfficherCommandesDvd(dvd.Id);

                                ViderZoneDetailsCommandeDvd();
                                dgvListeCommandesDvd.ClearSelection();

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Veuillez séléctionner une commande.", "Erreur de modification");
                    }
                }
                else
                {
                    if (ActionCommandeDvd == "SupprimerCommandeDvd")
                    {

                        if(dgvListeCommandesDvd.SelectedRows.Count > 0)
                        {
                            id = txtNumeroCommandeDvd.Text;
                            dateCommande = dtpDateCommandeDvd.Value;
                            montant = float.Parse(txtMontantCommandeDvd.Text);
                            nbExemplaire = int.Parse(txtNombreExemplaireDvd.Text);
                            idLivreDvd = txtNumeroDvd.Text;
                            Suivi suivi = (Suivi)cmbEtatCommandeDvd.SelectedItem;
                            idSuivi = suivi.Id;
                            etat = suivi.Etat;

                            if (etat == "Livrée")
                            {
                                MessageBox.Show("Vous ne pouvez pas supprimer une commande qui a déjà été livrée", "Erreur de suppression");

                                Dvd dvd_ = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                                AfficherCommandesDvd(dvd_.Id);

                                ViderZoneDetailsCommandeDvd();
                                dgvListeCommandesDvd.ClearSelection();
                            }
                            else
                            {
                                CommandeDocument commande = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                                controller.SupprimerCommande(commande);

                                Dvd dvd_ = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                                AfficherCommandesDvd(dvd_.Id);

                                ViderZoneDetailsCommandeDvd();
                                dgvListeCommandesDvd.ClearSelection();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Veuillez séléctionner une commande.", "Erreur de modification");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Réinitialisation des ComboBox de filtrage des DVD par genre, public, rayon.
        /// </summary>
        public void ReinitialiserComboFiltresDvd()
        {
            cmbGenreDvd.SelectedIndex = -1;
            cmbPublicDvd.SelectedIndex = -1;
            cmbRayonDvd.SelectedIndex = -1;
        }

        /// <summary>
        /// Activer la zone de détails des DVD.
        /// </summary>
        public void ActiverZoneDetailsDvd()
        {
            txtMontantCommandeDvd.Enabled = true;
            txtNombreExemplaireDvd.Enabled = true;
            txtMontantCommandeDvd.Enabled = true;
            cmbEtatCommandeDvd.Enabled = true;
            dtpDateCommandeDvd.Enabled = true;
        }

        /// <summary>
        /// Vider la zone de recherche d'un DVD.
        /// </summary>
        public void ViderZoneRechercheDvd()
        {
            txtRechercherNumeroDvd.Text = "";
            txtRechercherTitreDvd.Text = "";
        }

        /* ---------- ---------- ---------- ---------- Évènement ---------- ---------- ---------- ---------- */

        /// <summary>
        /// Événement : Ouverture de l'onglet 'Commandes de DVD'.
        /// Actions : Initialisation de l'onglet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesDvd_Enter(object sender, EventArgs e)
        {
            lesDvd_Commandes = controller.GetAllDvd();
            RemplirDvdListeComplete_Commandes();

            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cmbGenreDvd);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cmbPublicDvd);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cmbRayonDvd);

            RemplirComboEtatSuivisCommandeLivre_Commande(controller.GetAllSuivis(), bdgSuivis, cmbEtatCommandeDvd);

            DesactiverZoneDetailsCommandeDvd();
            DesactiverBoutonsActionsCommandeDvd();
        }

        /// <summary>
        /// Événement : Clic sur le bouton 'btnRechercherDvd'.
        /// Actions : Recherche et affichage du DVD correspondant au numéro du DVD saisi dans le champ correspondant.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRechercherDvd_Click(object sender, EventArgs e)
        {
            Dvd dvd = lesDvd_Commandes.Find(x => x.Id.Equals(txtRechercherNumeroDvd.Text));

            if(txtRechercherNumeroDvd.Text == "")
            {
                txtRechercherTitreDvd.Text = "";

                RemplirDvdListeComplete_Commandes();
            }
            else
            {
                if(dvd == null)
                {
                    MessageBox.Show("Numéro de DVD introuvable.", "Erreur");

                    txtRechercherNumeroDvd.Text = "";
                    txtRechercherTitreDvd.Text = "";

                    RemplirDvdListeComplete_Commandes();
                }
                else
                {
                    txtRechercherTitreDvd.Text = "";

                    List<Dvd> listDvd = new List<Dvd>();
                    listDvd.Add(dvd);
                    RemplirDvdListe_Commandes(listDvd);
                }

            }
            
        }

        /// <summary>
        /// Événement : Saisie dans le champ 'txtRechercherTitreDvd'.
        /// Actions : Recherche et affichage des DVD dont les titres contiennent la chaîne de caractères saisie dans le champ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRechercherTitreDvd_TextChanged(object sender, EventArgs e)
        {
            txtRechercherNumeroDvd.Text = "";

            List<Dvd> lesDvd = new List<Dvd>(); 
            lesDvd = lesDvd_Commandes.FindAll(x => x.Titre.ToLower().Contains(txtRechercherTitreDvd.Text.ToLower()));
            RemplirDvdListe_Commandes(lesDvd);
        }

        /// <summary>
        /// Événement : Sélection d'un genre dans le menu déroulant 'cmbGenreDvd'.
        /// Action : Affichage des DVD faisant partie de ce genre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbGenreDvd_SelectedIndexChanged(object sender, EventArgs e)
        {
            

            List<Dvd> lesDvd = new List<Dvd>();
            Genre genre = (Genre)cmbGenreDvd.SelectedItem;
            if(genre != null && cmbGenreDvd.SelectedIndex > 0)
            {
                lesDvd = lesDvd_Commandes.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe_Commandes(lesDvd);

                cmbPublicDvd.SelectedIndex = -1;
                cmbRayonDvd.SelectedIndex = -1;
            }
            
        }

        /// <summary>
        /// Événement : Sélection d'un public dans le menu déroulant 'cmbPublicDvd'.
        /// Action : Affichage des DVD faisant partie de ce public.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPublicDvd_SelectedIndexChanged(object sender, EventArgs e)
        {
           

            List<Dvd> lesDvd = new List<Dvd>();
            Public public_categorie = (Public)cmbPublicDvd.SelectedItem;

            if(public_categorie != null && cmbPublicDvd.SelectedIndex > 0)
            {
                lesDvd = lesDvd_Commandes.FindAll(x => x.Public.Equals(public_categorie.Libelle));
                RemplirDvdListe_Commandes(lesDvd);

                cmbGenreDvd.SelectedIndex = -1;
                cmbRayonDvd.SelectedIndex = -1;
            }
            
        }

        /// <summary>
        /// Événement : Sélection d'un rayon dans le menu déroulant 'cmbRayonDvd'.
        /// Action : Affichage des DVD faisant partie de ce rayon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRayonDvd_SelectedIndexChanged(object sender, EventArgs e)
        {
           

            List<Dvd> lesDvd = new List<Dvd>();
            Rayon rayon = (Rayon)cmbRayonDvd.SelectedItem;

            if(rayon != null && cmbRayonDvd.SelectedIndex > 0)
            {
                lesDvd = lesDvd_Commandes.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe_Commandes(lesDvd);

                cmbGenreDvd.SelectedIndex = -1;
                cmbPublicDvd.SelectedIndex = -1;
            }

        }

        /// <summary>
        /// Événement : Sélection d'une ligne (d'un DVD) du DataGridView 'dgvListeDvd_Commandes'.
        /// Action : Affichage des commandes sur ce DVD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeDvd_Commandes_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvListeDvd_Commandes.SelectedRows.Count > 0)
            {
                Dvd dvd = (Dvd)bdgDvd_Commandes.List[bdgDvd_Commandes.Position];
                AfficherCommandesDvd(dvd.Id);

                btnAjouterCommandeDvd.Enabled = true;
                btnValiderActionCommandeDvd.Enabled = true;

                btnModifierCommandeDvd.Enabled = false;
                btnSupprimerCommandeDvd.Enabled = false;

                dgvListeCommandesDvd.ClearSelection();
            }

        }

        /// <summary>
        /// Événement : Sélection d'une ligne (d'une commande) du DataGridView 'dgvListeCommandesDvd'.
        /// Actions : Affichage des détails de la commande.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeCommandesDvd_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvListeCommandesDvd.SelectedRows.Count > 0)
            {
                dgvListeDvd_Commandes.ClearSelection();

                CommandeDocument commandeDocument = (CommandeDocument)bdgDvdCommandes_Commandes.List[bdgDvdCommandes_Commandes.Position];
                AfficherDetailsCommandeDvd(commandeDocument);

                btnAjouterCommandeDvd.Enabled = false;
                btnModifierCommandeDvd.Enabled = true;

                btnSupprimerCommandeDvd.Enabled = true;
                btnValiderActionCommandeDvd.Enabled = true;

                dgvListeDvd_Commandes.ClearSelection();
            }
            
        }

        /// <summary>
        /// Événement : Clic sur le bouton d'ajout d'une commande 'btnAjouterCommandeDvd'.
        /// Action : Ajouter une commande d'un DVD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterCommandeDvd_Click(object sender, EventArgs e)
        {
            ViderZoneDetailsCommandeDvd();
            ActiverZoneDetailsDvd();
            
            ActionCommandeDvd = "AjouterCommandeDvd";
            grpActionsDvd.Text = "Action : ajouter";
        }

        /// <summary>
        /// Événement : Clic sur le bouton de modification d'une commande 'btnModifierCommandeDvd'.
        /// Action : Modifier une commande d'un DVD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierCommandeDvd_Click(object sender, EventArgs e)
        {
            ActiverZoneDetailsDvd();

            CommandeDocument commandeDocument = (CommandeDocument)bdgDvdCommandes_Commandes[bdgDvdCommandes_Commandes.Position];
            string etat = commandeDocument.Etat;
            if (etat == "Livrée")
            {
                cmbEtatCommandeDvd.Enabled = false;
            }

            ActionCommandeDvd = "ModifierCommandeDvd";
            grpActionsDvd.Text = "Action : modifier";
        }

        /// <summary>
        /// Événement : Clic sur le bouton de suppression d'une commande 'btnSupprimerCommandeDvd'
        /// Action : Supprimer une commande d'un DVD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerCommandeDvd_Click(object sender, EventArgs e)
        {
            DesactiverZoneDetailsCommandeDvd();

            ActionCommandeDvd = "SupprimerCommandeDvd";
            grpActionsDvd.Text = "Action : supprimer";

        }

        /// <summary>
        /// Événement : Clique sur le bouton de validation d'action 'btnValiderActionCommandeDvd'.
        /// Action : Valider une commande.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderActionCommandeDvd_Click(object sender, EventArgs e)
        {
            ValiderActionCommandeDvd();
        }

        /// <summary>
        /// Événement : Clique sur le bouton d'annulation du filtre des DVD par genre 'btnAnnulerGenresDvd'.
        /// Action : Annulation du filtre par genre des DVD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerGenresDvd_Click(object sender, EventArgs e)
        {
            ViderZoneRechercheDvd();
            ReinitialiserComboFiltresDvd();
            RemplirDvdListeComplete_Commandes();
        }

        /// <summary>
        /// Événement : Clic sur le bouton d'annulation du filtre des DVD par public 'btnAnnulerPublicsDvd'. 
        /// Action : Annulation du filtre par public des DVD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerPublicsDvd_Click(object sender, EventArgs e)
        {
            ViderZoneRechercheDvd();
            ReinitialiserComboFiltresDvd();
            RemplirDvdListeComplete_Commandes();
        }

        /// <summary>
        /// Événement : Clic sur le bouton d'annulation du filtre des DVD par Rayon.
        /// Action : Annulation du filtre par rayon des DVD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerRayonsDvd_Click(object sender, EventArgs e)
        {
            ViderZoneRechercheDvd();
            ReinitialiserComboFiltresDvd();
            RemplirDvdListeComplete_Commandes();
        }

        /// <summary>
        /// Évènement : Clique sur le haut d'une des colonne.
        /// Action : Tris sur la colonnes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeDvd_Commandes_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titre = dgvListeDvd_Commandes.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> listeDvdTri = new List<Dvd>();

            switch (titre)
            {
                case "Id":
                    listeDvdTri = lesDvd_Commandes.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    listeDvdTri = lesDvd_Commandes.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    listeDvdTri = lesDvd_Commandes.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    listeDvdTri = lesDvd_Commandes.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    listeDvdTri = lesDvd_Commandes.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    listeDvdTri = lesDvd_Commandes.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    listeDvdTri = lesDvd_Commandes.OrderBy(o => o.Rayon).ToList();
                    break;
            }

            RemplirDvdListe_Commandes(listeDvdTri);
        }

        #endregion

        #region Onglet Abonnements aux revues

        /*---------- ---------- ---------- ---------- Déclarations et Initialisations ---------- ---------- ---------- ---------- ----------*/

        List<Revue> lesRevues_Abonnements = new List<Revue>();
        String actionAbonnement;

        BindingSource bdgRevues = new BindingSource();
        BindingSource bdgAbonnements_revue = new BindingSource();

        /*---------- ---------- ---------- ---------- Méthodes ---------- ---------- ---------- ---------- ----------*/

        /// <summary>
        /// Remplir le DataGridView 'dgvListeRevues_Abonnements' avec la liste des revues passée en paramètre.
        /// </summary>
        /// <param name="revues"></param>
        public void RemplirListeRevues_Abonnements(List<Revue> revues)
        {
            bdgRevues.DataSource = revues;
            dgvListeRevues_Abonnements.DataSource = bdgRevues;

            dgvListeRevues_Abonnements.Columns["idGenre"].Visible = false;
            dgvListeRevues_Abonnements.Columns["idPublic"].Visible = false;
            dgvListeRevues_Abonnements.Columns["idRayon"].Visible = false;
            dgvListeRevues_Abonnements.Columns["Image"].Visible = false;

            dgvListeRevues_Abonnements.Columns["Id"].DisplayIndex = 0;
            dgvListeRevues_Abonnements.Columns["Titre"].DisplayIndex = 1;

            dgvListeRevues_Abonnements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvListeRevues_Abonnements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        /// <summary>
        /// Remplir le DataGridView 'dgvListeRevues_Abonnements' avec toutes les revues.
        /// </summary>
        public void RemplirListeCompleteRevues_Abonnements()
        {
            RemplirListeRevues_Abonnements(controller.GetAllRevues());
        }

        /// <summary>
        /// Réinitialisation des ComboBox de filtre des revues (cmbFiltrerRevuesGenre, cmbFiltrerRevuesPublic, cmbFiltrerRevuesRayon).
        /// </summary>
        public void ReinitialiserComboCategories_Abonnements()
        {
            cmbFiltrerRevuesGenre.SelectedIndex = -1;
            cmbFiltrerRevuesPublic.SelectedIndex = -1;
            cmbFiltrerRevuesRayon.SelectedIndex = -1;
        }

        /// <summary>
        /// Remplir le DataGridView 'dgvAbonnementsRevue' avec la liste des abonnements d'une revue passée en paramètre.
        /// </summary>
        /// <param name="abonnements"></param>
        public void RemplirAbonnements_Abonnements(List<Abonnement> abonnements)
        {
            bdgAbonnements_revue.DataSource = abonnements;
            dgvAbonnementsRevue.DataSource = bdgAbonnements_revue;

            dgvAbonnementsRevue.Columns["IdRevue"].Visible = false;

            dgvAbonnementsRevue.Columns["Id"].DisplayIndex = 0;
            dgvAbonnementsRevue.Columns["Montant"].DisplayIndex = 1;
            dgvAbonnementsRevue.Columns["DateCommande"].DisplayIndex = 2;

            dgvAbonnementsRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

        }

        /// <summary>
        /// Affichage des abonnements d'une revue correspondant à l'identifiant de la revue passé en paramètre.
        /// </summary>
        /// <param name="idRevue"></param>
        public void AfficherAbonnements_Abonnements(string idRevue)
        {
            ViderZoneDetailsAbonnement();
            DesactiverZoneDetailsAbonnement();

            txtNumeroRevue.Text = idRevue;

            List<Abonnement> abonnements = new List<Abonnement>();
            abonnements = controller.GetAbonnements(idRevue);
            RemplirAbonnements_Abonnements(abonnements);
        }

        /// <summary>
        /// Affichage des détails d'un abonnement dans la zone de détails d'un abonnement.
        /// </summary>
        /// <param name="abonnement"></param>
        public void AfficherDetailsAbonnement(Abonnement abonnement)
        {
            txtNumeroAbonnementRevue.Text = abonnement.Id;
            txtNumeroRevue.Text = abonnement.IdRevue;
            txtMontantAbonnementRevue.Text = abonnement.Montant.ToString();
            dtpDateAbonnementRevue.Value = abonnement.DateCommande;
            dtpDateAbonnementFin.Value = abonnement.DateFinAbonnement;
        }

        /// <summary>
        /// Vider la zone de détails d'un abonnement.
        /// </summary>
        public void ViderZoneDetailsAbonnement()
        {
            txtNumeroAbonnementRevue.Text = "";
            txtMontantAbonnementRevue.Text = "";
            dtpDateAbonnementRevue.Value = DateTime.Now;
            dtpDateAbonnementFin.Value = DateTime.Now;
        }

        /// <summary>
        /// Désactiver les boutons d'action sur un abonnement.
        /// </summary>
        public void DesactiverBoutonsActionAbonnement()
        {
            btnAjouterAbonnement.Enabled = false;
            btnModifierAbonnement.Enabled = false;
            btnSupprimerAbonnement.Enabled = false;
            btnValiderActionAbonnement.Enabled = false;
        }

        /// <summary>
        /// Désactiver la zone de détails d'un abonnement.
        /// </summary>
        public void DesactiverZoneDetailsAbonnement()
        {
            txtNumeroAbonnementRevue.Enabled = false;
            txtNumeroRevue.Enabled = false;
            txtNumeroAbonnementRevue.Enabled = false;
            txtMontantAbonnementRevue.Enabled = false;
            dtpDateAbonnementRevue.Enabled = false;
            dtpDateAbonnementFin.Enabled = false;
        }

        /// <summary>
        /// Activer la zone de détails d'un abonnement.
        /// </summary>
        public void ActiverZoneDetailsAbonnement()
        {
            txtMontantAbonnementRevue.Enabled = true;
            dtpDateAbonnementRevue.Enabled = true;
            dtpDateAbonnementFin.Enabled = true;
        }

        /// <summary>
        /// Valider l'action en cours (Ajouter, Modifier, Supprimer)
        /// </summary>
        public void ValiderActionAbonnement()
        {
            string id = "";
            DateTime dateCommande;
            double montant = -1;
            DateTime dateFinAbonnement;
            string idRevue = "";

            if (actionAbonnement == "ActionAjouterAbonnement")
            {
                string maxId = controller.GetMaxIdCommande();

                if (maxId == null)
                {
                    id = "1";
                    txtNumeroAbonnementRevue.Text = id;
                }
                else
                {
                    int maxIdInt = int.Parse(maxId);
                    maxIdInt++;
                    id = maxIdInt.ToString();
                    txtNumeroAbonnementRevue.Text = id;
                }

                if(txtMontantAbonnementRevue.Text == "")
                {
                    MessageBox.Show("Veuillez remplir tous les champs.", "Erreur d'ajout");
                    ViderZoneDetailsAbonnement();
                }
                else
                {
                    if (!estNombreVirgule(txtMontantAbonnementRevue.Text))
                    {
                        MessageBox.Show("Veuillez respecter le typage des données saisies.", "Erreur d'ajout");
                        ViderZoneDetailsAbonnement();
                    }
                    else
                    {
                        id = txtNumeroAbonnementRevue.Text;
                        dateCommande = dtpDateAbonnementRevue.Value;
                        montant = float.Parse(txtMontantAbonnementRevue.Text);
                        dateFinAbonnement = dtpDateAbonnementFin.Value;
                        idRevue = txtNumeroRevue.Text;

                        Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue);
                        controller.AjouterAbonnement(abonnement);
                    }
                }
                
                AfficherAbonnements_Abonnements(idRevue);

            }
            else
            {
                if (actionAbonnement == "ActionModifierAbonnement")
                {
                    if (txtMontantAbonnementRevue.Text == "")
                    {
                        MessageBox.Show("Veuillez remplir tous les champs.", "Erreur d'ajout");
                        ViderZoneDetailsAbonnement();
                    }
                    else
                    {
                        if (!estNombreVirgule(txtMontantAbonnementRevue.Text))
                        {
                            MessageBox.Show("Veuillez respecter le typage des données saisies.", "Erreur d'ajout");
                            ViderZoneDetailsAbonnement();
                        }
                        else
                        {

                            id = txtNumeroAbonnementRevue.Text;
                            dateCommande = dtpDateAbonnementRevue.Value;
                            montant = float.Parse(txtMontantAbonnementRevue.Text);
                            dateFinAbonnement = dtpDateAbonnementFin.Value;
                            idRevue = txtNumeroRevue.Text;


                            Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue);
                            controller.ModifierAbonnement(abonnement);
                        }

                        AfficherAbonnements_Abonnements(idRevue);

                    }

                }
                else
                {
                    if (actionAbonnement == "ActionSupprimerAbonnement")
                    {
                        id = txtNumeroAbonnementRevue.Text;
                        dateCommande = dtpDateAbonnementRevue.Value;
                        montant = float.Parse(txtMontantAbonnementRevue.Text);
                        dateFinAbonnement = dtpDateAbonnementFin.Value;
                        idRevue = txtNumeroRevue.Text;


                        Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFinAbonnement, idRevue);

                        if (ParutionDansAbonnement(abonnement))
                        {
                            MessageBox.Show("Une revue a été livrée le temps de cet abonnement, il ne peut etre supprimée");
                        }
                        else
                        {
                            controller.SupprimerAbonnement(abonnement);
                        }
                        

                        AfficherAbonnements_Abonnements(idRevue);
                    }
                }
            }
        }

        /// <summary>
        /// Vérifie si un abonnement a des exemplaires associés qui ont été achetés dans la période de validité de l'abonnement. 
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        private bool ParutionDansAbonnement(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplairesAbo = controller.GetExemplairesRevue(abonnement.IdRevue);
            return lesExemplairesAbo.FindAll(o => (o.DateAchat >= abonnement.DateCommande) && (o.DateAchat <= abonnement.DateFinAbonnement)).Count > 0;
        }


        /*---------- ---------- ---------- ---------- Évènement ---------- ---------- ---------- ---------- ----------*/

        /// <summary>
        /// Évènement : Ouverture de l'onglet 'Abonnements aux revues'.
        /// Actions : Initialisation de l'onglet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabAbonnementsRevues_Enter(object sender, EventArgs e)
        {
            lesRevues_Abonnements = controller.GetAllRevues();
            RemplirListeCompleteRevues_Abonnements();

            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cmbFiltrerRevuesGenre);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cmbFiltrerRevuesPublic);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cmbFiltrerRevuesRayon);

            ReinitialiserComboCategories_Abonnements();
            DesactiverBoutonsActionAbonnement();
            DesactiverZoneDetailsAbonnement();
        }

        /// <summary>
        /// Évènement : Clique sur le bouton d'annulation du filtre des revues par genre 'btnAnnulerFiltreGenreRevues'.
        /// Actions : Annulation du filtre par genre des revues.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerFiltreGenreRevues_Click(object sender, EventArgs e)
        {
            ReinitialiserComboCategories_Abonnements();
            RemplirListeCompleteRevues_Abonnements();
        }

        /// <summary>
        /// Évènement : Clique sur le bouton d'annulation du filtre des revues par public 'btnAnnulerFiltrePublicRevues'.
        /// Actions : Annulation du filtre par public des revues.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerFiltrePublicRevues_Click(object sender, EventArgs e)
        {
            ReinitialiserComboCategories_Abonnements();
            RemplirListeCompleteRevues_Abonnements();
        }

        /// <summary>
        /// Évènement : Clique sur le bouton d'annulation du filtre des revues par rayon 'btnAnnulerFiltreRayonRevues'.
        /// Actions : Annulation du filtre par rayon des revues.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerFiltreRayonRevues_Click(object sender, EventArgs e)
        {
            ReinitialiserComboCategories_Abonnements();
            RemplirListeCompleteRevues_Abonnements();
        }

        /// <summary>
        /// Évènement : Sélection d'un genre dans le ComboBox 'cmbFiltrerRevuesGenre'.
        /// Actions : Affichage des revues faisant partie de ce genre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbFiltrerRevuesGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            Genre genre_categorie = (Genre)cmbFiltrerRevuesGenre.SelectedItem;
            List<Revue> revues = new List<Revue>();

            if(genre_categorie != null && cmbFiltrerRevuesGenre.SelectedIndex > 0)
            {
                revues = lesRevues_Abonnements.FindAll(x => x.Genre.Equals(genre_categorie.Libelle));
                RemplirListeRevues_Abonnements(revues);
            }
           
        }

        /// <summary>
        /// Évènement : Sélection d'un public dans le ComboBox 'cmbFiltrerRevuesPublic'.
        /// Actions : Affichage des revues faisant partie de ce public.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbFiltrerRevuesPublic_SelectedIndexChanged(object sender, EventArgs e)
        {
            Public public_categorie = (Public)cmbFiltrerRevuesPublic.SelectedItem;
            List<Revue> revues = new List<Revue>();

            if (public_categorie != null && cmbFiltrerRevuesPublic.SelectedIndex > 0)
            {
                revues = lesRevues_Abonnements.FindAll(x => x.Public.Equals(public_categorie.Libelle));
                RemplirListeRevues_Abonnements(revues);
            }
        }

        /// <summary>
        /// Évènement : Sélection d'un rayon dans le ComboBox 'cmbFiltrerRevuesRayon'
        /// Actions : Affichage des revues faisant partie de ce rayon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbFiltrerRevuesRayon_SelectedIndexChanged(object sender, EventArgs e)
        {
            Rayon rayon_categorie = (Rayon)cmbFiltrerRevuesRayon.SelectedItem;
            List<Revue> revues = new List<Revue>();

            if (rayon_categorie != null && cmbFiltrerRevuesRayon.SelectedIndex > 0)
            {
                revues = lesRevues_Abonnements.FindAll(x => x.Rayon.Equals(rayon_categorie.Libelle));
                RemplirListeRevues_Abonnements(revues);
            }
        }

        /// <summary>
        /// Évènement : Clic sur le bouton de recherche d'un numéro de revue 'btnRechercherRevue'.
        /// Actions : Affichage de la revue correspondant à ce numéro de revue dans le DataGrideView 'dgvListeRevues_Abonnements'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRechercherRevue_Click(object sender, EventArgs e)
        {
            Revue revue = lesRevues_Abonnements.Find(x => x.Id.Equals(txtRechercherNumeroRevue.Text));
            List<Revue> revues = new List<Revue>();
            revues.Add(revue);
            RemplirListeRevues_Abonnements(revues);
        }

        /// <summary>
        /// Évènement : Saisie d'une chaîne de caractères dans la zone de saisie 'txtRechercherTitreRevue'.
        /// Actions : Affichage des revues contenant dans leur titre la chaîne de caractères saisie dans le DataGrideView 'dgvListeRevues_Abonnements'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRechercherTitreRevue_TextChanged(object sender, EventArgs e)
        {
            List<Revue> lesRevues = new List<Revue>();
            lesRevues = lesRevues_Abonnements.FindAll(x => x.Titre.ToLower().Contains(txtRechercherTitreRevue.Text.ToLower()));
            RemplirListeRevues_Abonnements(lesRevues);
        }

        /// <summary>
        /// Évènement : Sélection d'une revue dans le DataGridView 'dgvListeRevues_Abonnements'.
        /// Actions : Affichage des abonnements dans le DataGridView 'dgvAbonnementsRevue'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeRevues_Abonnements_SelectionChanged(object sender, EventArgs e)
        {
            Revue revue = (Revue)bdgRevues.List[bdgRevues.Position];
            AfficherAbonnements_Abonnements(revue.Id);

            btnAjouterAbonnement.Enabled = true;
            btnValiderActionAbonnement.Enabled = true;

            btnModifierAbonnement.Enabled = false;
            btnSupprimerAbonnement.Enabled = false;

            dgvAbonnementsRevue.ClearSelection();
           
        }

        /// <summary>
        /// Événement : Sélection d'un abonnement dans le DataGridView 'dgvAbonnementsRevue'.
        /// Actions : Affichage des détails d'un abonnement dans la zone de détails d'un abonnement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsRevue_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvAbonnementsRevue.SelectedRows.Count > 0)
            {
                Abonnement abonnement = (Abonnement)bdgAbonnements_revue.List[bdgAbonnements_revue.Position];
                AfficherDetailsAbonnement(abonnement);

                btnModifierAbonnement.Enabled = true;
                btnSupprimerAbonnement.Enabled = true;

                btnAjouterAbonnement.Enabled = false;
                btnValiderActionAbonnement.Enabled = true;

                dgvListeRevues_Abonnements.ClearSelection();

            }

        }

        /// <summary>
        /// Événement : Clique sur le bouton d'ajout d'un abonnement 'btnAjouterAbonnement'.
        /// Actions : Ajouter un abonnement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterAbonnement_Click(object sender, EventArgs e)
        {
            ActiverZoneDetailsAbonnement();

            actionAbonnement = "ActionAjouterAbonnement";
        }

        /// <summary>
        /// Événement : Clique sur le bouton de modification d'un abonnement 'btnModifierAbonnement'.
        /// Actions : Modifier un abonnement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierAbonnement_Click(object sender, EventArgs e)
        {
            ActiverZoneDetailsAbonnement();

            actionAbonnement = "ActionModifierAbonnement";
        }

        /// <summary>
        /// Événement : Clique sur le bouton de suppression d'un abonnement 'btnSupprimerAbonnement'.
        /// Actions : Supprimer un abonnement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSupprimerAbonnement_Click(object sender, EventArgs e)
        {
            DesactiverZoneDetailsAbonnement();

            actionAbonnement = "ActionSupprimerAbonnement";
        }

        /// <summary>
        /// Événement : Clique sur le bouton de validation de l'action en cours sur un abonnment 'btnValiderActionAbonnement'.
        /// Actions : Valider l'action en cours sur un abonnement.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderActionAbonnement_Click(object sender, EventArgs e)
        {
            ValiderActionAbonnement();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeRevues_Abonnements_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titre = dgvListeRevues_Abonnements.Columns[e.ColumnIndex].HeaderText;
            List<Revue> listeRevueTri = new List<Revue>();

            switch (titre)
            {
                case "Id":
                    listeRevueTri = lesRevues_Abonnements.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    listeRevueTri = lesRevues_Abonnements.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    listeRevueTri = lesRevues_Abonnements.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    listeRevueTri = lesRevues_Abonnements.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    listeRevueTri = lesRevues_Abonnements.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    listeRevueTri = lesRevues_Abonnements.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    listeRevueTri = lesRevues_Abonnements.OrderBy(o => o.Rayon).ToList();
                    break;
            }

            RemplirListeRevues_Abonnements(listeRevueTri);

        }

        #endregion


    }

}