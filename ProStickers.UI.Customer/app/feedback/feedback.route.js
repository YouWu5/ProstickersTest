angular.module('app.feedback').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('CustomerFeedback', {
           url: '/customerFeedback/:ID',
           templateUrl: 'app/feedback/customerFeedback.html',
           resolve: {
               initialDataOfFeedback: ['feedbackFactory', '$q', 'localStorageService', '$stateParams',
                   function (feedbackFactory, $q, localStorageService, $stateParams) {
                       console.log('$stateParams kya aaya', $stateParams);
                       var promises = {
                           vm: feedbackFactory.getDefaultViewModel($stateParams.ID),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });

                   }]
           },
           controller: 'CustomerFeedback',
           controllerAs: 'fo'
       });

}]);