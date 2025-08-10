# Start clean
Remove-Variable cert -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path .\secrets | Out-Null

# Create self-signed cert in CurrentUser\My using CNG Software KSP (no smart card)
$cert = New-SelfSignedCertificate `
  -Type Custom `
  -Subject "CN=VentaApi-DP" `
  -KeyAlgorithm RSA `
  -KeyLength 3072 `
  -KeyExportPolicy Exportable `
  -KeyUsage DigitalSignature,KeyEncipherment `
  -HashAlgorithm SHA256 `
  -NotAfter (Get-Date).AddYears(2) `
  -Provider "Microsoft Software Key Storage Provider" `
  -CertStoreLocation "Cert:\CurrentUser\My"

# Export to PFX
$pwd = ConvertTo-SecureString "change-me" -AsPlainText -Force
Export-PfxCertificate -Cert $cert -FilePath .\secrets\dpkeys.pfx -Password $pwd

# Show thumbprint and provider
$thumb = $cert.Thumbprint
Write-Host "Thumbprint: $thumb"
(Get-Item "Cert:\CurrentUser\My\$thumb").PublicKey.Key.SignatureAlgorithm.FriendlyName

$pwd = ConvertTo-SecureString "change-me" -AsPlainText -Force
$test = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2(".\secrets\dpkeys.pfx",$pwd)
$test.HasPrivateKey  # should be True