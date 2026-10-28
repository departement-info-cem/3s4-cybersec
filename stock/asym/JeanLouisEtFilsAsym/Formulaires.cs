namespace JeanLouisEtFilsAsym;
using System.ComponentModel.DataAnnotations;


public class Formulaires
{
    
    public class NouveauMotDePasse
    {
        [Display(Name = "C'est le mot de passe de quoi?")]
        [Required]
        public string Site { get; set; }

        [Display(Name = "Le mot de passe?")]
        [DataType(DataType.Password)]
        [Required]
        [MinLength(4)]
        public string MotDePasse { get; set; }

        [Display(Name = "Confirmation du mot de passe?")]
        [DataType(DataType.Password)]
        [Required]
        [MinLength(4)]
        public string MotDePasseConfirmation { get; set; }
        
    }
    
}