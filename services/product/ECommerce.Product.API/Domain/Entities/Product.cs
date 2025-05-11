namespace ECommerce.Product.API.Domain.Entities
{
    /// <summary>
    /// 商品实体
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// 商品价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string Description { get; set; }=string.Empty;

        /// <summary>
        /// 商品库存数量
        /// </summary>
        public int Stock { get; set; }
    }
}
