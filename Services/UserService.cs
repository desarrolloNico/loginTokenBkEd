using System;
using System.Collections.Generic;
using System.Linq;

using contactos.Models;

namespace contactos.Service
{
    public class UserService : IUserService
    {
        private readonly Context _context;

        public UserService(Context context)
        {
            this._context = context;
        }
        public User Authenticate(string username, string password)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = _context.Users.SingleOrDefault(x => x.username == username);

            if(user == null)
            {
                return null;
            }

            if(VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return user;
            }
            return null;
        }

        public User Create(User user, string password)
        {
            if(string.IsNullOrEmpty(password)) 
                throw new ArgumentException("El password es requerido");
            
            if(_context.Users.Any(x => x.username == user.username))
                throw new ArgumentException("Username: "+ user.username + " ya existe");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return user;
        }

        public void Delete(string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetByUserName(string username)
        {
            return _context.Users.Find(username);
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.username);

            if(user==null)
                throw new ArgumentException("Usuario no existe");
            
            user.email = userParam.email;
            user.fechaCreado = userParam.fechaCreado;

            if(!string.IsNullOrEmpty(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        //Privados
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if(string.IsNullOrWhiteSpace(password)) { throw new ArgumentException("Valor no vacio y no con espacion blanco"); }

            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if(string.IsNullOrWhiteSpace(password)) { throw new ArgumentException("Valor no vacio y no con espacion blanco", "password"); }
            if(passwordHash.Length != 64) { throw new ArgumentException("Longitud invalida para Hash, se esperaba 64 bytes", "passwordHash"); }
            if(passwordSalt.Length != 128) { throw new ArgumentException("Longitud invalida para Hash, se esperaba 128 bytes", "passwordSalt"); }

            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0 ; i < computedHash.Length ; i++)
                {
                    if(computedHash[i] != passwordHash[i]) { return false; }
                }
            }
            return true;
        }
    }
}