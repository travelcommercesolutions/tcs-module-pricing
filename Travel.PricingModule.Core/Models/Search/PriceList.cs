using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Data.Model;

namespace Travel.PricingModule.Core.Models.Search
{
	public class Pricelist : VirtoCommerce.Domain.Pricing.Model.Pricelist
	{
	
		public string Vendors { get; set; }

		#region ICloneable members
		//public virtual object Clone()
		//{
		//	var result = MemberwiseClone() as Pricelist;

		//	result.Prices = Prices?.Select(x => x.Clone()).OfType<Price>().ToList();
		//	result.Assignments = Assignments?.Select(x => x.Clone()).OfType<PricelistAssignment>().ToList();

		//	return result;
		//}
		#endregion
	}
}
