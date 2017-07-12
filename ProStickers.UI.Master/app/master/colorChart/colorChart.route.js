angular.module('app.colorChart').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('ColorChartList', {
           url: '/colorChartList',
           templateUrl: 'app/master/colorChart/list.html',
           resolve: {
               initialDataOfColorChartList: ['ColorChartListFactory', '$q',
                   function (ColorChartListFactory, $q) {
                       var promises = {
                           vm: ColorChartListFactory.getDefaultViewModel(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });

                   }]
           },
           controller: 'ColorChartList',
           controllerAs: 'fo'
       })

     .state('ColorChartUpdate', {
         url: '/colorChartList/colorChartUpdate/:ID',
         templateUrl: 'app/master/colorChart/update.html',
         resolve: {
             initialDataOfColorChartUpdate: ['ColorChartUpdateFactory', '$q', '$stateParams',
                 function (ColorChartUpdateFactory, $q, $stateParams) {
                     var promises = {
                         vm: ColorChartUpdateFactory.getDefaultViewModel($stateParams.ID),
                     };
                     return $q.all(promises).then(function (values) {
                         var initData = {};
                         initData.viewModel = values.vm;
                         return initData;
                     });

                 }]
         },
         controller: 'ColorChartUpdate',
         controllerAs: 'fo'
     })

    .state('ColorChartCreate', {
        url: '/colorChartList/colorChartCreate',
        templateUrl: 'app/master/colorChart/create.html',
        resolve: {
            initialDataOfColorChartCreate: ['ColorChartCreateFactory', '$q',
                function (ColorChartCreateFactory, $q) {
                    var promises = {
                        vm: ColorChartCreateFactory.getDefaultViewModel(),
                    };
                    return $q.all(promises).then(function (values) {
                        var initData = {};
                        initData.viewModel = values.vm;
                        return initData;
                    });
                }]
        },
        controller: 'ColorChartCreate',
        controllerAs: 'fo'
    });
}]);