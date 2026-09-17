using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UGB.Proyecto.Final.DTO.UsersDTO;
using UGB.Proyecto.Final.Helper;
using UGB.Proyecto.Final.Interfaces;
using UGB.Proyecto.Final.Entities;
using UGB.Proyecto.Final.CustomExceptions;

namespace UGB.Proyecto.Final.Controllers 
{
    public class LoginController(
        IUsersRepository usersRepository,
        IValidator<LoginUserDTO> loginUserDTOValidator
    ) : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(User!.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Esta acción es pública, no requiere autenticación para acceder a ella
        [AllowAnonymous]
        public async Task<ActionResult> Authenticate([FromBody] LoginUserDTO loginUserDTO)
        {
            //Aplicamos la validación de los datos del login
            var validation = loginUserDTOValidator.Validate(loginUserDTO);
            if(!validation.IsValid)
            {
                throw new CustomValidationException(validation.Errors);
            }

            //buscamos el usuario por su correo
            users user = await usersRepository.GetByEmail(loginUserDTO.email);
            if(user == null)
            {
                throw new HttpRequestException("usuario no encontrado.");
            }

            
            //validamos las credenciales
            bool validCredentials = HashHelper.CheckHash(loginUserDTO.password, user.password, user.salt);
            if(!validCredentials)
            {
                throw new HttpRequestException("Las credenciales no son válidas.");
            }

            //iniciamos sesión con cookies
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            
            //Almacenamos los datos en la sesión, nunca se deben guardar datos sensibles
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.email));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.email));
            identity.AddClaim(new Claim("UserId", user.id.ToString()));
            identity.AddClaim(new Claim("UserName", user.first_name + " " + user.last_name));

            //Registramos cada uno de los roles
            foreach(var role in user.users_roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.role.description));
            }

            //Definimos la fecha de expiración que es de 4 horas
            DateTime cookieExpirationDate = DateTime.Now.AddHours(4);
            var principal = new ClaimsPrincipal(identity);
            //iniciamos la sesión, la respuesta de la cabecera http automaticamente hace que el navegador guarde la cookie retornada
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties { ExpiresUtc = cookieExpirationDate, IsPersistent = true });

            return NoContent();
        }

        //Solo si está autenticado podrá hacer un logout
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        public IActionResult AccessDenied()
        {
            throw new HttpRequestException("No está autorizado para ingresar.");
        }
    }
}