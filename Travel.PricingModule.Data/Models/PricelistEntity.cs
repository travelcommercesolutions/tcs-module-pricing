using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travel.PricingModule.Core.Models.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.PricingModule.Data.Model;

namespace Travel.PricingModule.Data.Models
{

	public class PricelistExEntity : VirtoCommerce.PricingModule.Data.Model.PricelistEntity
	{
		[Column(TypeName = "NVARCHAR")]
		public string Vendors { get; set; }

		#region Navigation Properties

		#endregion

		public virtual PricelistExEntity FromModel(Pricelist pricelist, PrimaryKeyResolvingMap pkMap)
		{
			if (pricelist == null)
				throw new ArgumentNullException(nameof(pricelist)); pkMap.AddPair(pricelist, this);

			Id = pricelist.Id;
			ModifiedBy = pricelist.ModifiedBy;
			Name = pricelist.Name;
			Description = pricelist.Description;
			Currency = pricelist.Currency;
			Vendors = pricelist.Vendors;
			return this;

		}



		public virtual void Patch(PricelistExEntity target)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			base.Patch(target);
			var priceExEntity = (PricelistExEntity)target;
			priceExEntity.Vendors = Vendors;


		}

	

		public virtual PricelistExEntity ToModel(Pricelist pricelist)
		{
			if (pricelist == null)
				throw new ArgumentNullException(nameof(pricelist));
			PricelistExEntity priceList = new PricelistExEntity();

			priceList.CreatedDate = System.DateTime.Now;
			priceList.ModifiedDate = System.DateTime.Now;
			priceList.Id = pricelist.Id;
			priceList.ModifiedBy = pricelist.ModifiedBy;
			priceList.Name = pricelist.Name;
			priceList.Description = pricelist.Description;
			priceList.Currency = pricelist.Currency;
			priceList.Vendors = pricelist.Vendors;

			return priceList;
		}


	}
}
