angular.module('app.coupons').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('CouponsList', {
           url: '/couponsList',
           templateUrl: 'app/master/coupons/list.html',
           resolve: {
               initialDataOfCouponsList: ['CouponsListFactory', '$q',
                   function (CouponsListFactory, $q) {
                       var promises = {
                           vm: CouponsListFactory.getDefaultViewModel(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });
                   }]
           },
           controller: 'CouponsList',
           controllerAs: 'fo'
       })

     .state('CouponsUpdate', {
         url: '/couponsList/updateCoupon/:ID',
         templateUrl: 'app/master/coupons/update.html',
         resolve: {
             initialDataOfCouponsUpdate: ['CouponsUpdateFactory', '$q', '$stateParams',
                 function (CouponsUpdateFactory, $q, $stateParams) {
                     var promises = {
                         vm: CouponsUpdateFactory.getDefaultViewModel($stateParams.ID),
                     };
                     return $q.all(promises).then(function (values) {
                         var initData = {};
                         initData.viewModel = values.vm;
                         return initData;
                     });
                 }]
         },
         controller: 'CouponsUpdate',
         controllerAs: 'fo'
     })

     .state('CouponsCreate', {
         url: '/couponsList/createCoupon',
         templateUrl: 'app/master/coupons/create.html',
         resolve: {
             initialDataOfCouponsCreate: ['CouponsCreateFactory', '$q',
                 function (CouponsCreateFactory, $q) {
                     var promises = {
                         vm: CouponsCreateFactory.getDefaultViewModel(),
                         rl: CouponsCreateFactory.getRoleList(),
                     };
                     return $q.all(promises).then(function (values) {
                         var initData = {};
                         initData.viewModel = values.vm;
                         initData.roleList = values.rl;
                         return initData;
                     });
                 }]
         },
         controller: 'CouponsCreate',
         controllerAs: 'fo'
     });
}]);