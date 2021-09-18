using System;
using System.ComponentModel.DataAnnotations;

namespace contactos.Models
{
    public class UserDto
    {
        [Key]
        [Required]
        [StringLength(20, ErrorMessage = "El valor de {0}, debe contener al menos {2} y como maximo {1}", MinimumLength = 5)]
        public string username {get;set;}

        public string password {get;set;}

        public string email {get;set;}
        
        [DataType(DataType.DateTime)]
        public DateTime fechaCreado {get;set;}
    }
}