using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace UGB.Proyecto.Final.Helper
{
    public static class FluentHelper
    {
        public static IRuleBuilderOptions<T, string> NotNullOrWhiteSpace<T>
        (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(x=>!string.IsNullOrWhiteSpace(x));
        }
    }
}