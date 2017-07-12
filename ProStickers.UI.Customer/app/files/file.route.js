angular.module('app.file').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('FilesList', {
           url: '/filesList',
           templateUrl: 'app/files/list.html',
           resolve: {
               initialDataOfFilesList: ['filesListFactory', '$q',
                   function (filesListFactory, $q) {
                       var promises = {
                           vm: filesListFactory.getDefaultViewModel(),
                           fl: filesListFactory.getFilesList(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           initData.filesList = values.fl;
                           return initData;
                       });

                   }]
           },
           controller: 'FilesList',
           controllerAs: 'fo'
       });
         
}]);