using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Modes;
using UGB.Proyecto.Final.CustomExceptions;
using UGB.Proyecto.Final.Entities;
using UGB.Proyecto.Final.Helper;
using UGB.Proyecto.Final.Interfaces;
using UGB.Proyecto.FinalDTO.UsersDTO;
using UGB.Proyecto.FinalMapper;

namespace UGB.Proyecto.Final.Controllers
{

    public class UsersController(
         IValidator<CreateUserDTO> createUserDTOValidator,
        IUsersRepository usersRepository,
        IUsersRolesRepository usersRolesRepository,
        IWebHostEnvironment webHostEnvironment
    ) : Controller
    {
       [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateUserDTO createUserDTO)
        {
            //validamos el objeto que recibimos en el body de la petición
            var validation = createUserDTOValidator.Validate(createUserDTO);

            //si el objeto no es valido retornamos un BadRequest con los errores de validación
            if(!validation.IsValid)
            {
                throw new CustomValidationException(validation.Errors);
            }

            //verificamos si el correo existe antes de registrar
            if(await usersRepository.EmailExists(createUserDTO.email))
            {
                throw new HttpRequestException("Este correo ya se encuentra registrado.");
            }

            //Creamos el hash de la contraseña y el salt para guardarlo en la base de datos
            HashedPassword hashedPassword = HashHelper.Hash(createUserDTO.password);

            //mapeamos el objeto CreateUserDTO a la entidad users para poder guardarlo en la base de datos
            users user = CustomMapper<users>.Map(createUserDTO);

            //asignamos la contraseña hasheada y el salt al objeto users
            user.password = hashedPassword.Password;
            user.salt = hashedPassword.Salt;

            //insertamos el nuevo usuario en la base de datos y retornamos el objeto mapeado a UserDTO
            user = await usersRepository.Insert(user);
            await usersRolesRepository.Insert(new users_roles()
            {
                user_id = user.id,
                role_id = 2 //asignamos el rol de usuario por defecto
            });
            user = await usersRepository.Get(user.id);
            return Ok(CustomMapper<UserDTO>.Map(user));
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await usersRepository.GetAll();
            return Ok(CustomMapper<UserDTO>.Map(users));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ListAll(int id)
        {
            ViewBag.Users = await usersRepository.GetAll();
            return View();
        }

        [Authorize(Roles = "Administrador,Usuario")]
        public async Task<IActionResult> ShowAuthenticated(int id)
        {
            string email = User.GetProperty(ClaimTypes.Email);
            ViewBag.User = await usersRepository.GetByEmail(email);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> uploadpicture([FromForm] IFormFile user_picture)
        {
            if(user_picture.Length == 0)
            {
                throw new HttpRequestException("El archivo no es válido.");
            }
            
            if(user_picture.Length / 1024 >= 2048)
            {
                throw new HttpRequestException("El archivo excede los 2mb.");
            }

            int userId = User.GetProperty("UserId", typeof(int));
            string fileName = userId.ToString() + "_" + StringHelper.RemoveSpecialChars(user_picture.FileName).Replace(' ', '_');
            string filePath = Path.Combine(webHostEnvironment.ContentRootPath, "uploadedPictures", fileName);
            using(FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await user_picture.CopyToAsync(fs);
            }
            string hash = HashHelper.GetSha256Hash(filePath);
            await usersRepository.UpdatePicture(userId, fileName, hash);
            return Redirect("/users/showauthenticated");
        }

    }
}