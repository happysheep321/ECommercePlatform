namespace ECommerce.SharedKernel.Enums
{
    public enum UserStatus
    {
        Active = 0,     // 正常启用
        Banned = 1,     // 封禁/禁用（违规操作后不可登录）
        Deleted = 2,    // 已注销（逻辑删除，不展示）
        Inactive = 3,   // 未激活（注册后未验证邮箱/手机号）
        Frozen = 4      // 冻结（账号存在风险、临时冻结）
    }
}
