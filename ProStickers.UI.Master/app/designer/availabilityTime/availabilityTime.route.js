angular.module('app.availabilityTime').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $stateProvider
    .state('AvailabilityTime', {
        url: '/availabilityTime',
        templateUrl: '/app/designer/availabilityTime/Availability.html',
        controller: 'AvailabilityTime',
        controllerAs: 'fo'
    });
}]);