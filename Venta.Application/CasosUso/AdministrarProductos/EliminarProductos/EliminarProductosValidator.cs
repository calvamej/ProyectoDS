using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.ActualizarProductos;

namespace Venta.Application.CasosUso.AdministrarProductos.EliminarProductos
{
    public class EliminarProductosValidator : AbstractValidator<EliminarProductosRequest>
    {
        public EliminarProductosValidator()
        {

            RuleFor(item => item.IdProducto).NotNull().GreaterThanOrEqualTo(0).WithMessage("IdProducto nulo o menor a 0");

        }

    }
}
