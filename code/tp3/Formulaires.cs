using System.ComponentModel.DataAnnotations;

namespace consoleApp;

public class Formulaires
{
    
    public class FormulaireNouveauCompte
    {
        [Display(Name = "Votre nom d'utilisateur?")]
        [Required]
        public string Nom { get; set; }

        [Display(Name = "Votre mot de passe?")]
        [DataType(DataType.Password)]
        [Required]
        [MinLength(4)]
        public string MotDePasse { get; set; }

        [Display(Name = "Votre mot de passe (confirmation)?")]
        [DataType(DataType.Password)]
        [Required]
        [MinLength(4)]
        public string MotDePasseConfirmation { get; set; }

        [Display(Name = "Votre numero d'assurance sociale (NAS)?")]
        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        public string NAS { get; set; }
    }
    
    public class FormulaireConnexion
    {
        [Display(Name = "Votre nom d'utilisateur?")]
        [Required]
        public string Nom { get; set; }

        [Display(Name = "Votre mot de passe?")]
        [DataType(DataType.Password)]
        [Required]
        [MinLength(4)]
        public string MotDePasse { get; set; }
    }
}