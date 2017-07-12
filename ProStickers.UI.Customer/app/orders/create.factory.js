(function () {
    'use strict';

    angular
        .module('app.order')
        .factory('orderCreateFactory', orderCreateFactory);

    orderCreateFactory.$inject = ['$http', '$q', 'appUrl'];

    function orderCreateFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            getStateList: getStateList,
            getCountryList: getCountryList,
            getYearList: getYearList,
            getMonthList: getMonthList,
            submit: submit
        };

        return service;

        function getDefaultViewModel(designNo, appointmentNo) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Order/' + designNo + '/' + appointmentNo + '/Detail')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at order create', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }

        function getMonthList() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Order/MonthList')
            .then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getYearList() {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/Order/YearList')
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
            $http.get(appUrl + 'Customer/Order/CountryList')
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
            $http.get(appUrl + 'Customer/Order/' + countryID + '/StateList')
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

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Customer/Order', ViewModel).then(function (response) {
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