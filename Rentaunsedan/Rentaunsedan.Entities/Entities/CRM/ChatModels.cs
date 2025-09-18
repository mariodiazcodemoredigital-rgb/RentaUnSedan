using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentaunsedan.Entities.Entities.CRM
{
    public enum InboxFilter { Todos, Mios, SinAsignar, Equipo }

    public enum SenderKind { Agent, Customer, System }

    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public SenderKind Kind { get; set; }
        public string Sender { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Text { get; set; } = "";
    }

    public class ChatThread
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string CustomerName { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string Channel { get; set; } = "WhatsApp"; // o Web, FB, etc
        public string CompanyId { get; set; } = "DATA";    // Empresa_ID si quieres segmentar
        public string AssignedTo { get; set; } = "";       // agente (usuario)
        public int UnreadCount { get; set; }
        public bool Closed { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public List<string> Tags { get; set; } = new();
        public List<ChatMessage> Messages { get; set; } = new();
    }

}
