angular.module('app.profile').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('CustomerProfile', {
           url: '/customerProfile',
           templateUrl: 'app/customerProfile/customerProfile.html',
           resolve: {
               initialDataOfCustomerProfile: ['customerProfileFactory', '$q', 'localStorageService',
                   function (customerProfileFactory, $q, localStorageService) {
                       var userID = localStorageService.get('UserSession');
                       console.log('userID', userID);
                       var promises = {
                           vm: customerProfileFactory.getDefaultViewModel(),
                           cl: customerProfileFactory.getCountryList(),
                           pd: customerProfileFactory.getProfileDetailByID(userID.UserID)
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           initData.countryList = values.cl;
                           initData.profileDetail = values.pd;
                           return initData;
                       });

                   }]
           },
           controller: 'CustomerProfile',
           controllerAs: 'fo'
       });

}]);