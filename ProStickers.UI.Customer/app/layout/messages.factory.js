(function () {
    'use strict';

    angular
        .module('app.layout')
        .factory('messagesFactory', messagesFactory);

    messagesFactory.$inject = ['$http', '$q', 'appUrl'];

    function messagesFactory($http, $q,  appUrl) {
        var service = {
            print: printPage,
            printDetail: printDetail
        };

        return service;

        function printPage(url) {

            var def = $q.defer();

            $http.post(appUrl + url).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('messagesFactory.printPage', error);
                def.reject(error);
            });

            return def.promise;

        }

        function printDetail(obj) {
            var def = $q.defer();

            $http.post(appUrl + 'Print/TransactionDetail/' + obj.Id + '/' + 'false' + '/' + obj.ReportId, null, { headers: { 'loaderID': 'pageContainerSpinner' } })
            .then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('messagesFactory.printDetail', error);
                def.reject(error);
            });

            return def.promise;
        }


    }
})();