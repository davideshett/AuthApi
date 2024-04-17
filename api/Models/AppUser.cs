using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class AppUser: IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public string TempOtp { get; set; }
        public int LoginCount { get; set; }
        public string TokenGuid { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }

        public ICollection<LatestActivity> LatestActivities { get; set; }
    }
}