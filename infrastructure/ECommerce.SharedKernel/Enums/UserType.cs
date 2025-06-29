namespace ECommerce.SharedKernel.Enums
{
    public enum UserType
    {
        Normal = 0,      // 普通买家
        Seller = 1,      // 卖家/商户
        Admin = 2,       // 平台管理员（可以管理用户、订单、商品）
        CustomerService = 3, // 客服账号（只读部分数据、答疑）
        System = 4       // 系统账号（内部系统接口调用或数据同步使用）
    }
}
