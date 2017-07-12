angular.module('app.customers').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('CustomersList', {
           url: '/customerslist',
           templateUrl: 'app/master/customers/list.html',
           resolve: {
               initialDataOfCustomersList: ['CustomersListFactory', '$q',
                   function (CustomersListFactory, $q) {
                       var promises = {
                           vm: CustomersListFactory.getDefaultViewModel(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });
                   }]
           },
           controller: 'CustomersList',
           controllerAs: 'fo'
       })

     .state('CustomersDetail', {
         url: '/customerslist/customersdetail/:ID',
         templateUrl: 'app/master/customers/detail.html',
         resolve: {
             initialDataOfCustomersDetail: ['CustomersDetailFactory', '$q', '$stateParams', 'stackView',
                 function (CustomersDetailFactory, $q, $stateParams, stackView) {
                     var obj = stackView.getLastViewDetail();
                     if (obj.formName !== 'CustomersDetail') {
                         var promises = {
                             vm: CustomersDetailFactory.getDefaultViewModel($stateParams.ID),
                             cl: CustomersDetailFactory.getCountryList(),
                         };
                         return $q.all(promises).then(function (values) {
                             var initData = {};
                             initData.viewModel = values.vm;
                             initData.countryList = values.cl;
                             return initData;
                         });
                     }
                 }]
         },
         controller: 'CustomersDetail',
         controllerAs: 'fo'
     })

     .state('DesignDetail', {
         url: '/customerslist/customersdetail/designdetail/:DesignID/:UserID',
         templateUrl: 'app/master/customers/designDetail.html',
         resolve: {
             initialDataOfCustomersDesignDetail: ['DesignDetailFactory', '$q', '$stateParams',
                 function (DesignDetailFactory, $q, $stateParams) {
                     var promises = {
                         vm: DesignDetailFactory.getDefaultViewModel($stateParams.DesignID, $stateParams.UserID),
                     };
                     return $q.all(promises).then(function (values) {
                         var initData = {};
                         initData.viewModel = values.vm;
                         return initData;
                     });
                 }]
         },
         controller: 'DesignDetail',
         controllerAs: 'fo'
     });
}]);