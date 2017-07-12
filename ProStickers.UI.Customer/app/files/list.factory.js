(function () {
    'use strict';

    angular
        .module('app.file')
        .factory('filesListFactory', filesListFactory);

    filesListFactory.$inject = ['$http', '$q', 'appUrl', 'spinnerService'];

    function filesListFactory($http, $q, appUrl, spinnerService) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            getFilesList: getFilesList,
            downloadFile: downloadFile,
            uploadFile: uploadFile,
            deleteFile: deleteFile,
            submit: submit
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Files/Default')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at files list', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function getFilesList() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Files')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at files list', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function downloadFile(fileNo) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Files/' + fileNo + '/DownloadFile')
            .then(function (response) {
                def.resolve(response.data);
                spinnerService.hide('pageContainerSpinner');
                console.log('Data response at files list', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 spinnerService.hide('pageContainerSpinner');
                 def.reject(error);
             });

            return def.promise;
        }

        function deleteFile(fileNo) {
            var def = $q.defer();
            $http.delete(appUrl + 'Customer/Files/' + fileNo +'/DeleteFile')
            .then(function (response) {
                def.resolve(response.data);
                spinnerService.hide('pageContainerSpinner');
                console.log('Data response at files list', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 spinnerService.hide('pageContainerSpinner');
                 def.reject(error);
             });

            return def.promise;
        }

        function uploadFile(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/Files', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('customerProfileFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/Files/Create', ViewModel).then(function (response) {
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