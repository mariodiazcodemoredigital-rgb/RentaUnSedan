using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rentaunsedan.Entities.Entities.CRM.WhatsappWebhook;

namespace Rentaunsedan.Data.Data
{
    public class CrmInboxDbContext : DbContext
    {
        public CrmInboxDbContext(DbContextOptions<CrmInboxDbContext> options) : base(options) { }

    
        /* ES PARA WHATSAPP WEBHOOK */
        public DbSet<ChatThreadDb> ChatThreads => Set<ChatThreadDb>();
        public DbSet<ChatMessageDb> ChatMessages => Set<ChatMessageDb>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // Thread

            mb.Entity<ChatThreadDb>(e =>
            {
                e.ToTable("ChatThreads");               // o ("ChatThreads", "crm") si usas esquema
                e.HasKey(x => x.ThreadId);
                e.Property(x => x.ThreadId).HasMaxLength(120);

                e.HasMany(x => x.Messages)
                 .WithOne(x => x.Thread!)
                 .HasForeignKey(x => x.ThreadId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => x.LastMessageAt);
            });

            // Message
            mb.Entity<ChatMessageDb>(e =>
            {
                e.ToTable("ChatMessages");
                e.HasKey(x => x.MessageId);
                e.Property(x => x.ThreadId).HasMaxLength(120).IsRequired();
                e.HasIndex(x => new { x.ThreadId, x.CreatedAt });
            });
        }
    }
}
