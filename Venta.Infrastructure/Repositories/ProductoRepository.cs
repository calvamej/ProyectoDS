using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Venta.Domain.Models;
using Venta.Domain.Repositories;
using Venta.Infrastructure.Repositories.Base;

namespace Venta.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly VentaDbContext _context;
        public ProductoRepository(VentaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Adicionar(Producto entity)
        {
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        public async Task<Producto> Consultar(int id)
        {
           return await _context.Productos.FindAsync(id);
        }

        public async Task<IEnumerable<Producto>> Consultar(string nombre)
        {
            return await _context.Productos.Where(p => p.Nombre.Equals(nombre)).ToListAsync();
        }

        public async Task<bool> Eliminar(Producto entity)
        {
            try
            {
                var affectedRows = await _context.Productos.Where(x => x.IdProducto.Equals(entity.IdProducto))
                    .ExecuteDeleteAsync();

                return affectedRows == 0 ? false : true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Modificar(Producto entity)
        {
            try
            {
                var affectedRows = await _context.Productos.Where(x => x.IdProducto.Equals(entity.IdProducto))
                .ExecuteUpdateAsync(z => z.SetProperty(y => y.Nombre, entity.Nombre)
                .SetProperty(y => y.Stock, entity.Stock)
                .SetProperty(y => y.StockMinimo, entity.StockMinimo)
                .SetProperty(y => y.PrecioUnitario, entity.PrecioUnitario)
                .SetProperty(y => y.IdCategoria, entity.IdCategoria));

                return affectedRows == 0 ? false : true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> ModificarStock(int idProducto, int cantidad)
        {
            try
            {
                var affectedRows = await _context.Productos.Where(x => x.IdProducto.Equals(idProducto))
                .ExecuteUpdateAsync(z => z.SetProperty(y => y.Stock, y => y.Stock - cantidad));

                return affectedRows == 0 ? false : true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
