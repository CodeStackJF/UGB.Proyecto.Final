using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UGB.Proyecto.Final.Entities;
using UGB.Proyecto.Final.Interfaces;

namespace UGB.Proyecto.Final.Repositories
{
    public class RolesRepository(StoreCTX ctx) : IRolesRepository
    {
        public async Task<IEnumerable<roles>> GetAll()
        {
            return await ctx.roles.ToListAsync();
        }
    }
}