(function () {
    'use strict';

    angular
        .module('app.ordersTracking')
        .factory('TrackingListFactory', TrackingListFactory);

    TrackingListFactory.$inject = ['$http', '$q', 'appUrl'];

    function TrackingListFactory($http, $q, appUrl) {
        var service = {
            getList: getList,
            getstatuslist: getstatuslist,
            getCustomerList: getCustomerList,
            submit: submit
        };

        return service;

        function getList() {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/OrderTracking/GetList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('TrackingListFactory.getList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getstatuslist() {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/OrderTracking/GetStatusList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('TrackingListFactory.GetStatusList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getCustomerList(name) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/OrderTracking/' + name + '/GetCustomerNameList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('OrderTrackingListFactory.getCustomerList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Designer/OrderTracking/List', viewModel).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('TrackingListFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }
    }
})();