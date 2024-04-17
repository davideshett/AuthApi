using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Response
{
    public class GenericResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccessful { get; set; }
    }
}