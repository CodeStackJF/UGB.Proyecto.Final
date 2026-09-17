using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UGB.Proyecto.Final.Helper
{
    public static class StringHelper
    {
        //funcion para remover caracteres especiales y letras tildadas
        public static string RemoveSpecialChars(string str)
        {
            return Regex.Replace(str, @"[^a-zA-Z0-9 ]", ""); 
        }
    }
}