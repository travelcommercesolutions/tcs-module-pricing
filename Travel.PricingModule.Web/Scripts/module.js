// Call this to register your module to main application
var moduleName = "travel.pricingModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('workspace.travelPricingModuleState', {
                    url:'/api/TravelPricingModule',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                            var newBlade = {
                                id: 'blade1',
                                controller: 'travel.pricingModule.helloWorldController',
                                template: 'Modules/$(TCS.PricingModule)/Scripts/blades/hello-world.html',
                                isClosingDisabled: true
                            };
                            bladeNavigationService.showBlade(newBlade);
                        }
                    ]
                });
        }
    ])
    .run(['$rootScope',
        'platformWebApp.mainMenuService',
        'platformWebApp.widgetService',
        'platformWebApp.permissionScopeResolver',
        'tcs.module.myarline.manager.vendorProfileService',
        'platformWebApp.bladeNavigationService',
        'platformWebApp.widgetService',
        '$state',

        function ($rootScope, mainMenuService, widgetService, scopeResolver, vendorProfiles, bladeNavigationService, widgetService, $state) {
            $rootScope.$on('loginStatusChanged', function (event, authContext) {
                if (authContext.isAuthenticated) {
                    var isAdmin = authContext.isAdministrator;
                    var isUserName = authContext.userName;
                    sessionStorage.setItem('isAdmin', isAdmin);
                    sessionStorage.setItem('isUserName', isUserName);
                }
            });

            //Register pricelist widgets
            widgetService.registerWidget({
                isVisible: function (blade) { return !blade.isNew; },
                controller: 'travel.pricingModule.pricesWidgetController',
                template: 'Modules/$(TCS.PricingModule)/Scripts/widgets/pricesWidget.tpl.html',
            }, 'pricelistDetails');
            widgetService.registerWidget({
                controller: 'travel.pricingModule.assignmentsWidgetController',
                template: 'Modules/$(TCS.PricingModule)/Scripts/widgets/assignmentsWidget.tpl.html',
            }, 'pricelistDetails');


            var profileVendorScope = {
                type: 'ProfileVendorScope',
                title: 'Only for profiles in selected vendors',
                selectFn: function (blade, callback) {
                    var newBlade = {
                        id: 'vandor-pick',
                        title: this.title,
                        subtitle: 'Select vendors',
                        currentEntity: this,
                        onChangesConfirmedFn: callback,
                        dataPromise: vendorProfiles.getVendorProfiles(0, 100).then(function (data) { return data.vendorProfiles }),
                        controller: 'platformWebApp.security.scopeValuePickFromSimpleListController',
                        template: '$(Platform)/Scripts/app/security/blades/common/scope-value-pick-from-simple-list.tpl.html'
                    };
                    bladeNavigationService.showBlade(newBlade, blade);
                }
            };
            scopeResolver.register(profileVendorScope);

         }
    ]);
