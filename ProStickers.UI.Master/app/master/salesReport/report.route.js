angular.module('app.report').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('SalesReportList', {
           url: '/report',
           templateUrl: 'app/master/salesReport/list.html',
           resolve: {
               initialDataOfSalesReportList: ['SalesReportListFactory', '$q',
                   function (SalesReportListFactory, $q) {
                       var promises = {
                           vm: SalesReportListFactory.getDefaultViewModel(),
                           dl: SalesReportListFactory.getDesignerList(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           initData.designerList = values.dl;
                           return initData;
                       });

                   }]
           },
           controller: 'SalesReportList',
           controllerAs: 'fo'
       });
}]);