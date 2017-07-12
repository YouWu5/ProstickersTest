angular.module('app.design').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('DesignsList', {
           url: '/designsList',
           templateUrl: 'app/designs/list.html',
           resolve: {
               initialDataOfDesignList: ['designListFactory', '$q',
                   function (designListFactory, $q) {
                       var promises = {
                           vm: designListFactory.getDesignList(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });

                   }]
           },
           controller: 'DesignsList',
           controllerAs: 'fo'
       })
       .state('DesignsDetail', {
           url: '/designsList/designsDetail/:Number',
           templateUrl: 'app/designs/detail.html',
           resolve: {
               initialDataOfDesignDetail: ['designDetailFactory', '$q', '$stateParams', 'stackView',
                   function (designDetailFactory, $q, $stateParams, stackView) {
                       var obj = stackView.getLastViewDetail();
                       if (obj.formName !== 'DesignsDetail') {
                           var promises = {
                               vm: designDetailFactory.getDesignDetail($stateParams.Number),
                           };
                           return $q.all(promises).then(function (values) {
                               var initData = {};
                               initData.viewModel = values.vm;
                               return initData;
                           });
                       }
                   }]
           },
           controller: 'DesignsDetail',
           controllerAs: 'fo'
       });

}]);