angular.module('app.appointment').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $stateProvider
    .state('Appointments', {
        url: '/appointments',
        templateUrl: '/app/designer/customerAppointments/list.html',
        resolve: {
            InitialDataOfAppointmentList: ['AppointmentListFactory', '$q',
                function (AppointmentListFactory, $q) {

                    var promises = {
                        vm: AppointmentListFactory.getList(),
                        sl: AppointmentListFactory.getStatusList(),
                        dl: AppointmentListFactory.getDateList()
                    };

                    return $q.all(promises).then(function (values) {
                        var initData = {};
                        initData.viewModel = values.vm;
                        initData.statusList = values.sl;
                        initData.dateList = values.dl;
                        return initData;
                    });
                }]
        },
        controller: 'AppointmentList',
        controllerAs: 'fo'
    })

    .state('AppointmentDetail', {
        url: '/appointments/:OrderNumber/:OrderDate',
        templateUrl: '/app/designer/customerAppointments/detail.html',
        resolve: {
            InitialDataOfAppointmentDetail: ['AppointmentDetailFactory', '$q', '$stateParams', 'stackView',
                function (AppointmentDetailFactory, $q, $stateParams, stackView) {
                    var obj = stackView.getLastViewDetail();
                    if (obj.formName !== 'AppointmentDetail') {
                        var promises = {
                            vm: AppointmentDetailFactory.getDefaultViewModel($stateParams.OrderNumber, $stateParams.OrderDate),
                        };

                        return $q.all(promises).then(function (values) {
                            var initData = {};
                            initData.viewModel = values.vm;
                            return initData;
                        });
                    }
                }]
        },
        controller: 'AppointmentDetail',
        controllerAs: 'fo'
    })

     .state('DesignDetails', {
         url: '/appointments/Designs/:Number/:ID',
         templateUrl: '/app/designer/customerAppointments/designDetail.html',
         resolve: {
             InitialDataOfDesignDetail: ['DesignDetailsFactory', '$q', '$stateParams',
                 function (DesignDetailsFactory, $q, $stateParams) {
                     var promises = {
                         vm: DesignDetailsFactory.getDefaultViewModel($stateParams.Number, $stateParams.ID),
                     };

                     return $q.all(promises).then(function (values) {
                         var initData = {
                         };
                         initData.viewModel = values.vm;
                         return initData;
                     });
                 }]
         },
         controller: 'DesignDetails',
         controllerAs: 'fo'
     });
}]);