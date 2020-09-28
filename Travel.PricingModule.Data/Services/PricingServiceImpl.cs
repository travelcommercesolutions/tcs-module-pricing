using CacheManager.Core;
using Common.Logging;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Travel.PricingModule.Core.Models.Search;
using Travel.PricingModule.Data.Models;
using Travel.PricingModule.Data.Repositories;
using VirtoCommerce.Domain.Catalog.Services;
using VirtoCommerce.Domain.Pricing.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Serialization;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.PricingModule.Data.Model;
using VirtoCommerce.PricingModule.Data.Repositories;

namespace Travel.PricingModule.Data.Services
{
	public class PricingService : ServiceBase, Core.Services.IPricingService
	{
		private readonly Func<IPricingModuleCoreContext> _repositoryFactory;
		public PricingService(Func<IPricingModuleCoreContext> repositoryFctry)
		{
			_repositoryFactory = repositoryFctry;
		}
		
		public Pricelist[] GetPricelistsByIdAsync(string[] ids)
		{
			
			var pricingRepositoryImpl = new PricingRepositoryImpl();

			Pricelist[] priceList = null;
			
			VirtoCommerce.Domain.Pricing.Model.Pricelist[] result = null;
			if (ids != null)
			{
				using (var repository = new PricingModuleCoreContext())
				{
					
					var pricelistEntities = pricingRepositoryImpl.GetPricelistByIds(ids);
					result = pricelistEntities.Select(x => x.ToModel(AbstractTypeFactory<Pricelist>.TryCreateInstance())).ToArray();
					List<Pricelist> list = result.OfType<Pricelist>().ToList();
					priceList = list.Cast<Pricelist>().ToArray();

					foreach(var res in priceList)
					{
						res.Vendors = repository.GetVendorAssigned(res.Id);
					}
					
					return priceList;

				}
			}
			return priceList;
		}

		public virtual async Task SavePricelistsAsync(Pricelist[] priceLists)
		{
			if (priceLists == null)
				throw new ArgumentNullException(nameof(priceLists));

			var pkMap = new PrimaryKeyResolvingMap();

			var pricingRepositoryImpl = new PricingRepositoryImpl();
			using (var repository = new PricingModuleCoreContext())
			{
				using (var changeTracker = GetChangeTracker(repository))
				{
					bool saveFailed;
					var pricelistsIds = priceLists.Select(x => x.Id).Where(x => x != null).Distinct().ToArray();
					var alreadyExistEntities = GetPricelistsByIdAsync(pricelistsIds);
					foreach (var pricelist in priceLists)
							{
								var sourceEntity = AbstractTypeFactory<PricelistExEntity>.TryCreateInstance().FromModel(pricelist, pkMap);
								var targetEntityAlreadyExist = alreadyExistEntities.FirstOrDefault(x => x.Id == pricelist.Id);

								if (targetEntityAlreadyExist != null)
								{
									var targetEntity = AbstractTypeFactory<PricelistExEntity>.TryCreateInstance().ToModel(targetEntityAlreadyExist);
									repository.Attach(targetEntity);
									sourceEntity.Patch(targetEntity);
								}
								else
								{
									repository.Add(sourceEntity);
								}
							}
					do
					{
						saveFailed = false;
						try
						{
							CommitChanges(repository);
							pkMap.ResolvePrimaryKeys();
						}
						catch (DbUpdateConcurrencyException ex)
						{
							var entry = ex.Entries.Single();
							entry.OriginalValues.SetValues(entry.GetDatabaseValues());
						}

					} while (saveFailed);
				}
			}
			
	}

		public virtual async Task CreatePriceList(Pricelist priceLists)
		{
			if (priceLists == null)
				throw new ArgumentNullException(nameof(priceLists));

			var pkMap = new PrimaryKeyResolvingMap();
			if (priceLists == null)
			{
				throw new ArgumentNullException("paymentReminder");
			}

			using (var repository = _repositoryFactory())
			{
				var entity = new PricelistExEntity();
				entity = entity.FromModel(priceLists, pkMap);
				repository.Add(entity);
				CommitChanges(repository);
			}
		}

		public void DeletePriceList(string[] priceListsid)
		{
			if (priceListsid == null)
				throw new ArgumentNullException(nameof(priceListsid));

			var pricingRepositoryImpl = new PricingRepositoryImpl();
			if (priceListsid == null)
			{
				throw new ArgumentNullException("priceListsid is null");
			}
			
			using (var repository = new PricingModuleCoreContext())
			{
				var priceDetailList = repository.GetPriceListDetails(priceListsid);
				foreach(var result in priceDetailList)
				{
					repository.Remove(result);
					CommitChanges(repository);
				}
			}

			pricingRepositoryImpl.DeletePricelists(priceListsid);

		}

	
	}
}
