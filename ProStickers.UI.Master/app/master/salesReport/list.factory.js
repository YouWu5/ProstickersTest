(function () {
    'use strict';

    angular
        .module('app.report')
        .factory('SalesReportListFactory', SalesReportListFactory);

    SalesReportListFactory.$inject = ['$http', '$q', 'appUrl'];

    function SalesReportListFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            getDesignerList: getDesignerList
        };

        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Report/Report/GetList').then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('SalesReportListFactory.getDefaultViewModel', error);
                    def.reject(error);
                });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Report/Report/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('SalesReportListFactory.submit', error);
                    def.reject(error);
                });
            return def.promise;
        }

        function getDesignerList() {
            var def = $q.defer();
            $http.get(appUrl + 'Report/Report/DesignerList').then(function (response) {
                def.resolve(response.data);
            })
                .catch(function fail(error) {
                    console.log('SalesReportListFactory.getDesignerList', error);
                    def.reject(error);
                });
            return def.promise;
        }

    }
})();