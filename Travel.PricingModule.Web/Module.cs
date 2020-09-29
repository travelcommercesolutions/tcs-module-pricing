
using Microsoft.Practices.Unity;
using System;

using Travel.PricingModule.Data.Models;
using Travel.PricingModule.Data.Repositories;
using Travel.PricingModule.Data.Services;

using Travel.PricingModule.Web.Infrastructure.Interceptors;
using Travel.ProfileModule.Web.Security;

using VirtoCommerce.Domain.Pricing.Model;

using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.PricingModule.Data.Model;
using IPricingSearchService = Travel.PricingModule.Core.Services.IPricingSearchService;
using IPricingService = Travel.PricingModule.Core.Services.IPricingService;

namespace Travel.PricingModule.Web
{
	public class Module : ModuleBase
	{
		//private readonly string _connectionString = "Travel.PricingModule";
		private const string _connectionStringName = "VirtoCommerce";
		private readonly IUnityContainer _container;

		public Module(IUnityContainer container)
		{
			_container = container;
		}

		public override void SetupDatabase()
		{
			// Modify database schema with EF migrations
			using (var context = new PricingModuleCoreContext(_connectionStringName, _container.Resolve<TravelPricingAuditableInterceptor>()))
			{
				var initializer = new SetupDatabaseInitializer<PricingModuleCoreContext, Data.Repositories.Migrations.Configuration>();
				initializer.InitializeDatabase(context);
			}
		}

	

		public override void Initialize()
		{
			base.Initialize();

			_container
							 .RegisterType<IPricingModuleCoreContext>
							 (
									 new InjectionFactory
									 (
											 c => new PricingModuleCoreContext(_connectionStringName, _container.Resolve<TravelPricingAuditableInterceptor>(), new EntityPrimaryKeyGeneratorInterceptor())
									 )
							 );

			
			// This method is called for each installed module on the first stage of initialization.

			// Register implementations:

			_container.RegisterType<IPricingSearchService, PricingSearchServiceImpl>();
			_container.RegisterType<IPricingService, PricingService>();


			// Try to avoid calling _container.Resolve<>();
		}

		public override void PostInitialize()
		{
			base.PostInitialize();

			var securityScopeService = _container.Resolve<IPermissionScopeService>();
			securityScopeService.RegisterSope(() => new ProfileVendorScope());
// This method is called for each installed module on the second stage of initialization.

			// Override types using AbstractTypeFactory:
			AbstractTypeFactory<Pricelist>.OverrideType<Pricelist, Core.Models.Search.Pricelist>();
			AbstractTypeFactory<PricelistEntity>.OverrideType<PricelistEntity, PricelistExEntity>();
			
		}
	}
}
