angular.module('app.ordersTracking').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $stateProvider
    .state('Tracking', {
        url: '/orders',
        templateUrl: '/app/designer/orderTracking/list.html',
        resolve: {
            initialdataofTrackinglist: ['TrackingListFactory', '$q',
                function (TrackingListFactory, $q) {

                    var promises = {
                        vm: TrackingListFactory.getList(),
                        sl: TrackingListFactory.getstatuslist()
                    };

                    return $q.all(promises).then(function (values) {
                        var initdata = {};
                        initdata.viewmodel = values.vm;
                        initdata.statuslist = values.sl;
                        return initdata;
                    });
                }]
        },
        controller: 'TrackingList',
        controllerAs: 'fo'
    })

    .state('TrackingDetail', {
        url: '/orders/trackingDetail/:OrderNumber',
        templateUrl: '/app/designer/orderTracking/detail.html',
        resolve: {
            InitialDataOfOrderDetail: ['OrderDetailFactory', '$q', '$stateParams',
                function (OrderDetailFactory, $q, $stateParams) {
                    var promises = {
                        vm: OrderDetailFactory.getDefaultViewModel($stateParams.OrderNumber),
                    };
                    return $q.all(promises).then(function (values) {
                        var initData = {};
                        initData.viewModel = values.vm;
                        return initData;
                    });
                }]
        },
        controller: 'TrackingDetail',
        controllerAs: 'fo'
    });



}]);