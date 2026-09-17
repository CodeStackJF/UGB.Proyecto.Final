using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UGB.Proyecto.Final.DTO.RolesDTO;
using UGB.Proyecto.Final.DTO.RolesUsersDTO;
using UGB.Proyecto.Final.Entities;
using UGB.Proyecto.FinalDTO.UsersDTO;

namespace UGB.Proyecto.FinalMapper
{
    public static class CustomMapper<TOutput> where TOutput : class
    {
        //TInput representa el tipo de datos de entrada y TOutput representa el tipo de datos de salida
        public static TOutput Map<TInput>(TInput input)
        {
            //Se valida el tipo de datos de entrada y salida para realizar el mapeo correspondiente
            //esto se debe hacer para cada tipo de datos que se quiera mapear, si se quiere mapear otro tipo de datos se debe agregar otro if con el tipo de datos correspondiente
            if(typeof(TInput) == typeof(users) && typeof(TOutput) == typeof(UserDTO))
            {
                //el inputType representa un tipo de datos generico, por lo que debe convertirse a su tipo de datos real para poder acceder a sus propiedades, esto se hace con el operador "as" y el operador "!" para indicar que no es nulo
                users inputType = (input as users)!;

                //se retorna un objeto de tipo TOutput, por lo que se debe convertir el objeto a su tipo de datos real, esto se hace con el operador "as" y el operador "!" para indicar que no es nulo
                return (new UserDTO()
                {
                    id = inputType.id,
                    firstName = inputType.first_name,
                    lastName = inputType.last_name,
                    email = inputType.email,
                    roles = inputType.users_roles.Select(x=>CustomMapper<RolesUsersDTO>.Map(x)),
                } as TOutput)!;
            }

            if(typeof(TInput) == typeof(CreateUserDTO) && typeof(TOutput) == typeof(users))
            {
                CreateUserDTO inputType = (input as CreateUserDTO)!;
                return (new users()
                {
                    first_name = inputType.firstName,
                    last_name = inputType.lastName,
                    email = inputType.email,
                    password = inputType.password,
                } as TOutput)!;
            }

            if(typeof(TInput) == typeof(roles) && typeof(TOutput) == typeof(RoleDTO))
            {
                roles inputType = (input as roles)!;
                return (new RoleDTO()
                {
                    id = inputType.id,
                    description = inputType.description,
                } as TOutput)!;
            }

            if(typeof(TInput) == typeof(users_roles) && typeof(TOutput) == typeof(RolesUsersDTO))
            {
                users_roles inputType = (input as users_roles)!;
                return (new RolesUsersDTO()
                {
                    role = CustomMapper<RoleDTO>.Map(inputType.role),
                } as TOutput)!;
            }

            throw new NotSupportedException();
        }

        //método para mapear una lista de objetos de un tipo a otro tipo
        public static IEnumerable<TOutput> Map<TInput>(IEnumerable<TInput> input)
        {
            if(typeof(TInput) == typeof(users) && typeof(TOutput) == typeof(UserDTO))
            {
                IEnumerable<users> inputType = (input as IEnumerable<users>)!;
                //hace uso de la función que mapea un solo objeto para mapear cada objeto de la lista, esto se hace con el método Select de LINQ
                return input.Select(Map);
            }

            if(typeof(TInput) == typeof(users_roles) && typeof(TOutput) == typeof(RolesUsersDTO))
            {
                IEnumerable<users_roles> inputType = (input as IEnumerable<users_roles>)!;
                //hace uso de la función que mapea un solo objeto para mapear cada objeto de la lista, esto se hace con el método Select de LINQ
                return input.Select(Map);
            }

            throw new NotSupportedException();
        }

        public static List<TOutput> Map<TInput>(List<TInput> input)
        {
            if(typeof(TInput) == typeof(users) && typeof(TOutput) == typeof(UserDTO))
            {
                IEnumerable<users> inputType = (input as IEnumerable<users>)!;
                //hace uso de la función que mapea un solo objeto para mapear cada objeto de la lista, esto se hace con el método Select de LINQ
                return input.Select(Map).ToList();
            }
            throw new NotSupportedException();
        }
    }
}