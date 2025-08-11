# ProyectoDS - Revisión de Seguridad y Análisis de Amenazas

## 🎯 Objetivo de la Revisión
Este documento proporciona un análisis comprehensivo de seguridad para SecureShop, siguiendo metodologías empresariales usadas por equipos de seguridad de organizaciones Fortune 500.

## 📊 Metodología STRIDE - Análisis Detallado

### 🎭 S - Spoofing (Suplantación de Identidad)

#### **Vectores de Amenaza**
- Robo de credenciales de usuario
- Ataques de ingeniería social
- Intercepción de tokens de sesión
- Falsificación de identidad de aplicación

#### **Controles Implementados**
1. **Azure AD con MFA Obligatorio** (no forma parte del alcance del proyecto)
   - Requiere factor adicional más allá de contraseña
   - Resistente a ataques de credential stuffing
   - Políticas de acceso condicional basadas en riesgo

2. **OAuth 2.0 + OpenID Connect**
   - Tokens JWT con firma criptográfica
   - Expiración corta (1 hora) para limitar ventana de ataque
   - Refresh tokens seguros con rotación automática

3. **Validación de Audiencia de Tokens**
   - Verificación que tokens son para nuestra aplicación específicamente
   - Previene ataques de replay entre aplicaciones

#### **Casos de Estudio Preventivos**
- **Twitter Hack (2020)**: Atacantes usaron credenciales robadas. Nuestro MFA habría bloqueado el acceso.
- **LastPass (2022)**: Vault master password comprometida. Azure AD elimina dependencia de contraseñas únicas.

---

### 🔧 T - Tampering (Manipulación de Datos)

#### **Vectores de Amenaza**
- Modificación de datos en tránsito (Man-in-the-Middle)
- Alteración de datos en base de datos
- Manipulación de tokens de autenticación
- Modificación de configuración de aplicación

#### **Controles Implementados**
1. **HTTPS Obligatorio con Certificate Pinning**
   - TLS 1.3 para toda comunicación
   - Perfect Forward Secrecy (PFS)
   - HTTP Strict Transport Security (HSTS)

2. **Integridad de Datos con Checksums**
   - Hash SHA-256 para verificación de integridad
   - Firmas digitales para transacciones críticas
   - Timestamps para prevenir ataques de replay

3. **Transparent Data Encryption (TDE)**
   - Cifrado automático en Azure SQL Database
   - Keys gestionadas por Azure Key Vault
   - Protección contra acceso físico a discos

#### **Validación Continua**
```sql
-- Trigger para detectar modificaciones no autorizadas
CREATE TRIGGER tr_ProductTamperDetection
ON Producto AFTER UPDATE
AS
BEGIN
    INSERT INTO AuditLog (
        EntityName, Id, UserEmail, 
        [Changes], Timestamp
    )
    SELECT 'Producto', i.Id, SYSTEM_USER,
           'Old Values: ' + (SELECT * FROM deleted d WHERE d.Id = i.Id FOR JSON AUTO) + 
		   'New Values: ' + (SELECT * FROM i FOR JSON AUTO),
           GETDATE()
    FROM inserted i
END
```

---

### 🚫 R - Repudiation (Repudio/Negación)

#### **Vectores de Amenaza**
- Usuarios negando acciones realizadas
- Falta de trazabilidad en transacciones críticas
- Logs manipulados o eliminados
- Ausencia de evidencia forense

#### **Controles Implementados**
1. **Auditoría Forense Completa**
   - Registro de cada acción con contexto completo
   - Timestamps inmutables a nivel de base de datos
   - Correlación con sesiones de usuario y direcciones IP

2. **Application Insights Integration**
   - Telemetría centralizada con retención de 730 días
   - Correlación automática entre logs de aplicación y Azure AD
   - Exportación automática a sistemas SIEM

3. **Digital Signatures para Transacciones**
   - Certificados x.509 almacenados en Key Vault
   - Firma digital de documentos y transacciones críticas
   - Verificación criptográfica de integridad

#### **Implementación de Auditoría**
```csharp
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                || e.State == EntityState.Modified
                || e.State == EntityState.Deleted)
                .ToList();
            foreach (var modifiedEntity in modifiedEntities)
            {
                var auditLog = new AuditLog
                {
                    EntityName = modifiedEntity.Entity.GetType().Name,
                    Action = modifiedEntity.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Changes = GetChanges(modifiedEntity)
                };
                AuditLogs.Add(auditLog);
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        private static string GetChanges(EntityEntry entity)
        {
            var changes = new StringBuilder();
            foreach (var property in entity.OriginalValues.Properties)
            {
                var originalValue = entity.OriginalValues[property];
                var currentValue = entity.CurrentValues[property];
                if (!Equals(originalValue, currentValue))
                {
                    changes.AppendLine($"{property.Name}: From '{originalValue}' to '{currentValue}'");
                }
            }
            return changes.ToString();
        }
}
```

---

### 🔍 I - Information Disclosure (Divulgación de Información)

#### **Vectores de Amenaza**
- Exposición accidental de secretos en logs
- Acceso no autorizado a datos sensibles
- Filtración a través de mensajes de error detallados
- Exposición de PII en comunicaciones no cifradas

#### **Controles Implementados**
1. **Clasificación y Cifrado de Datos**
   - **Públicos**: Sin cifrado adicional (nombres de productos)
   - **Internos**: Cifrados con AES-256 (precios de costo)
   - **Confidenciales**: Cifrados + access logging (datos de clientes)
   - **Restringidos**: Double encryption + approval workflow

2. **Azure Key Vault para Secretos**
   - Separación completa entre código y credenciales
   - HSM-backed key storage
   - Rotación automática de secrets
   - RBAC granular con principio de menor privilegio

3. **Safe Error Handling**
   - Mensajes de error genéricos para usuarios finales
   - Detalles técnicos solo en logs internos seguros
   - No exposición de stack traces en producción

---

### 💥 D - Denial of Service (Denegación de Servicio)

#### **Vectores de Amenaza**
- Ataques volumétricos (DDoS)
- Resource exhaustion attacks
- Application-layer attacks (SlowLoris, etc.)
- Database connection pool exhaustion

#### **Controles Implementados**
1. **Azure DDoS Protection Standard**
   - Mitigación automática de ataques volumétricos
   - ML-based traffic analysis
   - Always-on monitoring con alertas automáticas

2. **Application-Level Rate Limiting**
   - Throttling por IP, usuario, y endpoint
   - Circuit breaker pattern para servicios externos
   - Request size limits para prevenir memory exhaustion

3. **Optimización de Performance**
   - Connection pooling optimizado
   - Caching estratégico con Redis
   - CDN para contenido estático

#### **Configuración de Rate Limiting**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("AuthPolicy", configure =>
    {
        configure.PermitLimit = 5;
        configure.Window = TimeSpan.FromMinutes(1);
        configure.QueueLimit = 0;
    });

    options.AddSlidingWindowLimiter("ApiPolicy", configure =>
    {
        configure.PermitLimit = 100;
        configure.Window = TimeSpan.FromMinutes(1);
        configure.SegmentsPerWindow = 4;
    });
});
```

---

### ⬆️ E - Elevation of Privilege (Escalación de Privilegios) - (No forma parte del proyecto)

#### **Vectores de Amenaza**
- Explotación de vulnerabilidades para obtener permisos administrativos
- Bypass de controles de autorización
- Abuso de funcionalidades legítimas para acceso no autorizado
- Privilege creep a través del tiempo

#### **Controles Implementados**
1. **Role-Based Access Control (RBAC) Granular**
   - Principio de menor privilegio aplicado rigurosamente
   - Revisión automática de permisos cada 90 días
   - Approval workflow para cambios de roles críticos

2. **Claims-Based Authorization**
   - Autorización contextual que considera el recurso específico
   - Políticas dinámicas basadas en atributos de usuario y contexto
   - Verificación en tiempo real de permisos

3. **Separation of Duties**
   - Ningún usuario individual puede completar transacciones críticas solo
   - Approval de múltiples personas para operaciones sensibles
   - Rotación obligatoria de roles administrativos


## 🎯 Matriz de Riesgo y Mitigación

| Riesgo | Probabilidad | Impacto | Riesgo Total | Estado de Mitigación |
|--------|--------------|---------|--------------|---------------------|
| Credential Theft | Alto | Alto | 🔴 Crítico | ✅ Mitigado (MFA + Azure AD) |
| Data Breach | Medio | Alto | 🟡 Alto | ✅ Mitigado (Encryption + Key Vault) |
| DDoS Attack | Alto | Medio | 🟡 Alto | ✅ Mitigado (Azure DDoS + Rate Limiting) |
| Insider Threat | Bajo | Alto | 🟡 Alto | ✅ Mitigado (RBAC + Auditing) |
| SQL Injection | Medio | Alto | 🟡 Alto | ✅ Mitigado (EF Core + Parameterization) |
| XSS Attack | Medio | Medio | 🟢 Medio | ✅ Mitigado (CSP + Input Validation) |

## 📋 Plan de Respuesta a Incidentes

### Clasificación de Incidentes
1. **P0 - Crítico**: Compromiso de datos de clientes, acceso no autorizado a sistemas de producción
2. **P1 - Alto**: Suspensión de servicios, vulnerabilidades de seguridad explotables
3. **P2 - Medio**: Intentos de ataque detectados, problemas de performance
4. **P3 - Bajo**: Anomalías menores, alertas de monitoreo

### Playbooks de Respuesta
- **Data Breach Response**: Procedimientos dentro de 72 horas para cumplimiento GDPR
- **Account Compromise**: Revocación automática de tokens y reset de credenciales
- **DDoS Mitigation**: Escalación automática de recursos y activación de WAF
- **Insider Threat**: Preservación de evidencia y notificación a recursos humanos

## 🔄 Revisión Continua

### Frecuencia de Auditorías
- **Diaria**: Revisión automática de logs de seguridad y alertas
- **Semanal**: Análisis de patrones de acceso y anomalías
- **Mensual**: Revisión de permisos y derechos de acceso
- **Trimestral**: Penetration testing y vulnerability assessment
- **Anual**: Revisión completa de arquitectura de seguridad

### Métricas de Monitoreo
- **MTTD (Mean Time to Detection)**: Objetivo < 5 minutos
- **MTTR (Mean Time to Response)**: Objetivo < 30 minutos  
- **Security Score**: Mantenimiento > 95% en Azure Security Center
- **Compliance Rate**: 100% en auditorías regulatorias

---

> **📌 Nota Crítica**: Esta revisión de seguridad no es un documento estático. Debe actualizarse cada vez que se agregan nuevas funcionalidades, se descubren nuevas amenazas, o cambian los requisitos de cumplimiento. La seguridad es un proceso continuo, no un estado final.