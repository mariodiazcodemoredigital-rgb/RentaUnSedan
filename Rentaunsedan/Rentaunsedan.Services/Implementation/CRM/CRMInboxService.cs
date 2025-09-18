using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentaunsedan.Data.Repositories.CRM;
using Rentaunsedan.Entities.Entities.CRM;
using Rentaunsedan.Services.Interfaces.CRM;

namespace Rentaunsedan.Services.Implementation.CRM
{
    public class CRMInboxService : ICRMInboxService
    {
        private readonly CRMInboxRepository _crminboxRepository;

        public CRMInboxService(CRMInboxRepository crminboxRepository)
        {
            _crminboxRepository = crminboxRepository;
        }

        // Propaga el evento a la UI (para que los componentes se suscriban a Inbox.Changed)
        public event Action? Changed
        {
            add { _crminboxRepository.Changed += value; }
            remove { _crminboxRepository.Changed -= value; }
        }

        public async Task<(int todos, int mios, int sinAsignar, int equipo)> GetCountsAsync(CancellationToken ct = default)
            => await _crminboxRepository.GetCountsAsync(ct);

        public async Task<IReadOnlyList<ChatThread>> GetThreadsAsync(InboxFilter filter, string search = null, string companyId = null, CancellationToken ct = default)
        {
            var q = await _crminboxRepository.GetThreadsAsync(filter, search, ct);

            if (!string.IsNullOrWhiteSpace(companyId))
                q = q.Where(t => string.Equals(t.CompanyId, companyId, System.StringComparison.OrdinalIgnoreCase));

            return q.ToList();
        }

        public  Task<ChatThread?> GetThreadAsync(string id, CancellationToken ct = default)
           => _crminboxRepository.GetThreadAsync(id, ct);

        public Task<bool> AssignAsync(string threadId, string agentUser, CancellationToken ct = default)
            => _crminboxRepository.AssignAsync(threadId, agentUser, ct);

        public Task<bool> AppendMessageAsync(
            string threadId, SenderKind kind, string sender, string text, CancellationToken ct = default)
            => _crminboxRepository.AppendMessageAsync(threadId, kind, sender, text, ct);

        public Task MarkReadAsync(string threadId, CancellationToken ct = default)
            => _crminboxRepository.MarkReadAsync(threadId, ct);

        public Task UpsertThreadAsync(string threadId,string businessPhoneId,string customerPhone,DateTime lastMessageAtUtc,string? customerName = null,string? companyId = null,string? lastPreview = null,bool incrementUnread = false,CancellationToken ct = default)
            => _crminboxRepository.UpsertThreadAsync(
              threadId, businessPhoneId, customerPhone, lastMessageAtUtc,
              customerName, companyId, lastPreview, incrementUnread, ct);
    }
}
