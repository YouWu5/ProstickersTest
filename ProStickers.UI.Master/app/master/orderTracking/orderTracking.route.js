angular.module('app.orderTracking').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('OrderTrackingList', {
           url: '/orderTrackingList',
           templateUrl: 'app/master/orderTracking/list.html',
           resolve: {
               initialDataOfOrderTrackingList: ['OrderTrackingListFactory', '$q',
                   function (OrderTrackingListFactory, $q) {
                       var promises = {
                           vm: OrderTrackingListFactory.getDefaultViewModel(),
                           sl: OrderTrackingListFactory.getStatusList(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           initData.statusList = values.sl;
                           return initData;
                       });
                   }]
           },
           controller: 'OrderTrackingList',
           controllerAs: 'fo'
       })

      .state('OrderTrackingUpdate', {
          url: '/orderTrackingList/orderTrackingupdate/:OrderNo',
          templateUrl: 'app/master/orderTracking/update.html',
          resolve: {
              initialDataOfOrderTrackingUpdate: ['OrderTrackingUpdateFactory', '$q', '$stateParams',
                  function (OrderTrackingUpdateFactory, $q, $stateParams) {
                      var promises = {
                          vm: OrderTrackingUpdateFactory.getDefaultViewModel($stateParams.OrderNo),
                      };
                      return $q.all(promises).then(function (values) {
                          var initData = {};
                          initData.viewModel = values.vm;
                          return initData;
                      });
                  }]
          },
          controller: 'OrderTrackingUpdate',
          controllerAs: 'fo'
      });
}]);