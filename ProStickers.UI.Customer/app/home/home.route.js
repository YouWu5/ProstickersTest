
angular.module('app.home').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider
         .state('/', {
             url: '',
             templateUrl: 'app/home/home.html',
             //resolve: {
             //    initialDataOfCustomerHome: ['customerHomeFactory', '$q', function (customerHomeFactory, $q) {
             //        var promises = {
             //            al: customerHomeFactory.getDetailLists(),
             //        };
             //        return $q.all(promises).then(function (values) {
             //            var initData = {};
             //            initData.detailList = values.al;
             //            return initData;
             //        });

             //    }]
             //},
         })
        .state('404', {
            url: '/404',
            templateUrl: '/app/home/404.html'
        })
    .state('serviceTerms', {
        url: '/serviceterms',
        templateUrl: '/app/home/serviceTerms.html'
    })
    .state('privacyPolicy', {
        url: '/privacypolicy',
        templateUrl: '/app/home/privacyPolicy.html'
    })
        .state('CustomerHome', {
            url: '/customerHome',
            templateUrl: 'app/home/customerHome.html',
            resolve: {
                initialDataOfCustomerHome: ['customerHomeFactory', '$q', function (customerHomeFactory, $q) {
                    var promises = {
                        al: customerHomeFactory.getDetailLists(),
                    };
                    return $q.all(promises).then(function (values) {
                        var initData = {};
                        initData.detailList = values.al;
                        return initData;
                    });

                }]
            },
            controller: 'CustomerHome',
            controllerAs: 'fo'
        });
}]);