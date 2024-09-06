using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Authentication.Services
{
    public class CryptoSettings
    {
        public required string EncryptionKey { get; set; }
        public required string IV { get; set; }
    }

    public class CryptoSettingsValidator : IValidateOptions<CryptoSettings>
    {
        public ValidateOptionsResult Validate(string? name, CryptoSettings options)
        {
            var validationResult = new List<string>();

            if (string.IsNullOrEmpty(options.EncryptionKey))
                validationResult.Add("EncryptionKey cannot be empty");
            else
            {
                byte[]? encryptionKeyBytes = null;
                try
                {
                    encryptionKeyBytes = Convert.FromBase64String(options.EncryptionKey);
                }
                catch
                {
                    validationResult.Add("EncryptionKey must be a valid base 64 string");
                }

                if (encryptionKeyBytes != null)
                    if (encryptionKeyBytes.Length != 32)
                        validationResult.Add("EncryptionKey must be 32 bytes long");
            }

            if (string.IsNullOrEmpty(options.IV))
                validationResult.Add("IV cannot be empty");
            else
            {
                byte[]? ivBytes = null;
                try
                {
                    ivBytes = Convert.FromBase64String(options.IV);
                }
                catch
                {
                    validationResult.Add("IV must be a valid base 64 string");
                }

                if (ivBytes != null)
                    if (ivBytes.Length != 16)
                        validationResult.Add("IV must be 16 bytes long");
            }

            if (validationResult.Count > 0)
                return ValidateOptionsResult.Fail(string.Join('\n', validationResult));
            return ValidateOptionsResult.Success;
        }
    }
}
