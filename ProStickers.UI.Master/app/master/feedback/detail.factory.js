(function () {
    'use strict';

    angular
        .module('app.feedback')
        .factory('FeedbackDetailFactory', FeedbackDetailFactory);

    FeedbackDetailFactory.$inject = ['$http', '$q', 'appUrl'];

    function FeedbackDetailFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            deleteFeedback: deleteFeedback
        };

        return service;

        function getDefaultViewModel(customerId, designNo) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/Feedback/' + customerId +'/'+ designNo + '/GetByID').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('FeedbackDetailFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function submit(ViewModel) {
            var def = $q.defer();
            $http.put(appUrl + 'Master/Feedback', ViewModel).then(function (response) {
                def.resolve(response.data);
            }).catch(function fail(error) {
                console.log('FeedbackDetailFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function deleteFeedback(customerId, designNo) {
            var def = $q.defer();
            $http.delete(appUrl + 'Master/Feedback/' + customerId + '/' + designNo + '/DeleteFeedback').then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('PredefinedSizeListFactory.deleteSize', error);
                def.reject(error);
            });
            return def.promise;
        }

    }
})();