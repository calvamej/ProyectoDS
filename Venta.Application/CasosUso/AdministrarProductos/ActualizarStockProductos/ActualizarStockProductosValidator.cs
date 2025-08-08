using FluentValidation;

namespace Venta.Application.CasosUso.AdministrarProductos.ActualizarStockProductos
{
    public class ActualizarStockProductosValidator : AbstractValidator<ActualizarStockProductosRequest>
    {
        public ActualizarStockProductosValidator() {

            RuleFor(item => item.IdProducto).NotNull().GreaterThanOrEqualTo(0).WithMessage("IdProducto nulo o menor a 0");
            RuleFor(item => item.Stock).NotNull().GreaterThanOrEqualTo(0).WithMessage("Stock nulo o menor a 0");
        }

    }
}
