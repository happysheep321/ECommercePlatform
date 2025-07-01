using Ecommerce.SharedKernel.Base;

namespace Ecommerce.Identity.API.Domain.ValueObjects
{
    public class Region : ValueObject
    {
        /// <summary>
        /// 省级
        /// </summary>
        public string Province { get; private set; }

        /// <summary>
        /// 市级
        /// </summary>
        public string City { get; private set; }

        /// <summary>
        /// 区级
        /// </summary>
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
