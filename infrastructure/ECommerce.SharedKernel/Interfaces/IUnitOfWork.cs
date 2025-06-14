using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.SharedKernel.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
