
angular.module('app.home').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider
         .state('/', {
             url: '',
             templateUrl: '/app/home/home.html',
         })
        .state('404', {
            url: '/404',
            templateUrl: '/app/home/404.html',
            controller: 'homeController',
            controllerAs: 'fo'
        })
    .state('serviceTerms', {
            url: '/serviceterms',
            templateUrl: '/app/home/serviceTerms.html'
        })
    .state('privacyPolicy', {
            url: '/privacypolicy',
            templateUrl: '/app/home/privacyPolicy.html'
        });
    }]);