using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rentaunsedan.Entities.Entities.CRM;

namespace Rentaunsedan.Services.Interfaces.CRM
{
    public interface ICRMInboxService
    {
        Task<(int todos, int mios, int sinAsignar, int equipo)> GetCountsAsync(CancellationToken ct = default);        
        Task<IReadOnlyList<ChatThread>> GetThreadsAsync(InboxFilter filter, string search = null, string companyId = null, CancellationToken ct = default);
        Task<ChatThread?> GetThreadAsync(string id, CancellationToken ct = default);
        Task<bool> AssignAsync(string threadId, string agentUser, CancellationToken ct = default);
        Task<bool> AppendMessageAsync(string threadId, SenderKind kind, string sender, string text, CancellationToken ct = default);
        Task MarkReadAsync(string threadId, CancellationToken ct = default);
        Task UpsertThreadAsync(string threadId,string businessPhoneId,string customerPhone,DateTime lastMessageAtUtc,string? customerName = null,string? companyId = null,string? lastPreview = null,bool incrementUnread = false,CancellationToken ct = default);

    }
}
