using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Module.Platform.Core.Model;
using Travel.PricingModule.Core.Models.Search;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Pricing.Model.Search;

namespace Travel.PricingModule.Core.Services
{
	
		public interface IPricingSearchService
		{
			//Task<PriceSearchResult> SearchPricesAsync(PricesSearchCriteria criteria);


			Task<PricelistSearchResult> SearchPricelistsAsync(PricelistSearchCriteria criteria, bool isAdmin, string isUserName,string selectedVendorId);

		VendorProfilesList GetVendorList(GenericSearchResult<Member> members);
			//Task<PricelistAssignmentSearchResult> SearchPricelistAssignmentsAsync(PricelistAssignmentsSearchCriteria criteria);
	}
	
}
