(function () {
    'use strict';

    angular
        .module('app.feedback')
        .factory('feedbackFactory', feedbackFactory);

    feedbackFactory.$inject = ['$http', '$q', 'appUrl'];

    function feedbackFactory($http, $q, appUrl) {
        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit
        };

        return service;

        function getDefaultViewModel(customerID) {
            var def = $q.defer();
            $http.get(appUrl + 'Customer/CustomerFeedback/' + customerID + '/GetByID')
            .then(function (response) {
                def.resolve(response.data);
                console.log('Data response at feedback', response.data);
            })
             .catch(function fail(error) {
                 console.log('error', error);
                 def.reject(error);
             });

            return def.promise;
        }
 
        function submit(viewModel) {
            console.log('viewModel', viewModel);
            var def = $q.defer();
            $http.post(appUrl + 'Customer/CustomerFeedback', viewModel)
             .then(function (response) {
                 def.resolve(response.data);
             }).catch(function fail(error) {
                 console.log('feedbackFactory.submit', error);
                 def.reject(error);
             });
            return def.promise;
        }


    }
})();