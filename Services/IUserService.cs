using System.Collections.Generic;

using contactos.Models;

namespace contactos.Service
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetByUserName(string username);
        User Create(User user, string password);
        void Update(User user, string password=null);
        void Delete(string username);
    }
}