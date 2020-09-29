angular
    .module('travel.pricingModule')
    .service('travel.pricingModule.pricelists', [
        '$http',
        '$state',
      
        function ($http, $state, menuService) {
            var API_URL = 'api/TravelPricingModule';

            this.search = function (criteria, isAdmin, isUserName, selectedVendorId) {
                return $http
                    .get(API_URL + '/pricelist' + '/' + criteria.keyword 
                    + '/' + criteria.skip + '/' + criteria.take + '/' + isAdmin + '/' + isUserName + '/' + selectedVendorId)
                    
                    .then(function (response) {
                        return response.data;
                    });
            };

            

            this.get = function (id) {
                return $http
                    .get(API_URL + '/pricelistbyid' + '/' + id)

                    .then(function (response) {
                        return response.data;
                    });
            };

            this.getvendors = function () {
                return $http
                    .get(API_URL + '/getvendors')

                    .then(function (response) {
                        return response.data;
                    });
            };

            this.update = function (priceList) {
                return $http
                    .put(API_URL + '/updatepricelist', priceList)
                    

                    .then(function (response) {
                        return response.data;
                    });
            };

            this.save = function (priceList) {
                return $http
                    .post(API_URL + '/createpricelist', priceList)
                    .then(function (response) {
                        return response.data;
                    });
            };

            this.remove = function (priceListid) {
                return $http
                    .post(API_URL + '/deletepricelist', priceListid.ids)
                    .then(function (response) {
                        return response.data;
                    });
            };
        }
    ]);
