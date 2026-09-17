using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UGB.Proyecto.FinalDTO.UsersDTO
{
    public class UpdateUserPasswordDTO
    {
        public string oldPassword { get; set; } = string.Empty;
        public string newPassword { get; set; } = string.Empty;
        public string newPasswordConfirm { get; set; } = string.Empty;
    }
}