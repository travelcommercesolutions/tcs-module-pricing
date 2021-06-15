using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Travel.PricingModule.Core;
using Travel.PricingModule.Core.Models.Search;
using Travel.PricingModule.Core.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Customer.Model;
using VirtoCommerce.Domain.Customer.Services;
using VirtoCommerce.Domain.Pricing.Model.Search;
using VirtoCommerce.Domain.Pricing.Services;
using VirtoCommerce.Platform.Core.Web.Security;
using IPricingSearchService = Travel.PricingModule.Core.Services.IPricingSearchService;
using IPricingService = Travel.PricingModule.Core.Services.IPricingService;

namespace Travel.PricingModule.Web.Controllers.Api
{
	[RoutePrefix("api/TravelPricingModule")]
	
	public class TravelPricingModuleController : ApiController
	{

		private readonly IPricingSearchService _pricingSearchService;
		private readonly IPricingService _pricingService;
		private readonly IMemberSearchService _memberSearchSrv;
		

		public TravelPricingModuleController(
					 IPricingSearchService pricingSearchService,
					 IPricingService pricingService,
					 IMemberSearchService memberSearchSrv

			)
		{
			_pricingSearchService = pricingSearchService;
			_pricingService = pricingService;
			_memberSearchSrv = memberSearchSrv;

			}

		// <summary>
		// Get pricelists
		// </summary>
		//<remarks>Get all pricelists for all catalogs.</remarks>
		[HttpGet]
		[Route("pricelist/{keyword}/{skip}/{take}/{isAdmin}/{isUserName}/{selectedVendorId}")]
		[CheckPermission(Permission = ModuleConstants.Security.Permissions.Access)]
		public async Task<IHttpActionResult> SearchPricelistsAsync(string keyword,int skip, int take, bool isAdmin, string isUserName, string selectedVendorId)
		{
			var criteria = new PricelistSearchCriteria();
			criteria.Keyword = keyword;
			criteria.Skip = skip;
			criteria.Take = take;
			
			var result = await _pricingSearchService.SearchPricelistsAsync(criteria, isAdmin, isUserName, selectedVendorId);

			return Ok(result);
		}


		/// <summary>
		/// Get pricelist by Id
		/// </summary>
		[HttpGet]
		[Route("pricelistbyid/{id}")]
		public IHttpActionResult GetPriceListById(string id)
		{
			var pricelist = (_pricingService.GetPricelistsByIdAsync(new[] { id })).FirstOrDefault();
			
			return Ok(pricelist);
		}

		/// <summary>
		/// Get Vendors
		/// </summary>
		[HttpGet]
		[Route("getvendors")]
		public IHttpActionResult Getvendors()
		{
			var members = _memberSearchSrv.SearchMembers(new MembersSearchCriteria
			{
				MemberType = "Vendor",
				Skip = 0,
				Take = 100
			});
			var vendorList = _pricingSearchService.GetVendorList(members);
			
			return Ok(vendorList.VendorProfiles);
		}

		/// <summary>
		/// Update pricelist
		/// </summary>
		[HttpPut]
		[Route("updatepricelist")]
		[CheckPermission(Permission = ModuleConstants.Security.Permissions.Update)]
		public IHttpActionResult UpdatePriceList([FromBody] Pricelist priceList)
		{
			var result = _pricingService.SavePricelistsAsync(new[] { priceList });
			return Ok();
		}


		/// <summary>
		/// Create pricelist
		/// </summary>
		[HttpPost]
		[Route("createpricelist")]
		[CheckPermission(Permission = ModuleConstants.Security.Permissions.Create)]
		public IHttpActionResult CreatePriceList([FromBody]Pricelist priceList)
		{
			var result = _pricingService.CreatePriceList(priceList);
			return Ok();
		}

		/// <summary>
		/// Delete pricelists  
		/// </summary>
		/// <remarks>Delete pricelists by given array of pricelist ids.</remarks>
		/// <param name="priceListids">An array of pricelist ids</param>
		[HttpPost]
		[Route("deletepricelist")]
		[CheckPermission(Permission = ModuleConstants.Security.Permissions.Delete)]
		public IHttpActionResult DeletePriceList([FromBody] string[] priceListids)
		{
			
			_pricingService.DeletePriceList(priceListids);
			return Ok(priceListids);
		}

	}
}
