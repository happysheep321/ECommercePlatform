using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.BuildingBolcks.Authentication.Attributes
{
    public class PermissionAttribute
    {
        public string Code { get; set; }

        public PermissionAttribute(string code)
        {
            Code=code;
        }
    }
}
