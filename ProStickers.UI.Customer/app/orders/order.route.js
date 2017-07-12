angular.module('app.order').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('OrdersList', {
           url: '/ordersList',
           templateUrl: 'app/orders/list.html',
           resolve: {
               initialDataOfOrderList: ['orderListFactory', '$q',
                   function (orderListFactory, $q) {
                       var promises = {
                           vm: orderListFactory.geDefaultListViewModel(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });

                   }]
           },
           controller: 'OrdersList',
           controllerAs: 'fo'
       })
       .state('OrdersDetail', {
           url: '/ordersList/ordersDetail/:Number',
           templateUrl: 'app/orders/detail.html',
           resolve: {
               initialDataOfOrderDetail: ['orderDetailFactory', '$q', '$stateParams',
                   function (orderDetailFactory, $q, $stateParams) {
                       var promises = {
                           vm: orderDetailFactory.getDetailByID($stateParams.Number),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });

                   }]
           },
           controller: 'OrdersDetail',
           controllerAs: 'fo'
       })
     .state('OrdersCreate', {
         url: '/ordersList/ordersCreate/:DesignNumber?AppointmentNumber',
         templateUrl: 'app/orders/create.html',
         resolve: {
             initialDataOfCreateOrder: ['orderCreateFactory', '$q', '$stateParams',
                 function (orderCreateFactory, $q, $stateParams) {
                     var promises = {
                         vm: orderCreateFactory.getDefaultViewModel($stateParams.DesignNumber, $stateParams.AppointmentNumber),
                         cl: orderCreateFactory.getCountryList(),
                         yl: orderCreateFactory.getYearList(),
                         ml: orderCreateFactory.getMonthList()
                     };
                     return $q.all(promises).then(function (values) {
                         var initData = {};
                         initData.viewModel = values.vm;
                         initData.countryList = values.cl;
                         initData.yearList = values.yl;
                         initData.monthList = values.ml;
                         return initData;
                     });

                 }]
         },
         controller: 'OrdersCreate',
         controllerAs: 'fo'
     });

}]);