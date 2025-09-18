using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentaunsedan.Entities.Entities.CRM.TestPayload
{
    public record TestInboundMessage(
        string BusinessPhoneId,   // Id del número de WhatsApp de la empresa (dummy en pruebas)
        string From,              // Número del cliente (ej. +5215512345678)
        string Text,              // Mensaje
        long? Timestamp = null   // Unix epoch seconds opcional
    );
}
