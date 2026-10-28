using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace consoleApp
{
    public class DonneesUtilisateur
    {
        public string Nom              { get; set; }
        public string MotDePasseHash   { get; set; }
        public string NAS              { get; set; }
    }

    public class DonneesAnneeRevenu
    {
        public string Nom              { get; set; }
        public int Annee              { get; set; }
        public int Revenu            { get; set; }
    }

}
