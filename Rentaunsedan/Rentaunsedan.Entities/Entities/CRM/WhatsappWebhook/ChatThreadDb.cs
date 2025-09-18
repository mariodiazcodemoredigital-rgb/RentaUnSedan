using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentaunsedan.Entities.Entities.CRM.WhatsappWebhook
{
    public class ChatThreadDb
    {
        [Key]
        public string ThreadId { get; set; } = default!;
        public string BusinessPhoneId { get; set; } = default!;
        public string CustomerWaNumber { get; set; } = default!;
        public string? CustomerName { get; set; }
        public string? CompanyId { get; set; }
        public string? AssignedTo { get; set; }
        public int UnreadCount { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public string? LastMessagePreview { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ChatMessageDb> Messages { get; set; } = new List<ChatMessageDb>();
    }
}
