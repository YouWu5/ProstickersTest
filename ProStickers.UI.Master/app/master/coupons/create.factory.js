(function () {
    'use strict';

    angular
        .module('app.coupons')
        .factory('CouponsCreateFactory', CouponsCreateFactory);

    CouponsCreateFactory.$inject = ['$http', '$q', 'appUrl'];

    function CouponsCreateFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            getTypeList: getTypeList
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/Coupon/Default').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CouponsCreateFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/Coupon', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('CouponsCreateFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function getTypeList() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/Coupon/CouponTypeList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CouponsCreateFactory.getTypeList', error);
                 def.reject(error);
             });
            return def.promise;
        }

    }
})();