(function () {
    'use strict';

    angular
        .module('app.customers')
        .factory('DesignDetailFactory', DesignDetailFactory);

    DesignDetailFactory.$inject = ['$http', '$q', 'appUrl'];

    function DesignDetailFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit
        };

        return service;

        function getDefaultViewModel(designNumber, userID) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/' + designNumber + '/' + userID + '/DesignerNote').then(function (response) {
                def.resolve(response.data);
            })
           .catch(function fail(error) {
               console.log('DesignDetailFactory.getDefaultViewModel', error);
               def.reject(error);
           });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/CustomerDetail/UpdateDesignerNote', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('DesignDetailFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }
    }
})();