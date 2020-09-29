angular.module('travel.pricingModule')
    .controller('travel.pricingModule.pricesWidgetController', ['$scope', 'platformWebApp.bladeNavigationService', 'virtoCommerce.pricingModule.prices', function ($scope, bladeNavigationService, prices) {
    var blade = $scope.widget.blade;

    function refresh() {
        $scope.priceCount = '...';

        prices.search({
            priceListId: blade.currentEntityId,
            take: 0
        }, function (data) {
            $scope.priceCount = data.totalCount;
        });
    }

    $scope.openBlade = function () {
        var newBlade = {
            id: "pricelistChild",
            currency: blade.currentEntity.currency,
            currentEntity: blade.currentEntity,
            currentEntityId: blade.currentEntityId,
            parentWidgetRefresh: refresh,
            title: blade.title,
            subtitle: 'pricing.blades.pricelist-item-list.subtitle',
            controller: 'travel.pricingModule.pricelistItemListController',
            template: 'Modules/$(TCS.PricingModule)/Scripts/blades/price-list/pricelist-item-list.tpl.html'
        };

        bladeNavigationService.showBlade(newBlade, blade);
    };

    refresh();
}]);