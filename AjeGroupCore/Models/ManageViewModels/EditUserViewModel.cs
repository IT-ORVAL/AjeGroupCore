using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AjeGroupCore.Models.ManageViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Nombres")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string LastName { get; set; }

        [Display(Name = "Cumpleaños")]
        public DateTime? Birthday { get; set; }

        [Required]
        [Display(Name = "Pregunta Secreta")]
        public string SecretQuestion { get; set; }

        [Required]
        [Display(Name = "Respuesta Secreta")]
        public string SecretResponse { get; set; }

    }
}
