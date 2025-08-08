using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venta.Application.CasosUso.AdministrarProductos.RegistrarProductos;

namespace Venta.Application.CasosUso.AdministrarVentas.RegistrarVenta
{
    public class RegistrarVentaValidator : AbstractValidator<RegistrarVentaRequest>
    {
        public RegistrarVentaValidator()
        {

            RuleFor(item => item.IdCliente).NotEmpty().GreaterThanOrEqualTo(0).WithMessage("IdCliente nulo, vacío o menor a 0");
            RuleFor(item => item.Productos).NotNull().NotEmpty().WithMessage("Lista de Productos nula o vacía");
        }
    }
}
