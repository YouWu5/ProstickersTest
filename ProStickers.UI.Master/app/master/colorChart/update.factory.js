(function () {
    'use strict';

    angular
        .module('app.colorChart')
        .factory('ColorChartUpdateFactory', ColorChartUpdateFactory);

    ColorChartUpdateFactory.$inject = ['$http', '$q', 'appUrl'];

    function ColorChartUpdateFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            deleteColor: deleteColor
        };

        return service;

        function getDefaultViewModel(id) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/ColorChart/' + id).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('ColorChartUpdateFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/ColorChart', viewModel)
            .then(function (response) {
                def.resolve(response.data);
            }).catch(function fail(error) {
                console.log('ColorChartUpdateFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function deleteColor(ViewModel) {
            var def = $q.defer();
            $http.delete(appUrl + 'Master/ColorChart/' + ViewModel.ColorID + '/' + ViewModel.Name+ '/' + ViewModel.UpdatedTS + '/DeleteColor').then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('CustomersListFactory.deleteColor', error);
                def.reject(error);
            });
            return def.promise;
        }

    }
})();