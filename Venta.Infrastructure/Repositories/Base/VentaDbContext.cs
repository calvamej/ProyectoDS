using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.API.Models;
using Venta.Domain.Models;

namespace Venta.Infrastructure.Repositories.Base
{
    public class VentaDbContext: DbContext
    {
       /// <summary>
       /// Configurando el aceso a la base de datos
       /// </summary>
       /// <param name="options"></param>
        public VentaDbContext(DbContextOptions<VentaDbContext>  options)
            :base (options) 
        {

        }

        public virtual DbSet<Producto> Productos { get; set; }


        public virtual DbSet<Categoria> Categorias { get; set; }

        public virtual DbSet<Cliente> Clientes { get; set; }


        public virtual DbSet<Domain.Models.Venta> Ventas { get; set; }
        public virtual DbSet<Domain.Models.Pago> Pagos { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            #region Configuraando las entidades en archivos de tipos separados
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VentaDbContext).Assembly);
            #endregion

            #region Configuracion de las entidades en el mismo ModelCreating
            //modelBuilder.Entity<Categoria>(
            //    p =>
            //    {
            //        p.ToTable("Categoria");
            //        p.HasKey(c => c.IdCategoria);
            //        //p.Property(p => p.Nombre).HasColumnName("Nombre");
            //    }
            //    );


            //modelBuilder.Entity<Cliente>(
            //    p =>
            //    {
            //        p.ToTable("Cliente");
            //        p.HasKey(c => c.IdCliente);
            //    }
            //    );

            //modelBuilder.Entity<Producto>(
            //    p =>
            //    {
            //        p.ToTable("Producto");
            //        p.HasKey(c => c.IdProducto);
            //        p.Property(c => c.PrecioUnitario).HasPrecision(2);

            //        p.HasOne(p => p.Categoria).WithMany(p => p.Productos)
            //            .HasForeignKey(p => p.IdCategoria);
            //    }                
            //    );


            //modelBuilder.Entity<Domain.Models.Venta>(
            //   p =>
            //   {
            //       p.ToTable("Venta");
            //       p.HasKey(c => c.IdVenta);

            //       p.HasOne(p => p.Cliente).WithMany(p => p.Ventas)
            //            .HasForeignKey(p => p.IdCliente);
            //   }
            //   );


            //modelBuilder.Entity<Domain.Models.VentaDetalle>(
            //   p =>
            //   {
            //       p.ToTable("VentaDetalle");
            //       p.HasKey(c => c.IdVentaDetalle);

            //       p.HasOne(p => p.Venta).WithMany(p => p.Detalle)
            //            .HasForeignKey(p => p.IdVenta);

            //       p.HasOne(p => p.Producto).WithMany(p => p.VentaDetalles)
            //           .HasForeignKey(p => p.IdProducto);
            //   }
            //   );

            #endregion


        }
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
}
