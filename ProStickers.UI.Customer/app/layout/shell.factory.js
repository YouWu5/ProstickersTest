(function () {
    'use strict';

    angular
        .module('app.layout')
        .factory('shellFactory', shellFactory);

    shellFactory.$inject = ['$http', '$q', 'appUrl'];

    function shellFactory($http, $q, appUrl) {
        var service = {
            ObtainLocalAccessToken: ObtainLocalAccessToken,
            GetUserSession: GetUserSession,
            LogOut: LogOut,
            submitAgreement: submitAgreement,
            submitHaveSkype: submitHaveSkype,
            CustomerSession: CustomerSession,
            GetDefault: GetDefault
        };

        return service;

        function ObtainLocalAccessToken(model) {
            var def = $q.defer();
            $http.post(appUrl + 'User/Account/ObtainAccessToken', model)
            .then(function (response) {
                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.ObtainLocalAccessToken', error);
                def.reject(error);
            });
            return def.promise;
        }
   
        function submitAgreement(submitModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/Customer/AcceptSignInPolicy', submitModel)
            .then(function (response) {
                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.SUBMIT', error);
                def.reject(error);
            });
            return def.promise;
        }
        
        function submitHaveSkype(submitModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/Customer/HaveSkype', submitModel)
            .then(function (response) {
                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.haveSkype', error);
                def.reject(error);
            });
            return def.promise;
        }   

        function GetDefault() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Customer/Default')
            .then(function (response) {

                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.ObtainLocalAccessToken', error);
                def.reject(error);
            });
            return def.promise;
        }

        function CustomerSession(model) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/Customer/CustomerSession', model).then(function (response) {
                def.resolve(response.data);
            }).catch(function fail(error) {
                console.log('LoginFactory.ObtainLocalAccessToken', error);
                def.reject(error);
            });
            return def.promise;
        }
 
        function GetUserSession(userID, fbID) {
            var def = $q.defer();
            $http.get(appUrl + 'User/Account/' + userID + '/' + fbID + '/UserSession')
            .then(function (response) {

                def.resolve(response.data);

            }).catch(function fail(error) {
                console.log('LoginFactory.ObtainLocalAccessToken', error);
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