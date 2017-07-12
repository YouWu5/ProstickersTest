(function () {
    'use strict';

    angular
        .module('app.layout')
        .factory('shellFactory', shellFactory);

    shellFactory.$inject = ['$http', '$q', 'appUrl'];

    function shellFactory($http, $q, appUrl) {
        var service = {
            ObtainAccessToken: ObtainAccessToken,
            GetUserSession: GetUserSession,
            LogOut: LogOut,
            submit: submit
        };

        return service;

        function ObtainAccessToken(viewmodel) {
            var def = $q.defer();
            $http.post(appUrl + 'User/Account/ObtainAccessToken', viewmodel)
            .then(function (response) {
                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.ObtainLocalAccessToken', error);
                def.reject(error);
            });
            return def.promise;
        }

        function submit(UserID, SourceID) {
            var def = $q.defer();
            $http.post(appUrl + 'User/Account/' + UserID + '/' + SourceID + '/AcceptSignInPolicy')
            .then(function (response) {
                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.SUBMIT', error);
                def.reject(error);
            });
            return def.promise;
        }

        function GetUserSession(userID, loginID) {
            var def = $q.defer();
            $http.get(appUrl + 'User/Account/' + userID + '/' + loginID + '/UserSession')
            .then(function (response) {

                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.GetUserSession', error);
                def.reject(error);
            });
            return def.promise;
        }

        function LogOut() {
            var def = $q.defer();
            $http.post(appUrl + 'User/Account/LogOut')
            .then(function (response) {

                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.ObtainLocalAccessToken', error);
                def.reject(error);
            });
            return def.promise;
        }
    }
})();