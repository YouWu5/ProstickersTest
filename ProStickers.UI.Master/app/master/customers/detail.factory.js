(function () {
    'use strict';

    angular
        .module('app.customers')
        .factory('CustomersDetailFactory', CustomersDetailFactory);

    CustomersDetailFactory.$inject = ['$http', '$q', 'appUrl', 'spinnerService'];

    function CustomersDetailFactory($http, $q, appUrl, spinnerService) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            deleteCustomer: deleteCustomer,
            getCountryList: getCountryList,
            getStateList: getStateList,
            downloadVectorFile: downloadVectorFile,
            downloadDesignImageFile: downloadDesignImageFile,
            downloadUserFile: downloadUserFile,
            downloadCustomerFile: downloadCustomerFile,
            uploadFile: uploadFile
        };

        return service;

        function getDefaultViewModel(customerID) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/' + customerID).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CustomersDetailFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/CustomerDetail', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('CustomersDetailFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function deleteCustomer(customerID, updatedTS) {
            var def = $q.defer();
            $http.delete(appUrl + 'Master/CustomerDetail/' + customerID + '/' + updatedTS + '/Delete').then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('CustomersDetailFactory.deleteCustomer', error);
                def.reject(error);
            });
            return def.promise;
        }

        function getCountryList() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/CountryList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CustomersDetailFactory.getCountryList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getStateList(countryID) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/' + countryID + '/StateList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CustomersDetailFactory.getStateList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function downloadVectorFile(number) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/' + number + '/DownloadVectorFile').then(function (response) {
                def.resolve(response.data); spinnerService.hide('pageContainerSpinner');
            })
             .catch(function fail(error) {
                 console.log('CustomersDetailFactory.downloadOtherFile', error);
                 def.reject(error); spinnerService.hide('pageContainerSpinner');
             });
            return def.promise;
        }

        function downloadDesignImageFile(number) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/' + number + '/DownloadDesignImageFile').then(function (response) {
                def.resolve(response.data); spinnerService.hide('pageContainerSpinner');
            })
             .catch(function fail(error) {
                 console.log('CustomersDetailFactory.downloadOtherFile', error);
                 def.reject(error); spinnerService.hide('pageContainerSpinner');
             });
            return def.promise;
        }

        function downloadUserFile(number) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/' + number + '/DownloadUserFile').then(function (response) {
                def.resolve(response.data); spinnerService.hide('pageContainerSpinner');
            })
             .catch(function fail(error) {
                 console.log('CustomersDetailFactory.downloadOtherFile', error);
                 def.reject(error); spinnerService.hide('pageContainerSpinner');
             });
            return def.promise;
        }

        function downloadCustomerFile(number) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/CustomerDetail/' + number + '/DownloadCustomerFile').then(function (response) {
                def.resolve(response.data); spinnerService.hide('pageContainerSpinner');
            })
             .catch(function fail(error) {
                 console.log('CustomersDetailFactory.downloadOtherFile', error);
                 def.reject(error); spinnerService.hide('pageContainerSpinner');
             });
            return def.promise;
        }

        function uploadFile(imageModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/CustomerDetail/UploadFile', imageModel).then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('CustomersDetailFactory.uploadFile', error);
                 def.reject(error);
             });
            return def.promise;
        }

    }
})();