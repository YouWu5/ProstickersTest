(function () {
    'use strict';

    angular
        .module('app.design')
        .factory('designDetailFactory', designDetailFactory);

    designDetailFactory.$inject = ['$http', '$q', 'appUrl'];

    function designDetailFactory($http, $q, appUrl) {
        var service = {
            getDesignDetail: getDesignDetail,
            deleteDesign: deleteDesign,
            downloadFile: downloadFile
        };

        return service;

        function getDesignDetail(designNo) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Design/' + designNo + '/GetByID')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at design detail', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function deleteDesign(designNo, updatedTS) {
            var def = $q.defer();
            $http.put(appUrl + 'Customer/Design/' + designNo + '/' + updatedTS + '/DeleteDesign')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at design detail', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function downloadFile(designNo) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Design/' + designNo + '/DownloadVectorFile')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at design detail', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

    }
})();