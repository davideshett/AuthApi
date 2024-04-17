using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    public class LatestActivityController: BaseController
    {
        private readonly DataContext dataContext;

        public LatestActivityController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetLatestActivities()
        {
            var userId = Convert.ToInt32(HttpContext.User.FindFirstValue("uid"));
            if(userId <1)
            {
                return Ok("Authentication required");
            }
            var data =  dataContext.LatestActivities
            .Where(x=> x.UserId == userId)
            .OrderByDescending(x=> x.Id)
            .Skip(0).Take(5);

            return Ok(data);
        }
    }
}