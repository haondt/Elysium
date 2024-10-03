using Microsoft.AspNetCore.Identity;

namespace Elysium.Authentication.Exceptions
{
    public class IdentityErrorsException(IEnumerable<IdentityError> errors) : Exception(StringifyErrors(errors))
    {
        private static string StringifyErrors(IEnumerable<IdentityError> errors)
        {
            var stringifiedErrors = errors.Select(e => $"[{e.Code}] {e.Description}");
            return string.Join(";", stringifiedErrors);
        }
    }
}
