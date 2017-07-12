(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('dateGreaterThan', dateGreaterThan);

    dateGreaterThan.$inject = ['$filter'];

    function dateGreaterThan($filter) {
        
        var directive = {
            link: link,
            restrict: 'A',
            require: 'ngModel',
        };
        return directive;

        function link(scope, elem, attrs, ngModelCtrl) {
            var dRegex = new RegExp(/(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[01])[- \/.](19|20)\d\d/);
           ngModelCtrl.$parsers.unshift(function (value) {
                var minDate, maxDate;
                var isValid = true;

                if (typeof value === 'object') {
                    ngModelCtrl.$setValidity('dateGreaterThan', true);
                    return value;
                }

                if (typeof value === 'string') {
                    if (value !== null && value !== '') {
                        isValid = dRegex.test(value);
                        ngModelCtrl.$setValidity('dateGreaterThan', isValid);

                        if (isValid) {
                            /* Here always the case of start date , here two max date i.e. today date and in case of sale order tentative delivery date */

                            minDate = scope.$eval(attrs.minDate);
                            maxDate = scope.$eval(attrs.maxDate);
                            console.log('maxDate', maxDate);

                            var dateArray = value.split('/');

                            var date = new Date(dateArray[2], dateArray[0] - 1, dateArray[1]).setHours(0, 0, 0, 0);
                            date = new Date(date).setHours(0, 0, 0, 0);

                            if (minDate !== undefined && minDate !== null && minDate !== '') {
                                minDate = new Date(minDate).setHours(0, 0, 0, 0);

                                if (date < minDate) {
                                    isValid = false;
                                    ngModelCtrl.$setValidity('dateGreaterThan', isValid);
                                }
                            }

                            if (maxDate !== undefined && maxDate !== null && maxDate !== '') {
                                maxDate = new Date(maxDate).setHours(0, 0, 0, 0);
                               

                                if (date > maxDate) {
                                    isValid = false;
                                    ngModelCtrl.$setValidity('dateGreaterThan', isValid);
                                }
                            }
                        }

                        if (!isValid) {
                            return undefined;
                        }

                        var fragments = value.split('/');

                        var result = new Date(Date.UTC(fragments[2], fragments[0] - 1, fragments[1]));

                        return result;
                    }
                    else {
                        ngModelCtrl.$setValidity('dateGreaterThan', true);
                        return value;
                    }
                }

                return value;
            });

            ngModelCtrl.$parsers.push(function (value) {
                return value;
            });

            scope.$watch(attrs.dateGreaterThan, function () {
                var minDate = scope.$eval(attrs.dateGreaterThan);  // other date 

                if (minDate !== undefined && minDate !== null && minDate !== '' && ngModelCtrl.$modelValue !== undefined && ngModelCtrl.$modelValue !== null && ngModelCtrl.$modelValue !== '') {
                    minDate = new Date(minDate).setHours(0, 0, 0, 0);

                    if (new Date(ngModelCtrl.$modelValue).setHours(0, 0, 0, 0) < minDate) {

                        ngModelCtrl.$setViewValue($filter('date')(new Date(minDate), 'MM/dd/yyyy'));
                        ngModelCtrl.$render();
                    }
                }
            });
        }
    }

})();