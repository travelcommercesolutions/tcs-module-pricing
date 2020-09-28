using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.PricingModule.Core.Models.Search;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.PricingModule.Data.Model;

namespace Travel.PricingModule.Core.Services
{
	public interface IPricingService
	{
		Pricelist[] GetPricelistsByIdAsync(string[] ids);
		 Task SavePricelistsAsync(Pricelist[] priceLists);
		Task CreatePriceList(Pricelist priceLists);
	void DeletePriceList(string[] priceListsid);
	}
}