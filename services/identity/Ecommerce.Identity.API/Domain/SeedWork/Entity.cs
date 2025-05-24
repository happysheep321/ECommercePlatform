namespace Ecommerce.Identity.API.Domain.SeedWork
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; }

        public override bool Equals(object obj)
        {
            if (obj is not Entity<TId> other)
                return false;

            return Id!.Equals(other.Id);
        }
        public override int GetHashCode()
        {
            return Id!.GetHashCode();
        }
    }
}
