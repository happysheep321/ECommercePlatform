using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Domain.ValueObjects
{
    public class Region : ValueObject
    {
        public string Province { get; private set; }
        public string City { get; private set; }
        public string District { get; private set; }

        public Region(string province, string city, string district)
        {
            Province = province;
            City = city;
            District = district;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Province;
            yield return City;
            yield return District;
        }
    }
}
