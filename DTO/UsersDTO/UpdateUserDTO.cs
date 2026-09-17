using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UGB.Proyecto.FinalDTO.UsersDTO
{
    public class UpdateUserDTO
    {
        public int id { get; set; }
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}