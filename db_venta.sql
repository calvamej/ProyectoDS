CREATE DATABASE [db_venta]
GO

USE [db_venta]
GO

-- Create a master key (if not already created)
CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'Your$StrongP@ssw0rd';

-- Create a certificate
CREATE CERTIFICATE Cert_Pagos
WITH SUBJECT = 'Certificate to protect Pagos data';

-- Create a symmetric key for column encryption
CREATE SYMMETRIC KEY SymKey_Pagos
WITH ALGORITHM = AES_256
ENCRYPTION BY CERTIFICATE Cert_Pagos;

/****** Object:  Table [dbo].[Categoria]    Script Date: 17/02/2024 15:52:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categoria](
	[IdCategoria] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NULL,
 CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED 
(
	[IdCategoria] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[Categoria] (Nombre)
VALUES 
('Electrónica'),
('Ropa'),
('Hogar'),
('Libros'),
('Deportes');

/****** Object:  Table [dbo].[Cliente]    Script Date: 17/02/2024 15:52:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[IdCliente] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Apellidos] [varchar](50) NOT NULL,
	[DireccionEntrega] [varchar](150) NULL,
	[Ciudad] [varchar](100) NULL,
 CONSTRAINT [PK_Cliente] PRIMARY KEY CLUSTERED 
(
	[IdCliente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[Cliente] (Nombre, Apellidos, DireccionEntrega, Ciudad)
VALUES 
('Juan', 'Pérez', 'Av. Siempre Viva 123', 'Lima'),
('Luisa', 'Gómez', 'Calle Falsa 456', 'Cusco'),
('Carlos', 'Ruiz', 'Jr. Los Pinos 789', 'Arequipa'),
('Ana', 'Torres', 'Av. Independencia 321', 'Trujillo'),
('Diego', 'Salas', 'Calle Mayor 111', 'Piura');

/****** Object:  Table [dbo].[Pago]    Script Date: 17/02/2024 15:52:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pago](
	[IdPago] [int] IDENTITY(1,1) NOT NULL,
	[IdVenta] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Monto] [VARBINARY](MAX) NOT NULL,
	[FormaPago] [int] NOT NULL,
	[NumeroTarjeta] [VARBINARY](MAX) NOT NULL,
	[FechaVencimiento] [datetime] NULL,
	[CVV] [varchar](3) NULL,
	[NombreTitular] [varchar](100) NULL,
	[NumeroCuotas] [int] NULL,
	[Procesado] [int] NOT NULL,
 CONSTRAINT [PK_Pago] PRIMARY KEY CLUSTERED 
(
	[IdPago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

OPEN SYMMETRIC KEY SymKey_Pagos
DECRYPTION BY CERTIFICATE Cert_Pagos;

INSERT INTO [dbo].[Pago] (IdVenta, Fecha, Monto, FormaPago, NumeroTarjeta, FechaVencimiento, CVV, NombreTitular, NumeroCuotas, Procesado)
VALUES
(1, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '1500.00')), 1, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '4111111111111111')), '2026-12-31', '123', 'Juan Pérez', 3, 1),
(2, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '800.00')), 2, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '5555555555554444')), '2025-11-30', '456', 'Luisa Gómez', 2, 0),
(3, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '920.00')), 1, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '6011111111111117')), '2025-10-15', '789', 'Carlos Ruiz', 4, 1),
(4, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '450.00')), 1, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '30569309025904')), '2026-08-01', '111', 'Ana Torres', 1, 1),
(5, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '850.00')), 2, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '378282246310005')), '2026-04-30', '222', 'Diego Salas', 5, 0),
(6, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '400.00')), 1, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '4111111111111111')), '2026-03-31', '333', 'Juan Pérez', 2, 1),
(7, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '320.00')), 2, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '5555555555554444')), '2025-05-10', '444', 'Luisa Gómez', 3, 0),
(8, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '280.00')), 1, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '6011111111111117')), '2025-08-12', '555', 'Carlos Ruiz', 1, 1),
(9, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '250.00')), 1, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '30569309025904')), '2026-01-15', '666', 'Ana Torres', 2, 1),
(10, GETDATE(), EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '300.00')), 2, EncryptByKey(Key_GUID('SymKey_Pagos'), CONVERT(NVARCHAR(100), '378282246310005')), '2026-06-01', '777', 'Diego Salas', 3, 1);

CLOSE SYMMETRIC KEY SymKey_Pagos;

/****** Object:  Table [dbo].[Producto]    Script Date: 17/02/2024 15:52:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Producto](
	[IdProducto] [int] IDENTITY(1,1) NOT NULL,
	[IdCategoria] [int] NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Stock] [int] NOT NULL,
	[StockMinimo] [int] NOT NULL,
	[PrecioUnitario] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Producto] PRIMARY KEY CLUSTERED 
(
	[IdProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[Producto] (IdCategoria, Nombre, Stock, StockMinimo, PrecioUnitario)
VALUES 
(1, 'Laptop Dell', 100, 10, 2500.00),
(1, 'Mouse Logitech', 200, 20, 150.00),
(2, 'Camisa Blanca', 150, 15, 80.00),
(3, 'Licuadora Oster', 50, 5, 300.00),
(4, 'El Principito', 120, 10, 35.00),
(5, 'Pelota de fútbol', 80, 8, 120.00),
(2, 'Pantalón Jean', 100, 10, 110.00),
(3, 'Sartén Tefal', 60, 6, 140.00),
(1, 'Monitor LG', 70, 7, 900.00),
(5, 'Bicicleta Urbana', 30, 3, 850.00);

/****** Object:  Table [dbo].[Venta]    Script Date: 17/02/2024 15:52:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Venta](
	[IdVenta] [int] IDENTITY(1,1) NOT NULL,
	[IdCliente] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Monto] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Venta] PRIMARY KEY CLUSTERED 
(
	[IdVenta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[Venta] (IdCliente, Fecha, Monto)
VALUES 
(1, GETDATE(), 2000.00),
(2, GETDATE(), 1500.00),
(3, GETDATE(), 1800.00),
(4, GETDATE(), 950.00),
(5, GETDATE(), 1700.00),
(1, GETDATE(), 890.00),
(2, GETDATE(), 720.00),
(3, GETDATE(), 610.00),
(4, GETDATE(), 450.00),
(5, GETDATE(), 330.00);

/****** Object:  Table [dbo].[VentaDetalle]    Script Date: 17/02/2024 15:52:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VentaDetalle](
	[IdVentaDetalle] [int] IDENTITY(1,1) NOT NULL,
	[IdVenta] [int] NOT NULL,
	[IdProducto] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Precio] [decimal](18, 2) NOT NULL,
	[SubTotal] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_VentaDetalle] PRIMARY KEY CLUSTERED 
(
	[IdVentaDetalle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[VentaDetalle] (IdVenta, IdProducto, Cantidad, Precio, SubTotal)
VALUES
(1, 1, 1, 2500.00, 2500.00),
(2, 3, 2, 80.00, 160.00),
(3, 2, 3, 150.00, 450.00),
(4, 5, 2, 35.00, 70.00),
(5, 10, 1, 850.00, 850.00),
(6, 7, 2, 110.00, 220.00),
(7, 6, 1, 120.00, 120.00),
(8, 8, 1, 140.00, 140.00),
(9, 4, 1, 300.00, 300.00),
(10, 9, 1, 900.00, 900.00);

CREATE OR ALTER PROCEDURE dbo.GetPagos
AS
BEGIN
    SET NOCOUNT ON;

    OPEN SYMMETRIC KEY SymKey_Pagos
        DECRYPTION BY CERTIFICATE Cert_Pagos;

    SELECT 
		IdVenta, 
		Fecha,
		CAST(DecryptByKey(monto) AS NVARCHAR(100)) AS Monto, 
		FormaPago, 
		CAST(DecryptByKey(numeroTarjeta) AS NVARCHAR(100)) AS NumeroTarjeta, 
		FechaVencimiento 
	FROM [dbo].[Pago];

    CLOSE SYMMETRIC KEY SymKey_Pagos;
END
GO
