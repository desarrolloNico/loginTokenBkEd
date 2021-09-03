using Microsoft.EntityFrameworkCore;

namespace contactos.Models
{
    public class Context: DbContext
    {
        public Context(DbContextOptions<Context> options)
            :base(options)
            {

            }
        
        public DbSet<Contacto> Contactos {get;set;}
        public DbSet<Usuario> Usuarios {get;set;}
    }
}