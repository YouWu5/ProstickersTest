(function () {
    'use strict';

    angular
        .module('app.users')
        .factory('UsersUpdateFactory', UsersUpdateFactory);

    UsersUpdateFactory.$inject = ['$http', '$q', 'appUrl'];

    function UsersUpdateFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit
        };

        return service;

        function getDefaultViewModel(id) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/User/' + id + '/GetByID').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('UsersUpdateFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/User', viewModel)
            .then(function (response) {
                def.resolve(response.data);
            }).catch(function fail(error) {
                console.log('UsersUpdateFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

    }
})();