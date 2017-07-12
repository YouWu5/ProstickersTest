(function () {
    'use strict';

    angular
        .module('app.orderTracking')
        .factory('OrderTrackingUpdateFactory', OrderTrackingUpdateFactory);

    OrderTrackingUpdateFactory.$inject = ['$http', '$q', 'appUrl'];

    function OrderTrackingUpdateFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            downloadFile: downloadFile
        };

        return service;

        function getDefaultViewModel(orderNo) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/OrderTracking/' + orderNo + '/GetByID').then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('OrderTrackingUpdateFactory.getDefaultViewModel', error);
                    def.reject(error);
                });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/OrderTracking', ViewModel).then(function (response) {
                def.resolve(response.data);
            }).catch(function fail(error) {
                console.log('OrderTrackingUpdateFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function downloadFile(designNumber) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/OrderTracking/' + designNumber + '/Download').then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('OrderTrackingUpdateFactory.downloadFile', error);
                    def.reject(error);
                });
            return def.promise;
        }

    }
})();