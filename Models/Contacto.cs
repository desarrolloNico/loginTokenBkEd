using System;

namespace contactos.Models
{
    public class Contacto
    {
        public int Id {get;set;}
        public string nombre {get;set;}
        public string email {get;set;}
        public DateTime fechaNacimiento {get;set;}
    }
}