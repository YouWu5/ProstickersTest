(function () {
    'use strict';

    angular
        .module('app.appointment')
        .factory('AppointmentDetailFactory', AppointmentDetailFactory);

    AppointmentDetailFactory.$inject = ['$http', '$q', 'appUrl', 'spinnerService'];

    function AppointmentDetailFactory($http, $q, appUrl, spinnerService) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            uploadImage: uploadImage,
            downloadOtherFile: downloadOtherFile,
            downloadDesignImage: downloadDesignImage,
            downloadVectorFile: downloadVectorFile,
            downloadFile: downloadFile,
            updateStatus: updateStatus,
            submit: submit
        };

        return service;

        function getDefaultViewModel(Number, Date) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/' + Number + '/' + Date).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentDetailFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function uploadImage(ImageModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Designer/Appointment/UploadUserFile', ImageModel).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentDetailFactory.uploadImage', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function downloadOtherFile(Number) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/' + Number + '/DownloadDesignerAppointmentFile').then(function (response) {
                def.resolve(response.data);
                spinnerService.hide('pageContainerSpinner');
            })
             .catch(function fail(error) {
                 console.log('AppointmentDetailFactory.downloadOtherFile', error);
                 def.reject(error);
                 spinnerService.hide('pageContainerSpinner');
             });
            return def.promise;
        }

        function downloadDesignImage(Number) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/' + Number + '/DownloadDesignImage').then(function (response) {
                def.resolve(response.data);
                spinnerService.hide('pageContainerSpinner');
            })
             .catch(function fail(error) {
                 console.log('AppointmentDetailFactory.downloadDesignImage', error);
                 def.reject(error);
                 spinnerService.hide('pageContainerSpinner');
             });
            return def.promise;
        }

        function downloadVectorFile(Number) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/' + Number + '/DownloadVectorFile').then(function (response) {
                def.resolve(response.data);
                spinnerService.hide('pageContainerSpinner');
            })
             .catch(function fail(error) {
                 console.log('AppointmentDetailFactory.downloadVectorFile', error);
                 def.reject(error);
                 spinnerService.hide('pageContainerSpinner');
             });
            return def.promise;

        }

        function downloadFile(Number) {
            var def = $q.defer();
            $http.get(appUrl + 'Designer/Appointment/' + Number + '/DownloadFile').then(function (response) {
                def.resolve(response.data);
                spinnerService.hide('pageContainerSpinner');
            })
             .catch(function fail(error) {
                 console.log('AppointmentDetailFactory.downloadFile', error);
                 def.reject(error);
                 spinnerService.hide('pageContainerSpinner');
             });
            return def.promise;
        }

        function updateStatus(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Designer/Appointment/UpdateAppointmentStatus', viewModel).then(function (response) {
                def.resolve(response.data);
            })
           .catch(function fail(error) {
               console.log('AppointmentDetailFactory.updateStatus', error);
               def.reject(error);
           });
            return def.promise;
        }

        function submit(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Designer/Appointment', viewModel).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('AppointmentDetailFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }
    }
})();