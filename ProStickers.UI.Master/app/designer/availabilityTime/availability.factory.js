(function () {
    'use strict';

    angular
        .module('app.availabilityTime')
        .factory('AvailabilityTimeFactory', AvailabilityTimeFactory);

    AvailabilityTimeFactory.$inject = ['$http', '$q', 'appUrl'];

    function AvailabilityTimeFactory($http, $q, appUrl) {
        var service = {
            getList: getList,
            submit: submit
        };

        return service;
   
        function getList(date, IsAll) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/DesignerTimeSlot/' + date + '/' + IsAll + '/GetList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AvailabilityTimeFactory.getList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Designer/DesignerTimeSlot', viewModel).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AvailabilityTimeFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }
    }
})();