(function () {
    'use strict';

    angular
        .module('app.appointment')
        .factory('appointmentDetailFactory', appointmentDetailFactory);

    appointmentDetailFactory.$inject = ['$http', '$q', 'appUrl'];

    function appointmentDetailFactory($http, $q, appUrl) {
        var service = {
            getAppointmentDetail: getAppointmentDetail,
            submit: submit
        };

        return service;

        function getAppointmentDetail(appointmentNo) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/CustomerAppointment/' + appointmentNo + '/GetByID')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at appoinmtment detail', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }



        function submit(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Customer/CustomerAppointment', viewModel)
             .then(function (response) {
                 def.resolve(response.data);
             }).catch(function fail(error) {
                 console.log('customerProfileFactory12121.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }

    }
})();