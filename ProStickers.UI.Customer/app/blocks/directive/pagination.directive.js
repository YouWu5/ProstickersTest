
(function () {
    'use strict';
    angular
         .module('app.blocks').directive('paginate', function () {
             return {
                 restrict: 'E',
                 templateUrl: '/app/other/paginate.html',
                 replace: false
             };
         });
})();

(function () {
    'use strict';

    angular
         .module('app.blocks').directive('paginateTransaction', function () {
             return {
                 restrict: 'E',
                 templateUrl: '/app/other/paginateTransaction.html',
                 replace: false
             };
         });
})();
