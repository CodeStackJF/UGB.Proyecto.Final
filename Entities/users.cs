using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGB.Proyecto.Final.Entities;

namespace UGB.Proyecto.Final.Entities
{
    public class users
    {
        public int id { get; set;}
        public string first_name { get; set;} = string.Empty;
        public string last_name { get; set;} = string.Empty;
        public string email { get; set;} = string.Empty;
        public string password { get; set;} = string.Empty;
        public string salt { get; set;} = string.Empty;
        public string? user_picture { get; set;} = string.Empty;
        public string? user_picture_hash { get; set;} = string.Empty;
        public DateTime created_on { get; set;}

        public IEnumerable<users_roles> users_roles { get; set;} = new List<users_roles>();
    }
}