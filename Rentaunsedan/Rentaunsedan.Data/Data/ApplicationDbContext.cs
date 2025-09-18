using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rentaunsedan.Entities.Entities.CRM;
using Rentaunsedan.Entities.Entities.CRM.WhatsappWebhook;

namespace Rentaunsedan.Data.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CRMEquipo> CRMEquipo => Set<CRMEquipo>();
        public DbSet<CRMUsuario> CRMUsuario => Set<CRMUsuario>();
        public DbSet<CRMEquipoUsuario> CRMEquipoUsuario => Set<CRMEquipoUsuario>();
        
    }


}
