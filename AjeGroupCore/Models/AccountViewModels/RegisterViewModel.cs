using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AjeGroupCore.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} debe ser mínimo de {2} y máximo de {1} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Clave")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar clave")]
        [Compare("Password", ErrorMessage = "La Clave y la Confirmación de la clave no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
