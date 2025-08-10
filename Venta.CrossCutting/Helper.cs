using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Venta.CrossCutting
{
    public static class Helper
    {
        public static X509Certificate2? LoadCertificate(IConfigurationSection certificationSection)
        {
            // Prefer PFX file if provided
            var pfxPath = certificationSection["Path"];
            var pfxPassword = certificationSection["Password"];

            if (!string.IsNullOrWhiteSpace(pfxPath))
            {
                if (!File.Exists(pfxPath))
                    throw new FileNotFoundException($"PFX not found at {pfxPath}");
                return new X509Certificate2(pfxPath, pfxPassword, // secure this via env/secret store
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.EphemeralKeySet);
            }

            // Otherwise, try Windows cert store by thumbprint
            var thumb = certificationSection["Thumbprint"];
            var storeName = certificationSection["StoreName"] ?? "My";
            var storeLocation = certificationSection["StoreLocation"] ?? "CurrentUser";

            if (!string.IsNullOrWhiteSpace(thumb))
            {
                using var store = new X509Store(storeName, Enum.Parse<StoreLocation>(storeLocation));
                store.Open(OpenFlags.ReadOnly);
                var matches = store.Certificates.Find(X509FindType.FindByThumbprint, thumb, validOnly: false);
                if (matches.Count == 0)
                    throw new InvalidOperationException($"Certificate with thumbprint {thumb} not found in {storeLocation}\\{storeName}");
                return matches[0];
            }

            // No certificate configured — return null so app can still run (not recommended for prod)
            return null;
        }
    }
}
