(function () {
    'use strict';

    angular
        .module('app.orderTracking')
        .factory('OrderTrackingListFactory', OrderTrackingListFactory);

    OrderTrackingListFactory.$inject = ['$http', '$q', 'appUrl'];

    function OrderTrackingListFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            getCustomerList: getCustomerList,
            getStatusList: getStatusList
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/OrderTracking/GetList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('OrderTrackingListFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/OrderTracking/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('OrderTrackingListFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function getCustomerList(name) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/OrderTracking/' + name + '/GetCustomerNameList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('OrderTrackingListFactory.getCustomerList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getStatusList() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/OrderTracking/GetStatusList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('OrderTrackingListFactory.getStatusList', error);
                 def.reject(error);
             });
            return def.promise;
        }

    }
})();