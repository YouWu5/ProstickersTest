(function () {
    'use strict';

    angular
        .module('app.appointment')
        .factory('appointmentListFactory', appointmentListFactory);

    appointmentListFactory.$inject = ['$http', '$q', 'appUrl'];

    function appointmentListFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            getAppointmentList: getAppointmentList,
            contactMe: contactMe,
            submit: submit
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/CustomerAppointment/Default')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at appoinmtment', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }


        function getAppointmentList() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/CustomerAppointment/GetList')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response appoinmtment check list', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function contactMe(viewModel) {
            console.log('viewModel', viewModel);
            var def = $q.defer();
            $http.post(appUrl + 'Customer/CustomerAppointment/CallRequestCreate', viewModel)
             .then(function (response) {
                 def.resolve(response.data);
             }).catch(function fail(error) {
                 console.log('customerProfileFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/CustomerAppointment/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('customerProfileFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }



    }
})();