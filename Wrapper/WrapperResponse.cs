using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UGB.Proyecto.Final.Wrapper
{
    public class WrapperResponse
    {
        private bool Success { get; set; } = false;
        public Dictionary<string, List<string>> ValidationErrors { get; set; } = new Dictionary<string, List<string>>();
        public dynamic Data { get; set; } = new {};
        public string Message { get; set; } = "";
        public int StatusCode { get; set; }
    }
}