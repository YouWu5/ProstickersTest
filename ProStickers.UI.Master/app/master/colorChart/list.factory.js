(function () {
    'use strict';

    angular
        .module('app.colorChart')
        .factory('ColorChartListFactory', ColorChartListFactory);

    ColorChartListFactory.$inject = ['$http', '$q', 'appUrl'];

    function ColorChartListFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            update: update,
            deleteColor: deleteColor
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/ColorChart/List').then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('ColorChartListFactory.getDefaultViewModel', error);
                    def.reject(error);
                });
            return def.promise;
        }

        function update(ViewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/ColorChart', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('CustomersListFactory.update', error);
                    def.reject(error);
                });
            return def.promise;
        }

        function deleteColor(ViewModel) {
            var def = $q.defer();
            $http.delete(appUrl + 'Master/ColorChart/' + ViewModel.ColorID + '/' + ViewModel.Name + '/' + ViewModel.UpdatedTS + '/DeleteColor').then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('CustomersListFactory.deleteColor', error);
                    def.reject(error);
                });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/ColorChart/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('CustomersListFactory.submit', error);
                    def.reject(error);
                });
            return def.promise;
        }
    }
})();