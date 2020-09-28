using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.PricingModule.Core.Models.Search;
using Travel.PricingModule.Core.Services;
using Travel.PricingModule.Data.Models;
using Travel.PricingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

using System.Data.Entity;
using VirtoCommerce.PricingModule.Data.Model;
using VirtoCommerce.Domain.Pricing.Model.Search;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Domain.Customer.Model;
using Travel.Module.Platform.Core.Model;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Security;

namespace Travel.PricingModule.Data.Services
{
	public class PricingSearchServiceImpl : IPricingSearchService
	{
		private readonly Func<PricingModuleCoreContext> _repositoryFactory;
		private readonly IPricingService _pricingService;
		private readonly ISecurityService _securityService;
		public  static string loginUserAssignedVendor;
		public static string loginUserSelectedVendor;

		private readonly List<string> _vendorAgentPricingPermissionsNames = new List<string>
				{
						"vendor:pricing:view",
						"vendor:pricing:change"
				};


		public PricingSearchServiceImpl(Func<PricingModuleCoreContext> repositoryFactory,
			IPricingService pricingService, 
			ISecurityService securityService
					
						)
		{
			_repositoryFactory = repositoryFactory;
			_pricingService = pricingService;
			_securityService = securityService;
			

		}

		public virtual async Task<PricelistSearchResult> SearchPricelistsAsync(PricelistSearchCriteria criteria,bool isAdmin,string isUserName,string selectedVendorId)
		{
			var result = AbstractTypeFactory<PricelistSearchResult>.TryCreateInstance();
			loginUserSelectedVendor = selectedVendorId;
			using (var repository = new PricingModuleCoreContext())
			{
				var query = BuildQuery(repository, criteria);
					
			if (criteria.Take > 0)
					{
					
					var pricelistIds = await query.Select(x => x.Id)
																									.ToArrayAsync();

					var unorderedResults = _pricingService.GetPricelistsByIdAsync(pricelistIds);
						if (!isAdmin)
							{
								loginUserAssignedVendor = GetUserTypePermission(isUserName);
								string[] scopeIds = loginUserAssignedVendor.Split(',');
								var resultList = unorderedResults.Where(c => scopeIds.Contains(c.Vendors));
								var priceList = resultList.Where(x => x.Vendors == selectedVendorId);
								result.Results = priceList.ToList();
								return result;
							}
						else
							{
								loginUserAssignedVendor = null;
						
								if (selectedVendorId != "null")
								{
									var priceList = unorderedResults.OrderBy(x => Array.IndexOf(pricelistIds, x.Id)).ToList();
									var priceListforSelectedVendor = priceList.Where(x => x.Vendors == selectedVendorId);
									result.Results = priceListforSelectedVendor.ToList();
									//result.Results = r.ToList();
									return result;
								}
								else
								{
								  loginUserSelectedVendor = null;
									result.Results = unorderedResults.OrderBy(x => Array.IndexOf(pricelistIds, x.Id)).ToList();
									return result;
								}
					}
					}
				}
			return result;
		}

		protected virtual IQueryable<PricelistEntity> BuildQuery(PricingModuleCoreContext repository, PricelistSearchCriteria criteria)
		{
			var query = repository.Pricelists;

			if (!string.IsNullOrEmpty(criteria.Keyword) && criteria.Keyword != "null")
			{
				query = query.Where(x => x.Name.Contains(criteria.Keyword) || x.Description.Contains(criteria.Keyword));
			}

			if (!criteria.Currencies.IsNullOrEmpty())
			{
				query = query.Where(x => criteria.Currencies.Contains(x.Currency));
				}

				return query;
		}

		public VendorProfilesList GetVendorList(GenericSearchResult<Member> members)
		{
			
			var vendors = new VendorProfilesList
			{
				VendorProfiles = new List<VendorProfile>(),
				TotalItems = members.TotalCount
			};
			if (!String.IsNullOrEmpty(loginUserAssignedVendor))
			{
				string[] scopeIds = loginUserAssignedVendor.Split(',');
				foreach (var item in members.Results.Where(c => scopeIds.Contains(c.Id) && c.Id == loginUserSelectedVendor))
					vendors.VendorProfiles.Add(new VendorProfile
					{
						Name = item.Name,
						Id = item.Id,

					});

				return vendors;
			}
			else
			{
				if(loginUserSelectedVendor == null)
				{
					foreach (var item in members.Results)
						vendors.VendorProfiles.Add(new VendorProfile
						{
							Name = item.Name,
							Id = item.Id,

						});
					return vendors;
				}
				else
				{
					foreach (var item in members.Results.Where(v => v.Id == loginUserSelectedVendor))
						vendors.VendorProfiles.Add(new VendorProfile
						{
							Name = item.Name,
							Id = item.Id,

						});
					return vendors;
				}
				
			}
			
		}

		private static string[] GetPropValue(IHasDynamicProperties member, string propName)
		{
			try
			{
				var dynamicProp = member.DynamicProperties.FirstOrDefault(x => x.Name == propName);
				if (dynamicProp != null)
				{
					if (dynamicProp.Values != null && dynamicProp.Values.Count != 0)
						return dynamicProp.Values.Select(x => x.Value.ToString()).ToArray();

					return new string[] { };
				}
			}
			catch (Exception ex)
			{
				return new string[] { };
			}

			return new string[] { };
		}

		public string GetUserTypePermission(string username)
		{

			var user = _securityService.FindByNameAsync(username, UserDetails.Full);
			var result = user != null ? user.Result.Roles.SelectMany(x => x.Permissions).ToArray() : Enumerable.Empty<Permission>().ToArray();
			var permissions = result.Where(c => _vendorAgentPricingPermissionsNames.Contains(c.Id))
					.ToList();
			StringBuilder assignedVendorScope = new StringBuilder();

			foreach (var permission in permissions)
			{
				var vendorScope = permission.AssignedScopes.FirstOrDefault(c => c.Scope != null);

				if (vendorScope == null || (assignedVendorScope.ToString().Contains(vendorScope.Scope)))
					continue;

				assignedVendorScope.Append(vendorScope.Scope);
				assignedVendorScope.Append(",");
			}

			return assignedVendorScope.ToString();


		}



	}
}
