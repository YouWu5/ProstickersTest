angular.module('app.appointment').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('AppointmentList', {
           url: '/appointmentList',
           templateUrl: 'app/appointment/list.html',
           resolve: {
               initialDataOfAppointmentList: ['appointmentListFactory', '$q',
                   function (appointmentListFactory, $q) {

                       var promises = {
                           vm: appointmentListFactory.getDefaultViewModel(),
                           al: appointmentListFactory.getAppointmentList(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           initData.appointmentListViewModel = values.al;
                           console.log("appointment.route.js-state(AptmntList)-initData",initData)
                           return initData;
                       });

                   }]
           },
           controller: 'AppointmentList',
           controllerAs: 'fo'
       })
       .state('AppointmentDetail', {
           url: '/appointmentList/appointmentDetail/:Number',
           templateUrl: 'app/appointment/detail.html',
           resolve: {
               initialDataOfAppointmentDetail: ['appointmentDetailFactory', 'stackView', '$q', '$stateParams', function (appointmentDetailFactory, stackView, $q, $stateParams) {
                   console.log('$stateParams', $stateParams);
                   var obj = stackView.getLastViewDetail();
                   if (obj.formName !== 'AppointmentDetail') {
                       var promises = {
                           vm: appointmentDetailFactory.getAppointmentDetail($stateParams.Number),
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
        .state('AppointmentCalendar', {
            url: '/appointmentList/appointmentCalendar',
            templateUrl: 'app/appointment/calendar.html',
            resolve: {
                initialDataOfAddCalendar: ['addCalendarFactory', '$q','helper',
                    function (addCalendarFactory, $q, helper) {
                        var today = helper.formatDate(new Date());
                        var promises = {
                            vm: addCalendarFactory.getDefaultViewModel(),
                            tsl: addCalendarFactory.getTimeSlotList(today)
                        };
                        return $q.all(promises).then(function (values) {
                            var initData = {};
                            initData.viewModel = values.vm;
                            initData.timeSlotList = values.tsl;
                            return initData;
                        });

                    }]
            },
            controller: 'AppointmentCalendar',
            controllerAs: 'fo'
        });

}]);