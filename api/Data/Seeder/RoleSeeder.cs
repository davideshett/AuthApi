using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace api.Data.Seeder
{
    public class RoleSeeder : IEntityTypeConfiguration<IdentityRole>
    {
        public async void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            var RoleData = await File.ReadAllTextAsync("Data/Seeder/RoleTableSeeder.json");
            var Roles = JsonConvert.DeserializeObject<List<IdentityRole>>(RoleData);

            foreach(var role in Roles)
            {
                builder.HasData(new IdentityRole{
                    Name = role.Name,
                    NormalizedName = role.NormalizedName
                });
            }
        }
    }
}