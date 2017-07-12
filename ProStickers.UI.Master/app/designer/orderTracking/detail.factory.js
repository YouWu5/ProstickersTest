(function () {
    'use strict';

    angular
        .module('app.ordersTracking')
        .factory('OrderDetailFactory', OrderDetailFactory);

    OrderDetailFactory.$inject = ['$http', '$q', 'appUrl', 'spinnerService'];

    function OrderDetailFactory($http, $q, appUrl, spinnerService) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            downloadFile: downloadFile,
            submit: submit
        };

        return service;

        function getDefaultViewModel(ID) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/OrderTracking/' + ID + '/GetByID').then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('OrderDetailFactory.getDefaultViewModel', error);
                    def.reject(error);
                });
            return def.promise;
        }

        function downloadFile(Number) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/OrderTracking/' + Number + '/Download').then(function (response) {
                def.resolve(response.data);
                spinnerService.hide('pageContainerSpinner');
            })
                .catch(function fail(error) {
                    console.log('OrderDetailFactory.downloadFile', error);
                    def.reject(error);
                    spinnerService.hide('pageContainerSpinner');
                });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Designer/OrderTracking', viewModel).then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('OrderDetailFactory.submit', error);
                    def.reject(error);
                });
            return def.promise;
        }
    }
})();