using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UGB.Proyecto.Final.Entities
{
    public class otp
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string otp_code { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public bool verified { get; set; }
        public string request_id { get; set; } = string.Empty;
    }
}