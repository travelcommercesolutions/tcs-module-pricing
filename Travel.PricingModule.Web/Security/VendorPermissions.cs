using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Travel.ProfileModule.Web.Security
{
    public static class VendorPermissions
    {
        public const string ViewVendorProfiles = "vendor:pricing:view";
        
        public const string CancelVendorProfiles = "vendor:pricing:change";
    }
}