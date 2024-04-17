using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Settings
{
    public class MailgunSettings
    {
        public string API_KEY { get; set; }
        public string DOMAIN { get; set; }
        public string FROM { get; set; }
    }
}