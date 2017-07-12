(function () {
    'use strict';

    angular
       .module('app.blocks')
        .directive('realMax', function () {
            return {
                restrict: 'A',
                require: 'ngModel',
                link: function (scope, elem, attr, ctrl) {
                    scope.$watch(attr.realMax, function () {
                        ctrl.$setViewValue(ctrl.$viewValue);
                    });
                    var maxValidator = function (value) {
                        console.log('value', value);
                        var max = scope.$eval(attr.realMax) || Infinity;
                        console.log('max', max);
                        if (!isEmpty(value) && value > max) {
                            ctrl.$setValidity('realMax', false);
                            return undefined;
                        } else {
                            ctrl.$setValidity('realMax', true);
                            return value;
                        }
                    };

                    ctrl.$parsers.push(maxValidator);
                    ctrl.$formatters.push(maxValidator);
                }
            };

            function isEmpty(value) {
                return angular.isUndefined(value) || value === '' || value === null || value !== value;
            }
        });

})();