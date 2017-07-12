(function () {
    'use strict';

    angular
        .module('app')
        .factory('DesignDetailsFactory', DesignDetailsFactory);

    DesignDetailsFactory.$inject = ['$http', '$q', 'appUrl'];

    function DesignDetailsFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit
        };

        return service;

        function getDefaultViewModel(Number, ID) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/' + Number + '/' + ID + '/DesignerNote').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('DesignDetailsFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Designer/Appointment/UpdateDesignerNote', viewModel).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('DesignDetailsFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }
    }
})();