(function () {
    'use strict';

    angular
        .module('app.users')
        .factory('UsersListFactory', UsersListFactory);

    UsersListFactory.$inject = ['$http', '$q', 'appUrl'];

    function UsersListFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            updateActive: updateActive
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/User/GetList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('UsersListFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/User/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('UsersListFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function updateActive(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/User/Inactive', viewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('UsersListFactory.updateActive', error);
                def.reject(error);
            });
            return def.promise;
        }

    }
})();