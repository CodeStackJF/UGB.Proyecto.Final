using FluentValidation;
using UGB.Proyecto.Final.DTO.UsersDTO;
using UGB.Proyecto.Final.Helper;

namespace UGB.Proyecto.Final.Validations.UsersValidation
{
    public class LoginUserDTOValidation : AbstractValidator<LoginUserDTO>
    {
        public LoginUserDTOValidation()
        {
            RuleFor(x => x.email)
                .NotNullOrWhiteSpace().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");

            RuleFor(x => x.password)
                .NotNullOrWhiteSpace().WithMessage("La contraseña es obligatoria.");
        }
    }
}