using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAuth.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser,ApplicationUserRole,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContext):base(dbContext)
        {

        }
    }
}
