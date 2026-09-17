using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGB.Proyecto.Final.Entities;

namespace UGB.Proyecto.Final.Interfaces
{
    public interface IRolesRepository
    {
        public Task<IEnumerable<roles>> GetAll();
    }
}