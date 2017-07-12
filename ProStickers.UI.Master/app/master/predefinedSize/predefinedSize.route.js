angular.module('app.predefinedSize').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('PredefinedSizeList', {
           url: '/predefinedSizeList',
           templateUrl: 'app/master/predefinedSize/list.html',
           resolve: {
               initialDataOfPredefinedSizeList: ['PredefinedSizeListFactory', '$q',
                   function (PredefinedSizeListFactory, $q) {
                       var promises = {
                           vm: PredefinedSizeListFactory.getDefaultViewModel(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });

                   }]
           },
           controller: 'PredefinedSizeList',
           controllerAs: 'fo'
       });

}]);