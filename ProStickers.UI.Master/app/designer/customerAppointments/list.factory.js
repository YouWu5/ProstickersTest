(function () {
    'use strict';

    angular
        .module('app.appointment')
        .factory('AppointmentListFactory', AppointmentListFactory);

    AppointmentListFactory.$inject = ['$http', '$q', 'appUrl'];

    function AppointmentListFactory($http, $q, appUrl) {
        var service = {
            getList: getList,
            getAppointmentList:getAppointmentList,
            getStatusList: getStatusList,
            getDateList: getDateList,
            submit: submit
        };

        return service;
        
        function getList() {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/GetList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentListFactory.getList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getAppointmentList() {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/GetAppointmentList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentListFactory.getAppointmentList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getStatusList() {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/StatusList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentListFactory.GetStatusList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getDateList() {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/DateList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentListFactory.GetDateList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Designer/Appointment/List', viewModel).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentListFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }
    }
})();