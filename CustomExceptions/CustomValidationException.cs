using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace UGB.Proyecto.Final.CustomExceptions
{
    public class CustomValidationException : Exception
    {
        /// <summary>
        /// Excepción personalizada para las validaciones
        /// </summary>
        public Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();

        public CustomValidationException() : base("Se han producido errores de validación.")
        { 

        }

        public CustomValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            var registers = failures.GroupBy(x=>x.PropertyName).Select(x=>new {
                PropertyName = x.Key,
                Errors = x.Select(y=>y.ErrorMessage).ToList()
            });

            foreach(var register in registers)
            {
                Errors.Add(register.PropertyName, register.Errors);
            }
        }

        public CustomValidationException(string property, string message)
        {
            Errors.Add(property, new List<string>(){message});
        }
    }
}