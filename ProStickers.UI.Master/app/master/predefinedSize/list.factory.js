(function () {
    'use strict';

    angular
        .module('app.predefinedSize')
        .factory('PredefinedSizeListFactory', PredefinedSizeListFactory);

    PredefinedSizeListFactory.$inject = ['$http', '$q', 'appUrl'];

    function PredefinedSizeListFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/PredefinedSize').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('PredefinedSizeListFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/PredefinedSize', viewModel)
            .then(function (response) {
                def.resolve(response.data);
            }).catch(function fail(error) {
                console.log('PredefinedSizeUpdateFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

    }
})();