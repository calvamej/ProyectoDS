using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venta.Application.CasosUso.AdministrarProductos.ConsultarProductos
{
    public class ConsultarProductosValidator: AbstractValidator<ConsultarProductosRequest>
    {
        public ConsultarProductosValidator()
        {
            RuleFor(item => item.FiltroPorNombre).NotNull().NotEmpty().Length(1, 50).WithMessage("Nombre nulo, vacío o con más de 50 caracteres");
        }
    }
}
