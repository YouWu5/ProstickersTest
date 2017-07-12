(function () {
    'use strict';

    angular
        .module('app.order')
        .factory('orderListFactory', orderListFactory);

    orderListFactory.$inject = ['$http', '$q', 'appUrl'];

    function orderListFactory($http, $q, appUrl) {
        var service = {
            geDefaultListViewModel: geDefaultListViewModel,
            submit: submit
        };

        return service;

        function geDefaultListViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Order/GetList')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at design', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/Order/List', ViewModel).then(function (response) {
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