USE [db_venta]
GO
/****** Object:  Table [dbo].[AuditLog] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserEmail] [varchar](100),
	[EntityName] [varchar](100) NOT NULL,
	[Action] [varchar](100) NOT NULL,
	[Timestamp] [datetime],
	[Changes] [varchar](500) NOT NULL,
 CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Auditoría por BD******/

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