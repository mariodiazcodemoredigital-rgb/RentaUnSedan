using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentaunsedan.Entities.Entities.CRM.WhatsappWebhook
{
    public class ChatMessageDb
    {
        [Key]
        public long MessageId { get; set; }
        public string ThreadId { get; set; } = default!;
        public string? WaMessageId { get; set; }
        public bool DirectionIn { get; set; }   // true: cliente
        public string Sender { get; set; } = default!;
        public string? Text { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaMime { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navegación inversa (opcional pero recomendable)
        public ChatThreadDb? Thread { get; set; }
    }
}
