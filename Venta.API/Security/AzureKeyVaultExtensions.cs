namespace Venta.API.Security
{
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;

    public static class AzureKeyVaultExtensions
    {
        public static IConfigurationBuilder AddAKeyVault(this IConfigurationBuilder configure, IConfiguration configuration)
        {
            var vAkvUri = new Uri($"{configuration["VaultURI"]}");
            string vTenantId = configuration["DirectoryTenantID"];
            string vApplicationClientId = configuration["ApplicationClientID"];
            string vClientSecret = configuration["MyWebApiValue"];

            var vCredential = new ClientSecretCredential(vTenantId, vApplicationClientId, vClientSecret);
            var vClient = new SecretClient(vAkvUri, vCredential);

            configure.AddAzureKeyVault(vClient, new AzureKeyVaultConfigurationOptions());

            return configure;
        }
    }
}
