angular.module('app.feedback').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('FeedbackList', {
           url: '/feedbacklist',
           templateUrl: 'app/master/feedback/list.html',
           resolve: {
               initialDataOfFeedbackList: ['FeedbackListFactory', '$q',
                   function (FeedbackListFactory, $q) {
                       var promises = {
                           vm: FeedbackListFactory.getDefaultViewModel(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });

                   }]
           },
           controller: 'FeedbackList',
           controllerAs: 'fo'
       })

     .state('FeedbackDetail', {
         url: '/feedbacklist/feedbackdetail/:ID/:DesignNo',
         templateUrl: 'app/master/feedback/detail.html',
         resolve: {
             initialDataOfFeedbackDetail: ['FeedbackDetailFactory', '$q', '$stateParams',
                 function (FeedbackDetailFactory, $q, $stateParams) {
                     var promises = {
                         vm: FeedbackDetailFactory.getDefaultViewModel($stateParams.ID, $stateParams.DesignNo),
                     };
                     return $q.all(promises).then(function (values) {
                         var initData = {};
                         initData.viewModel = values.vm;
                         return initData;
                     });

                 }]
         },
         controller: 'FeedbackDetail',
         controllerAs: 'fo'
     });
}]);