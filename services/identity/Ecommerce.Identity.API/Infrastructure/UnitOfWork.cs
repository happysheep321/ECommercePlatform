using ECommerce.SharedKernel.Interfaces;

namespace ECommerce.Identity.API.Infrastructure
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly IdentityDbContext context;

        public UnitOfWork(IdentityDbContext context)
        {
            this.context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}
