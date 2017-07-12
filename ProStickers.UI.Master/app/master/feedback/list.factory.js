(function () {
    'use strict';

    angular
        .module('app.feedback')
        .factory('FeedbackListFactory', FeedbackListFactory);

    FeedbackListFactory.$inject = ['$http', '$q', 'appUrl'];

    function FeedbackListFactory($http, $q, appUrl) {

        var service = {
            getDefaultViewModel: getDefaultViewModel,
            submit: submit,
            getCustomerFeedbackList: getCustomerFeedbackList,
            getCustomerList: getCustomerList
        };
        
        return service;

        function getDefaultViewModel() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/Feedback/GetList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('FeedbackListFactory.getDefaultViewModel', error);
                 def.reject(error);
             });
            return def.promise;
        }
        
        function submit(ViewModel) {
            var def = $q.defer();
            $http.post(appUrl + 'Master/Feedback/List', ViewModel).then(function (response) {
                def.resolve(response.data);
            })
            .catch(function fail(error) {
                console.log('FeedbackListFactory.submit', error);
                def.reject(error);
            });
            return def.promise;
        }

        function getCustomerFeedbackList() {
            var def = $q.defer();
            $http.get(appUrl + 'Master/Feedback/GetFeedbackList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('FeedbackListFactory.getCustomerFeedbackList', error);
                 def.reject(error);
             });
            return def.promise;
        }

        function getCustomerList(codeName) {
            var def = $q.defer();
            $http.get(appUrl + 'Master/Feedback/' + codeName + '/GetCustomerList').then(function (response) {
                def.resolve(response.data);
            })
             .catch(function fail(error) {
                 console.log('FeedbackListFactory.getCustomerList', error);
                 def.reject(error);
             });
            return def.promise;
        }

    }
})();