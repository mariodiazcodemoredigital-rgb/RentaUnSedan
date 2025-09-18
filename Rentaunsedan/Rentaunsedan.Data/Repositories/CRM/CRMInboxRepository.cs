using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rentaunsedan.Data.Data;
using Rentaunsedan.Entities.Entities.CRM;
using Rentaunsedan.Entities.Entities.CRM.WhatsappWebhook;


namespace Rentaunsedan.Data.Repositories.CRM
{
    public class CRMInboxRepository
    {
        private readonly IDbContextFactory<CrmInboxDbContext> _dbFactory;        
        public CRMInboxRepository(IDbContextFactory<CrmInboxDbContext> dbFactory)
        {
            _dbFactory = dbFactory;           
        }
        

        public string CurrentUser { get; } = "you"; // simula el agente actual
        // simula "mi equipo"
        public HashSet<string> TeamUsers { get; } = new(StringComparer.OrdinalIgnoreCase)
        { "you", "cpalacios", "jruiz" };


        private readonly object _gate = new();

        // ✅ evento para notificar cambios
        public event Action? Changed;

        private void NotifyChanged() => Changed?.Invoke();

        // ----------------- Lecturas -----------------
        public async Task<(int todos, int mios, int sinAsignar, int equipo)> GetCountsAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var todos = await db.ChatThreads.CountAsync(ct);
            var mios = await db.ChatThreads.CountAsync(t => t.AssignedTo == CurrentUser, ct);
            var sin = await db.ChatThreads.CountAsync(t => t.AssignedTo == null || t.AssignedTo == "", ct);
            var eq = await db.ChatThreads.CountAsync(t => t.AssignedTo != null && TeamUsers.Contains(t.AssignedTo), ct);
            return (todos, mios, sin, eq);
        }

       
        public async Task<IEnumerable<ChatThread>> GetThreadsAsync(InboxFilter filter, string? search = null, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            IQueryable<ChatThreadDb> q = db.ChatThreads.AsNoTracking();

            q = filter switch
            {
                InboxFilter.Mios => q.Where(t => t.AssignedTo == CurrentUser),
                InboxFilter.SinAsignar => q.Where(t => t.AssignedTo == null || t.AssignedTo == ""),
                InboxFilter.Equipo => q.Where(t => t.AssignedTo != null && TeamUsers.Contains(t.AssignedTo)),
                _ => q
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(t =>
                    (t.CustomerName ?? "").ToLower().Contains(s) ||
                    (t.CustomerWaNumber ?? "").ToLower().Contains(s) ||
                    (t.LastMessagePreview ?? "").ToLower().Contains(s));
            }

            var rows = await q
                .OrderByDescending(t => t.LastMessageAt ?? t.CreatedAt)
                .ToListAsync(ct);

            // Map a tus modelos de UI
            return rows.Select(t => new ChatThread
            {
                Id = t.ThreadId,
                CustomerName = t.CustomerName ?? t.CustomerWaNumber,
                CustomerPhone = t.CustomerWaNumber,
                AssignedTo = t.AssignedTo ?? "",
                UnreadCount = t.UnreadCount,
                CompanyId = t.CompanyId ?? "DATA",
                Channel = "WhatsApp",
                Tags = new List<string> { "WhatsApp" },
                LastUpdated = (t.LastMessageAt ?? t.CreatedAt),
                Messages = string.IsNullOrWhiteSpace(t.LastMessagePreview)
                                ? new List<ChatMessage>()
                                : new List<ChatMessage> {
                                    new ChatMessage {
                                        Kind = SenderKind.Customer,
                                        Sender = t.CustomerWaNumber,
                                        Text = t.LastMessagePreview!,
                                        Timestamp = (t.LastMessageAt ?? t.CreatedAt)
                                    }
                                  }
            });
        }

     
        public async Task<ChatThread?> GetThreadAsync(string id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var t = await db.ChatThreads
                .AsNoTracking()
                .Include(x => x.Messages)
                .FirstOrDefaultAsync(x => x.ThreadId == id, ct);

            if (t is null) return null;

            var msgs = t.Messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => new ChatMessage
                {
                    Kind = m.DirectionIn ? SenderKind.Customer : SenderKind.Agent,
                    Sender = m.Sender,
                    Text = m.Text ?? "",
                    Timestamp = m.CreatedAt
                }).ToList();

            return new ChatThread
            {
                Id = t.ThreadId,
                CustomerName = t.CustomerName ?? t.CustomerWaNumber,
                CustomerPhone = t.CustomerWaNumber,
                AssignedTo = t.AssignedTo ?? "",
                UnreadCount = t.UnreadCount,
                CompanyId = t.CompanyId ?? "DATA",
                Channel = "WhatsApp",
                Tags = new List<string> { "WhatsApp" },
                LastUpdated = (t.LastMessageAt ?? t.CreatedAt),
                Messages = msgs
            };
        }

        public async Task<bool> AssignAsync(string threadId, string agentUser, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var t = await db.ChatThreads.FirstOrDefaultAsync(x => x.ThreadId == threadId, ct);
            if (t is null) return false;

            t.AssignedTo = agentUser ?? "";
            t.LastMessageAt ??= DateTime.UtcNow; // “tocado”
            await db.SaveChangesAsync(ct);
            NotifyChanged();
            return true;
        }

     

        public async Task<bool> AppendMessageAsync(string threadId, SenderKind kind, string sender, string text, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var t = await db.ChatThreads.FirstOrDefaultAsync(x => x.ThreadId == threadId, ct);
            if (t is null)
            {
                // crea hilo “de emergencia” si no existía
                t = new ChatThreadDb
                {
                    ThreadId = threadId,
                    BusinessPhoneId = "default",
                    CustomerWaNumber = sender,
                    CustomerName = sender,
                    CompanyId = "DATA",
                    CreatedAt = DateTime.UtcNow,
                    LastMessageAt = DateTime.UtcNow,
                    LastMessagePreview = text,
                    UnreadCount = (kind == SenderKind.Customer) ? 1 : 0,
                    Status = 0
                };
                db.ChatThreads.Add(t);
            }

            var msg = new ChatMessageDb
            {
                ThreadId = threadId,
                DirectionIn = (kind == SenderKind.Customer),
                Sender = sender,
                Text = text,
                CreatedAt = DateTime.UtcNow
            };
            db.ChatMessages.Add(msg);

            t.LastMessageAt = msg.CreatedAt;
            t.LastMessagePreview = text;
            if (kind == SenderKind.Customer) t.UnreadCount++;

            await db.SaveChangesAsync(ct);
            NotifyChanged();
            return true;
        }

        public async Task MarkReadAsync(string threadId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var t = await db.ChatThreads.FirstOrDefaultAsync(x => x.ThreadId == threadId, ct);
            if (t is null) return;
            t.UnreadCount = 0;
            await db.SaveChangesAsync(ct);
            NotifyChanged();
        }
    

        public async Task UpsertThreadAsync(
           string threadId,
           string businessPhoneId,
           string customerPhone,
           DateTime lastMessageAtUtc,
           string? customerName = null,
           string? companyId = null,
           string? lastPreview = null,
           bool incrementUnread = false,
           CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var t = await db.ChatThreads.FirstOrDefaultAsync(x => x.ThreadId == threadId, ct);
            if (t is null)
            {
                t = new ChatThreadDb
                {
                    ThreadId = threadId,
                    BusinessPhoneId = string.IsNullOrWhiteSpace(businessPhoneId) ? "default" : businessPhoneId,
                    CustomerWaNumber = customerPhone,
                    CustomerName = string.IsNullOrWhiteSpace(customerName) ? customerPhone : customerName,
                    CompanyId = string.IsNullOrWhiteSpace(companyId) ? "DATA" : companyId,
                    CreatedAt = DateTime.UtcNow,
                    LastMessageAt = lastMessageAtUtc,
                    LastMessagePreview = lastPreview,
                    UnreadCount = incrementUnread ? 1 : 0,
                    Status = 0
                };
                db.ChatThreads.Add(t);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(customerName)) t.CustomerName = customerName;
                if (!string.IsNullOrWhiteSpace(companyId)) t.CompanyId = companyId;
                if (lastMessageAtUtc > (t.LastMessageAt ?? DateTime.MinValue))
                    t.LastMessageAt = lastMessageAtUtc;

                // NO sumamos aquí; AppendMessageAsync se encarga del UnreadCount
                if (!string.IsNullOrWhiteSpace(lastPreview))
                    t.LastMessagePreview = lastPreview;
            }

            await db.SaveChangesAsync(ct);
            NotifyChanged();
        }

    }
}
