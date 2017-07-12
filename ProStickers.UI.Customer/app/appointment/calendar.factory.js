(function () {
    'use strict';

    angular
        .module('app.appointment')
        .factory('addCalendarFactory', addCalendarFactory);

    addCalendarFactory.$inject = ['$http', '$q', 'appUrl'];

    function addCalendarFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            getTimeSlotList: getTimeSlotList,
            getAvailableDesignerList: getAvailableDesignerList,
            getAvailableDesigner: getAvailableDesigner,
            submit: submit
        };

        return service;
       
        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/CustomerAppointment/GetDefault')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at calendar 212', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function getTimeSlotList(currentDate) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/CustomerAppointment/' + currentDate)
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at calendar kya aayi', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function getAvailableDesignerList(date, timeslotID) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/CustomerAppointment/'+ date + '/' + timeslotID + '/AvailableDesignerList')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at calendar', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function getAvailableDesigner(date, timeslotID) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/CustomerAppointment/' +date + '/' + timeslotID)
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at calendar', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/CustomerAppointment', viewModel)
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at calendar', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

    }
})();