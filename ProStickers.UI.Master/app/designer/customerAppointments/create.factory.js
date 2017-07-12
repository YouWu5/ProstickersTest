(function () {
    'use strict';

    angular
        .module('app.appointment')
        .factory('AppointmentCreateFactory', AppointmentCreateFactory);

    AppointmentCreateFactory.$inject = ['$http', '$q', 'appUrl'];

    function AppointmentCreateFactory($http, $q, appUrl) {
        var service = {
            getDefault: getDefault,
            getSlotList: getSlotList,
            getAppointmentRequestCount: getAppointmentRequestCount,
            cancelRequest: cancelRequest,
            submit: submit
        };

        return service;

        function getDefault() {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/AppointmentRequest').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentCreateFactory.getDefault', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getSlotList(date) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/' + date + '/AppointmentSlotList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentCreateFactory.getDefault', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getAppointmentRequestCount() {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/AppointmentRequestCount').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentCreateFactory.getAppointmentRequestCount', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Designer/Appointment', viewModel).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentCreateFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function cancelRequest(viewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Designer/Appointment/CancelAppointmentRequest', viewModel).then(function (response) {
                def.resolve(response.data);
            })
           .catch(function fail(error) {
               console.log('AppointmentCreateFactory.submit', error);
               def.reject(error);
           });
            return def.promise;
        }
    }
})();