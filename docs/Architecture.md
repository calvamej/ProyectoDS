# ProyectoDS - Arquitectura de Aplicación Segura

## 🎯 Visión General
ProyectoDS es una aplicación de e-commerce para compra y venta de productos de la empresa, que implementa principios de seguridad desde el diseño (Secure-by-Design). Representa la materialización de años de evolución en mejores prácticas de seguridad en la nube.

## 🏛️ Arquitectura de Alto Nivel

### Componentes Principales:
- **Web Application (ASP.NET Core MVC)**: Frontend y API con seguridad integrada
- **Azure AD**: Gestión de identidad y autenticación empresarial  
- **Azure Key Vault**: Gestión de secretos y certificados con HSM
- **Azure SQL Database**: Almacenamiento de datos con cifrado TDE
- **Application Insights**: Monitoreo y auditoría forense completa

### Flujo de Datos Seguro:
```
Usuario → Azure AD (OAuth 2.0 + MFA) → ASP.NET Core (HTTPS + CSP) → Key Vault (Secretos) → Azure SQL (TDE)
```

**Principio Clave**: Cada comunicación entre componentes está cifrada y autenticada. Incluso si alguien captura el tráfico de red, no pueden usarlo para impersonar usuarios o acceder a datos.

## 🔒 Modelo de Seguridad

### Roles de Usuario (Principio de Menor Privilegio):

## Nota: 
Para el primer alcance solo se cuenta con el Rol Cliente.

#### **Cliente**
- **Permisos**: Gestión de perfil personal, compras, historial de pedidos
- **Restricciones**: Nunca puede ver datos de otros clientes, acceder a funciones administrativas, o ver información interna como precios de costo

#### **Supervisor**  
- **Permisos**: Gestión de catálogo de productos, reportes de ventas, administración de inventario
- **Restricciones**: No pueden acceder a datos personales de clientes, información de pagos, o funciones de administración del sistema

#### **Administrador**
- **Permisos**: Control completo del sistema con auditoría total
- **Principio**: Poder con responsabilidad - incluso administradores no pueden realizar acciones críticas sin dejar rastros auditables

### Capas de Protección (Defense in Depth):

#### **Capa 1 - Network**
- **HTTPS Forzado**: Toda comunicación cifrada usando estándares bancarios
- **WAF (Web Application Firewall)**: Filtrado automático de ataques comunes
- **CSP (Content Security Policy)**: Sistema inmunológico que rechaza código extraño

#### **Capa 2 - Identity**  
- **Azure AD con MFA**: Requiere tanto contraseña como acceso físico al teléfono
- **Single Sign-On (SSO)**: Una sola autenticación para múltiples aplicaciones
- **Gestión de identidad empresarial**: Control centralizado de accesos

#### **Capa 3 - Application**
- **Validación de entrada**: Datos maliciosos detectados antes de causar daño  
- **Protección CSRF**: Previene que sitios maliciosos engañen usuarios
- **Rate Limiting**: Protección contra ataques de fuerza bruta

#### **Capa 4 - Data**
- **Cifrado en reposo**: Datos protegidos incluso si se roban discos duros
- **Cifrado en tránsito**: Protección mientras datos se mueven entre sistemas  
- **Key Vault**: Material criptográfico protegido por HSMs

#### **Capa 5 - Monitoring**
- **Logging forense**: Cada acción significativa registrada
- **Alertas automáticas**: Detección de actividad sospechosa en tiempo real
- **Auditoría completa**: Trazabilidad que resiste escrutinio legal

## 📊 Análisis de Amenazas (STRIDE)

### Implementación de Mitigaciones Específicas:

| Amenaza | Descripción | Mitigación | Implementación Técnica |
|---------|-------------|------------|----------------------|
| **Spoofing** | Atacantes haciéndose pasar por usuarios legítimos | Azure AD Auth | OAuth 2.0 + JWT con expiración corta y validación criptográfica |
| **Tampering** | Modificación de datos en tránsito o almacenamiento | Data Integrity | HTTPS + Digital Signatures + validación de checksums |
| **Repudiation** | Usuarios negando acciones realizadas | Audit Logging | Application Insights con correlación de IP, timestamp, y user context |
| **Info Disclosure** | Exposición de datos sensibles | Encryption | Key Vault + TDE + cifrado a nivel de campo |
| **DoS** | Ataques para hacer la aplicación indisponible | Rate Limiting | API Throttling + WAF + Azure DDoS Protection |
| **Elevation** | Escalación no autorizada de privilegios | RBAC | Least Privilege + Claims-based authorization |


## 🗄️ Diseño de Base de Datos

### Tablas Principales con Consideraciones de Seguridad:

#### **Cliente**
```sql

CREATE TABLE [dbo].[Cliente](
	[IdCliente] [uniqueidentifier] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Apellidos] [varchar](50) NOT NULL,
	[DireccionEntrega] [varchar](150) NULL,
	[Ciudad] [varchar](100) NULL,
    [UsuarioCreacion] [uniqueidentifier],
    [FechaCreacion] [datetime2] DEFAULT GETDATE(),
    [EstadoActivo] [bit] DEFAULT 1

```

#### **Producto**  
```sql

CREATE TABLE [dbo].[Producto](
	[IdProducto] [int] IDENTITY(1,1) PRIMARY KEY,
	[IdCategoria] [int] NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Stock] [int] NOT NULL,
	[StockMinimo] [int] NOT NULL,
	[PrecioUnitario] [decimal](18, 2) NOT NULL,
    [PrecioEncriptado] [varbinary](128),
    [UsuarioCreacion] [uniqueidentifier],
    [FechaCreacion] [datetime2] DEFAULT GETDATE()
);

```

#### **Pago**  
```sql

CREATE TABLE [dbo].[Pago](
	[IdPago] [int] IDENTITY(1,1) PRIMARY KEY,
	[IdVenta] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Monto] [decimal](18, 2) NOT NULL,
	[FormaPago] [int] NOT NULL,
	[NumeroTarjeta] [varchar](16) NULL,
	[FechaVencimiento] [datetime] NULL,
	[CVV] [varchar](3) NULL,
	[NombreTitular] [varchar](100) NULL,
	[NumeroCuotas] [int] NULL,
	[Procesado] [int] NOT NULL,
    [UsuarioCreacion] [uniqueidentifier],   
    [FechaCreacion] [datetime2] DEFAULT GETDATE()   

```

#### **Venta**
```sql

CREATE TABLE [dbo].[Venta](
	[IdVenta] [int] IDENTITY(1,1) NOT NULL,
	[IdCliente] [int] NOT NULL,
	[Monto] [decimal](18, 2) NOT NULL,
    [UsuarioCreacion] [uniqueidentifier],
    [FechaCreacion] [datetime2] DEFAULT GETDATE(),

```

#### PROXIMAMENTE A IMPLEMENTAR EN NUEVAS MEJORAS, LAS TABLAS DE AUDITORIA Y USUARIOS CON DISTINTOS ROLES, ACTUALMENTE SOLO TENEMOS TABLA CLIENTES EN GENERAL

#### **AuditLogs**
```sql
CREATE TABLE AuditLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(50) NOT NULL,              -- Quién ejecutó la acción
    Action NVARCHAR(50) NOT NULL,              -- Qué acción se realizó  
    EntityType NVARCHAR(50),                   -- Qué tipo de objeto fue afectado
    EntityId NVARCHAR(50),                     -- Cuál objeto específico
    Changes NVARCHAR(MAX),                     -- Qué cambió exactamente (JSON)
    Timestamp DATETIME2 DEFAULT GETDATE(),    -- Cuándo ocurrió
    IpAddress NVARCHAR(45),                    -- Desde dónde se originó
    UserAgent NVARCHAR(500)                    -- Qué navegador/aplicación
);
```

### Características de Seguridad:

#### **Cifrado Selectivo**
- **Datos públicos** (nombres, descripciones): Sin cifrado especial
- **Datos internos** (precios de costo, márgenes): Cifrados con claves de Key Vault  
- **Datos personales** (información de contacto): Cifrados y sujetos a políticas GDPR
- **Datos de auditoría**: Almacenados con permisos de solo-inserción

#### **Auditoría Automática**
- **Campos temporales**: CreatedAt, UpdatedAt, UpdatedBy para cada tabla crítica
- **Versionado de datos**: Versiones históricas para detectar cambios no autorizados
- **Índices de seguridad**: Optimizados para consultas de auditoría rápidas incluso con millones de registros

#### **Integridad Referencial**
- **Claves foráneas**: Vinculación entre usuarios y acciones para trazabilidad completa
- **Constraints**: Validación a nivel de base de datos como última línea de defensa
- **Triggers de auditoría**: Registro automático de cambios sin dependencia del código de aplicación

## 🚀 Patrones de Despliegue Seguro

### Ambientes Separados
- **Desarrollo**: Key Vault propio, datos de prueba, logging extendido
- **Staging**: Réplica de producción con datos sintéticos  
- **Producción**: Configuración hardened, secrets rotación automática

### Infraestructura como Código
- **Bicep/ARM Templates**: Configuraciones auditables y repetibles
- **Azure DevOps**: Pipelines con gates de seguridad automáticos
- **Terraform**: Para organizaciones multi-cloud

### Estrategia de Backup y DR
- **RTO (Recovery Time Objective)**: < 4 horas
- **RPO (Recovery Point Objective)**: < 15 minutos  
- **Backup cifrado**: Protección del mismo nivel que datos de producción

## 📈 Métricas de Éxito

### KPIs de Seguridad
- **Mean Time to Detection (MTTD)**: < 5 minutos para amenazas críticas
- **Mean Time to Response (MTTR)**: < 30 minutos para incidentes de seguridad
- **False Positive Rate**: < 2% en alertas de seguridad
- **Compliance Score**: 100% en auditorías SOC 2 Type II

### Capacidades Empresariales
- **Escalabilidad**: Diseñado para crecer de 1,000 a 1,000,000 usuarios
- **Disponibilidad**: 99.9% uptime con failover automático
- **Performance**: Sub-500ms response time para operaciones críticas
- **Compliance**: Listo para auditorías PCI DSS, SOC 2, ISO 27001

---