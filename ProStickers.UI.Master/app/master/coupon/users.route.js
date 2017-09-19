angular.module('app.users').config(['$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {
    'use strict';
    $urlRouterProvider.otherwise('/');
    $stateProvider

       .state('UsersList', {
           url: '/usersList',
           templateUrl: 'app/master/users/list.html',
           resolve: {
               initialDataOfUsersList: ['UsersListFactory', '$q',
                   function (UsersListFactory, $q) {
                       var promises = {
                           vm: UsersListFactory.getDefaultViewModel(),
                       };
                       return $q.all(promises).then(function (values) {
                           var initData = {};
                           initData.viewModel = values.vm;
                           return initData;
                       });
                   }]
           },
           controller: 'UsersList',
           controllerAs: 'fo'
       })

     .state('UsersUpdate', {
         url: '/usersList/updateUser/:ID',
         templateUrl: 'app/master/users/update.html',
         resolve: {
             initialDataOfUsersUpdate: ['UsersUpdateFactory', '$q', '$stateParams',
                 function (UsersUpdateFactory, $q, $stateParams) {
                     var promises = {
                         vm: UsersUpdateFactory.getDefaultViewModel($stateParams.ID),
                     };
                     return $q.all(promises).then(function (values) {
                         var initData = {};
                         initData.viewModel = values.vm;
                         return initData;
                     });
                 }]
         },
         controller: 'UsersUpdate',
         controllerAs: 'fo'
     })

     .state('UsersCreate', {
         url: '/usersList/createUser',
         templateUrl: 'app/master/users/create.html',
         resolve: {
             initialDataOfUsersCreate: ['UsersCreateFactory', '$q',
                 function (UsersCreateFactory, $q) {
                     var promises = {
                         vm: UsersCreateFactory.getDefaultViewModel(),
                         rl: UsersCreateFactory.getRoleList(),
                     };
                     return $q.all(promises).then(function (values) {
                         var initData = {};
                         initData.viewModel = values.vm;
                         initData.roleList = values.rl;
                         return initData;
                     });
                 }]
         },
         controller: 'UsersCreate',
         controllerAs: 'fo'
     });
}]);