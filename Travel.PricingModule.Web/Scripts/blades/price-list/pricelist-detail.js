angular.module('travel.pricingModule')
    .controller('travel.pricingModule.pricelistDetailController', ['$scope', 'platformWebApp.bladeNavigationService', 'travel.pricingModule.pricelists', 'platformWebApp.settings', 'virtoCommerce.coreModule.currency.currencyUtils',
        function ($scope, bladeNavigationService, pricelists, settings, currencyUtils) {
            var blade = $scope.blade;
            blade.updatePermission = 'pricing:update';
            var isAdmin = sessionStorage.getItem('isAdmin');
            var isUserName = sessionStorage.getItem('isUserName');
            blade.refresh = function (parentRefresh) {
                if (blade.isNew) {
                    initializeBlade({ productPrices: [], assignments: [] });
                } else {
                    var id = blade.currentEntityId;
                    pricelists.get(id)
                        .then(function (data) {
                            initializeBlade(data);

                        if (parentRefresh) {
                            blade.parentBlade.refresh();
                        }
                        });
                }
             };

            function initializeBlade(data) {
                if (!blade.isNew) {
                    blade.title = data.name;
                    
                }
                
                
                 pricelists.getvendors()
                   .then(function (data) {
                     $scope.vendors = data
                     });
               
                
                

                blade.currentEntity = angular.copy(data);
                blade.origEntity = data;
                blade.isLoading = false;
            }

            function isDirty() {
                return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
            }

            function canSave() {
                return isDirty() && $scope.formScope && $scope.formScope.$valid;
            }

            $scope.cancelChanges = function () {
                angular.copy(blade.origEntity, blade.currentEntity);
                $scope.bladeClose();
            };
            $scope.saveChanges = function () {
                blade.isLoading = true;

                if (blade.isNew) {
                     pricelists.save({
                        Name: blade.currentEntity.name,
                        Currency: blade.currentEntity.currency,
                        Assignments: blade.currentEntity.assignments,
                        Description: blade.currentEntity.description,
                        Vendors: blade.currentEntity.vendors,
                        Prices: blade.currentEntity.Prices

                        }).then(function (data) {
                		angular.copy(blade.currentEntity, blade.origEntity);
                		$scope.bladeClose();
                        if (blade.saveCallback) {
                        	blade.saveCallback(data);
                       }
                        else {
                        	blade.parentBlade.refresh();
                        }  
                    });
                } else {
                    pricelists.update({
                        Name: blade.currentEntity.name,
                        Id: blade.currentEntity.id,
                        Currency: blade.currentEntity.currency,
                        Assignments: blade.currentEntity.assignments,
                        Description: blade.currentEntity.description,
                        Vendors: blade.currentEntity.vendors,
                        Prices: blade.currentEntity.Prices

                    }).then(function (data) {
                        blade.refresh(true);
                    });
                }
            };

            $scope.setForm = function (form) { $scope.formScope = form; };

            blade.onClose = function (closeCallback) {
                bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "pricing.dialogs.pricelist-save.title", "pricing.dialogs.pricelist-save.message");
            };

            blade.headIcon = blade.parentBlade.headIcon;

            function initializeToolbar() {
                if (!blade.isNew) {
                    blade.toolbarCommands = [
                        {
                            name: "platform.commands.save",
                            icon: 'fa fa-save',
                            executeMethod: $scope.saveChanges,
                            canExecuteMethod: canSave,
                            permission: blade.updatePermission
                        },
                        {
                            name: "platform.commands.reset",
                            icon: 'fa fa-undo',
                            executeMethod: function () {
                                angular.copy(blade.origEntity, blade.currentEntity);
                            },
                            canExecuteMethod: isDirty,
                            permission: blade.updatePermission
                        }
                    ];
                }
            }

            initializeToolbar();
            blade.refresh(false);
            $scope.currencyUtils = currencyUtils;
        }]);