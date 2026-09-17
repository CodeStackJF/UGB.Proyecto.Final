using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGB.Proyecto.Final.Entities;

namespace UGB.Proyecto.Final.Interfaces
{
    public interface IUsersRepository
    {
        public Task<IEnumerable<users>> GetAll();
        public Task<users> Get(int id);
        public Task<users> GetByEmail(string email);
        public Task<users> Insert(users user);
        public Task<bool> Delete(int id);
        public Task<bool> EmailExists(string email);
        public Task<bool> Update(int id, users user);
        public Task<bool> UpdatePassword(int id, string password, string salt);
        public Task<bool> Authenticate(string email, string password, string salt);
        public Task<bool> UpdatePicture(int userId, string fileName, string fileNameHash);
    }
}