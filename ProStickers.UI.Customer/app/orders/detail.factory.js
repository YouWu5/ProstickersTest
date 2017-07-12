(function () {
    'use strict';

    angular
        .module('app.order')
        .factory('orderDetailFactory', orderDetailFactory);

    orderDetailFactory.$inject = ['$http', '$q', 'appUrl'];

    function orderDetailFactory($http, $q, appUrl) {
        var service = {
            getDetailByID: getDetailByID,
          
        };

        return service;

        function getDetailByID(orderNo) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Order/' + orderNo + '/GetByID')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at orderNo', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }
 

    }
})();