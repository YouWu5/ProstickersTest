(function () {
    'use strict';

    angular
       .module('app.blocks')
       .directive('realMin', function () {
           return {
               restrict: 'A',
               require: 'ngModel',
               link: function (scope, elem, attr, ctrl) {
                   scope.$watch(attr.realMin, function () {
                       ctrl.$setViewValue(ctrl.$viewValue);
                   });
                   var minValidator = function (value) {
                       var min = scope.$eval(attr.realMin) || 0;
                       if (!isEmpty(value) && value < min) {
                           ctrl.$setValidity('realMin', false);
                           return undefined;
                       } else {
                           ctrl.$setValidity('realMin', true);
                           return value;
                       }
                   };

                   ctrl.$parsers.push(minValidator);
                   ctrl.$formatters.push(minValidator);
               }
           };

           function isEmpty(value) {
               return angular.isUndefined(value) || value === '' || value === null || value !== value;
           }

       });

})();