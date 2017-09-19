(function () {
    'use strict';

    angular
        .module('app.coupons')
        .factory('CouponsUpdateFactory', CouponsUpdateFactory);

    CouponsUpdateFactory.$inject = ['$http', '$q', 'appUrl'];

    function CouponsUpdateFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit
        };

        return service;

        function getDefaultViewModel(id) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/Coupon/' + id + '/GetByID').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CouponsUpdateFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/Coupon', viewModel)
            .then(function (response) {
                def.resolve(response.data);
            }).catch(function fail(error) {
                console.log('CouponsUpdateFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

    }
})();