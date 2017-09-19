(function () {
    'use strict';

    angular
        .module('app.coupons')
        .factory('CouponsListFactory', CouponsListFactory);

    CouponsListFactory.$inject = ['$http', '$q', 'appUrl'];

    function CouponsListFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            updateActive: updateActive
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/Coupon/GetList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CouponsListFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/Coupon/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('CouponsListFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function updateActive(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/Coupon/Inactive', viewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('CouponsListFactory.updateActive', error);
                def.reject(error);
            });
            return def.promise;
        }

    }
})();