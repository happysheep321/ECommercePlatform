using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BuildingBlocks.Authentication.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 权限编码
        /// </summary>
        public string PermissionCode { get; }


        public PermissionAttribute(string permissionCode)
        {
            PermissionCode = permissionCode;

            // 用 policy 名称作为权限编码
            Policy = permissionCode;
        }
    }
}
