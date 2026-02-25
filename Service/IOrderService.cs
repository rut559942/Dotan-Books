using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Repository;

namespace Service
{
    public interface IOrderService
    {
        Task<string> PlaceOrderAsync(OrderCreateDto dto, int customerId);

    }
}
