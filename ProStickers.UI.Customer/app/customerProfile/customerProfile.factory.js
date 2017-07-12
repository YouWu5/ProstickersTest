(function () {
    'use strict';

    angular
        .module('app.profile')
        .factory('customerProfileFactory', customerProfileFactory);

    customerProfileFactory.$inject = ['$http', '$q', 'appUrl'];

    function customerProfileFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            updateProfile: updateProfile,
            getStateList: getStateList,
            getCountryList: getCountryList,
            getProfileDetailByID: getProfileDetailByID
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Customer/Default')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response customer profile', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });
            return def.promise;
        }
         
        function getProfileDetailByID(customerID) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Customer/' + customerID)
            .then(function (response) {
                def.resolve(response.data);
              
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getCountryList() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Customer/CountryList')
            .then(function (response) {
                def.resolve(response.data);
                
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getStateList(countryID) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Customer/' + countryID + '/StateList')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response customer profile', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function updateProfile(viewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Customer/Customer', viewModel)
             .then(function (response) {
                 def.resolve(response.data);
             }).catch(function fail(error) {
                 console.log('customerProfileFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }


    }
})();