using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using UGB.Proyecto.Final.Helper;
using UGB.Proyecto.FinalDTO.UsersDTO;

namespace UGB.Proyecto.Final.Validations.UsersValidation
{
    public class CreateUserDTOValidation : AbstractValidator<CreateUserDTO>
    {
        public CreateUserDTOValidation()
        {
           /*  RuleFor(x=>x.firstName).NotNull().WithMessage("El nombre está nulo");
            RuleFor(x=>x.firstName).NotEmpty().WithMessage("El nombre está vacío"); */
            RuleFor(x=>x.firstName).NotNullOrWhiteSpace().WithMessage("El nombre es requerido.");
            RuleFor(x=>x.lastName).NotNullOrWhiteSpace().WithMessage("El apellido es requerido.");
            RuleFor(x=>x.email).EmailAddress().WithMessage("Ingrese un correo válido.");

            RuleFor(p => p.password).NotNullOrWhiteSpace().WithMessage("La contraseña no debe estar vacía")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\.\$]+").WithMessage("Your password must contain at least one (!?$*.).");
            
        }
    }
}