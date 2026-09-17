using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGB.Proyecto.Final.DTO.RolesUsersDTO;
using UGB.Proyecto.Final.Entities;

namespace UGB.Proyecto.FinalDTO.UsersDTO
{
    //Listar o leer datos
    public class UserDTO
    {
        public int id { get; set; }
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public IEnumerable<RolesUsersDTO> roles { get; set; } = new List<RolesUsersDTO>();
    }
}