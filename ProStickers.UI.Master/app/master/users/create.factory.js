(function () {
    'use strict';

    angular
        .module('app.users')
        .factory('UsersCreateFactory', UsersCreateFactory);

    UsersCreateFactory.$inject = ['$http', '$q', 'appUrl'];

    function UsersCreateFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            getRoleList: getRoleList
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/User/Default').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('UsersCreateFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/User', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('UsersCreateFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function getRoleList() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/User/UserTypeList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('UsersCreateFactory.getRoleList', error);
                 def.reject(error);
             });
            return def.promise;
        }

    }
})();