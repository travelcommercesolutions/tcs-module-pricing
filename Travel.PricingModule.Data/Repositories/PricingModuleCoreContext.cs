using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.PricingModule.Core.Models.Search;
using Travel.PricingModule.Data.Models;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using DbContext = System.Data.Entity.DbContext;

namespace Travel.PricingModule.Data.Repositories
{
	public class PricingModuleCoreContext : VirtoCommerce.PricingModule.Data.Repositories.PricingRepositoryImpl,/* EFRepositoryBase,*/ IPricingModuleCoreContext //EFRepositoryBase //
	{
		//public PricingModuleCoreContext(string nameOrConnectionString, params IInterceptor[] interceptors)
		//	 : base(nameOrConnectionString,null, interceptors)
		//{ }

		public PricingModuleCoreContext(string nameOrConnectionString, params IInterceptor[] interceptors) 
			: base(nameOrConnectionString, interceptors)
		{ }

		//public PricingModuleCoreContext(string nameOrConnectionString, IInterceptor[] interceptors=null) 
		//	: base(nameOrConnectionString, interceptors)
		//{ }
		public PricingModuleCoreContext() : base()
		{ }

		
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PricelistExEntity>().HasKey(x => x.Id)
			.Property(x => x.Id);
		
			modelBuilder.Entity<PricelistExEntity>().ToTable("PricelistVendors").HasKey(x => x.Id);
			

			base.OnModelCreating(modelBuilder);
		}

		public IQueryable<PricelistExEntity> Pricelist => GetAsQueryable<PricelistExEntity>();
		
		
		public string GetVendorAssigned(string ids)
		{
			return Pricelist.Where(x => x.Id==ids).Select(c => c.Vendors).FirstOrDefault();
		}

		public PricelistExEntity[] GetPriceListDetails(string[] ids)
		{
			return Pricelist.Where(x => ids.Contains(x.Id)).ToArray();
		}
	}
}
