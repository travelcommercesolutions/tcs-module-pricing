using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtoCommerce.Domain.Store.Model;
using VirtoCommerce.Platform.Core.Security;

namespace Travel.ProfileModule.Web.Security
{
    public class ProfileVendorScope : PermissionScope
    {
        public override bool IsScopeAvailableForPermission(string permission)
        {
            return permission == VendorPermissions.ViewVendorProfiles ||
                  
                   permission == VendorPermissions.CancelVendorProfiles;
        }

        public override IEnumerable<string> GetEntityScopeStrings(object entity)
        {
					if (entity == null)
					{
						throw new ArgumentNullException("entity");
					}
					var store = entity as Store;
					if (store != null)
					{
						return new[] { Type + ":" + store.Id };
					}

					return Enumerable.Empty<string>(); ;
		}
    }
}