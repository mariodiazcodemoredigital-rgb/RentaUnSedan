using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Rentaunsedan.Data
{
    public class SpanishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
            => new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"El nombre de usuario '{userName}' ya está en uso."
            };

        public override IdentityError DuplicateEmail(string email)
            => new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"El correo '{email}' ya está registrado."
            };

        public override IdentityError InvalidUserName(string userName)
            => new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = $"El nombre de usuario '{userName}' no es válido."
            };

        public override IdentityError InvalidEmail(string email)
            => new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = $"El correo '{email}' no es válido."
            };

        public override IdentityError PasswordTooShort(int length)
            => new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"La contraseña es demasiado corta. Longitud mínima: {length}."
            };

        public override IdentityError PasswordRequiresDigit()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "La contraseña debe contener al menos un dígito (0-9)."
            };

        public override IdentityError PasswordRequiresLower()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "La contraseña debe contener al menos una letra minúscula."
            };

        public override IdentityError PasswordRequiresUpper()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = "La contraseña debe contener al menos una letra mayúscula."
            };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "La contraseña debe contener al menos un carácter no alfanumérico."
            };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = $"La contraseña debe contener al menos {uniqueChars} caracteres únicos."
            };

        public override IdentityError UserAlreadyInRole(string role)
            => new IdentityError
            {
                Code = nameof(UserAlreadyInRole),
                Description = $"El usuario ya pertenece al rol '{role}'."
            };

        public override IdentityError UserNotInRole(string role)
            => new IdentityError
            {
                Code = nameof(UserNotInRole),
                Description = $"El usuario no pertenece al rol '{role}'."
            };

        public override IdentityError InvalidRoleName(string role)
            => new IdentityError
            {
                Code = nameof(InvalidRoleName),
                Description = $"El nombre de rol '{role}' no es válido."
            };

        public override IdentityError DuplicateRoleName(string role)
            => new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = $"El nombre de rol '{role}' ya existe."
            };
    }
}
