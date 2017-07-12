(function () {
    'use strict';

    angular
        .module('app.home')
        .factory('customerHomeFactory', customerHomeFactory);

    customerHomeFactory.$inject = ['$http', '$q', 'appUrl'];

    function customerHomeFactory($http, $q, appUrl) {
        var service = {
            getDetailLists: getDetailLists,
            contactMe: contactMe
        };

        return service;

        function getDetailLists() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Customer/DetailList')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response detail profile', response.data);
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


    }
})();