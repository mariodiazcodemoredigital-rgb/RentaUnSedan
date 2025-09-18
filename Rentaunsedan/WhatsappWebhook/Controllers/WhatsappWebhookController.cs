using Microsoft.AspNetCore.Mvc;
using Rentaunsedan.Services.Implementation.CRM;

namespace WhatsappWebhook.Controllers
{
    [ApiController]
    [Route("api/webhooks/whatsapp")]
    public class WhatsappWebhookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly CRMInboxService _inbox;
        public WhatsappWebhookController(CRMInboxService inbox) => _inbox = inbox;

        // Verificación (si la llegas a necesitar)
        [HttpGet]
        public IActionResult Verify([FromQuery(Name = "hub.mode")] string? mode,
                                    [FromQuery(Name = "hub.challenge")] string? challenge,
                                    [FromQuery(Name = "hub.verify_token")] string? token)
        {
            if (mode == "subscribe" && token == "TU_VERIFY_TOKEN")
                return Content(challenge ?? "");
            return Unauthorized();
        }

        // Endpoint de PRUEBA (Postman)
        public record TestInboundMessage(string BusinessPhoneId, string From, string Text, long? Timestamp = null);

        [HttpPost("test")]
        public async Task<IActionResult> Test([FromBody] TestInboundMessage dto, CancellationToken ct)
        {
            var createdUtc = dto.Timestamp is long ts
                ? DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime
                : DateTime.UtcNow;

            var threadId = $"{dto.BusinessPhoneId}:{dto.From}";

            await _inbox.UpsertThreadAsync(threadId, dto.BusinessPhoneId, dto.From, createdUtc,
                                           customerName: dto.From, companyId: "DATA",
                                           lastPreview: dto.Text, incrementUnread: false, ct);

            await _inbox.AppendMessageAsync(threadId, Rentaunsedan.Entities.Entities.CRM.SenderKind.Customer,
                                            dto.From, dto.Text, ct);

            return Ok(new { ok = true });
        }

    }
}
