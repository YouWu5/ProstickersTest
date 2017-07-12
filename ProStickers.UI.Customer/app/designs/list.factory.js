(function () {
    'use strict';

    angular
        .module('app.design')
        .factory('designListFactory', designListFactory);

    designListFactory.$inject = ['$http', '$q', 'appUrl'];

    function designListFactory($http, $q, appUrl) {
        var service = {
            getDesignList: getDesignList,
            submit: submit
        };

        return service;
 
        function getDesignList() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Design/GetList')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at design', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/Design/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('customerProfileFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }
          
    }
})();