(function () {
    'use strict';

    angular
        .module('app.customers')
        .factory('CustomersListFactory', CustomersListFactory);

    CustomersListFactory.$inject = ['$http', '$q', 'appUrl'];

    function CustomersListFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            getCustomerList: getCustomerList
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/List').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CustomersListFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/CustomerDetail/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('CustomersListFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function getCustomerList(codeName) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/' + codeName + '/CustomerList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CustomersListFactory.getCustomerList', error);
                 def.reject(error);
             });
            return def.promise;
        }

    }
})();