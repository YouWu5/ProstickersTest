(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('dateSmallerThan', dateSmallerThan);

    function dateSmallerThan() {

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
                    ngModelCtrl.$setValidity('dateSmallerThan', true);

                    return value;
                }

                if (typeof value === 'string') {


                    if (value !== null && value !== '') {
                        isValid = dRegex.test(value);

                        ngModelCtrl.$setValidity('dateSmallerThan', isValid);

                        if (isValid) {
                            minDate = scope.$eval(attrs.minDate);
                            maxDate = scope.$eval(attrs.maxDate);
                            /* Here always the case of start date , here two max date i.e. today date and in case of sale order tentative delivery date */

                            var dateArray = value.split('/');
                            var date = new Date(dateArray[2], dateArray[0] - 1, dateArray[1]).setHours(0, 0, 0, 0);

                            if (minDate !== undefined && minDate !== null && minDate !== '') {
                                minDate = new Date(minDate).setHours(0, 0, 0, 0);
                                if (date < minDate) {
                                    isValid = false;
                                    ngModelCtrl.$setValidity('dateSmallerThan', isValid);
                                }
                            }

                            if (maxDate !== undefined && maxDate !== null && maxDate !== '') {
                                maxDate = new Date(maxDate).setHours(0, 0, 0, 0); // contain today date

                                if (date > maxDate) {
                                    isValid = false;
                                    ngModelCtrl.$setValidity('dateSmallerThan', isValid);
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
                }

                return value;
            });

            ngModelCtrl.$parsers.push(function (value) {

                return value;
            });

            scope.$watch(attrs.dateSmallerThan, function () {
                var maxDate = scope.$eval(attrs.dateSmallerThan);  // other date 
                if (maxDate !== undefined && maxDate !== null && maxDate !== '' && ngModelCtrl.$modelValue !== undefined && ngModelCtrl.$modelValue !== null && ngModelCtrl.$modelValue !== '') {
                    maxDate = new Date(maxDate).setHours(0, 0, 0, 0);
                    if (new Date(ngModelCtrl.$modelValue).setHours(0, 0, 0, 0) > maxDate) {
                        ngModelCtrl.$setViewValue((new Date(maxDate), 'MM/dd/yyyy'));
                        ngModelCtrl.$render();
                    }
                }
            });

        }
    }

})();