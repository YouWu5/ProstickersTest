(function () {
    'use strict';

    angular
        .module('app.colorChart')
        .factory('ColorChartCreateFactory', ColorChartCreateFactory);

    ColorChartCreateFactory.$inject = ['$http', '$q', 'appUrl'];

    function ColorChartCreateFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/ColorChart/Default').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('ColorChartCreateFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/ColorChart', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('ColorChartCreateFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

    }
})();