using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class LatestActivity
    {
        public int Id { get; set; }
        public string Activity { get; set; }
        public string Timestamp { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
    }

    public class LatestActivityStripped 
    {
        public int Id { get; set; }
        public string Activity { get; set; }
        public string Timestamp { get; set; }
    }
}