(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('datepickerPattern', datepickerPattern);
     
    function datepickerPattern() {
        var dRegex;
        var directive = {
            restrict: 'A',
            require: 'ngModel',
            link: link

        };
        return directive;

        function link(scope, elem, attrs, ngModelCtrl) {
            dRegex = new RegExp(/(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[01])[- \/.](19|20)\d\d/);
            ngModelCtrl.$parsers.unshift(function (value) {

                if (attrs.datepickerPattern === 'FiscalYear') {
                    var formname = ngModelCtrl.$$parentForm.$name;
                    scope.$parent[formname].StartDate.$setValidity('datepickerPattern', true);
                    scope.$parent[formname].EndDate.$setValidity('datepickerPattern', true);
                }

                if (typeof value === 'string') {

                    validate(ngModelCtrl);

                    if (value !== null && value !== '') {
                        var isValid = dRegex.test(value);
                        ngModelCtrl.$setValidity('datepickerPattern', isValid);

                        if (isValid) {
                            var minDate, maxDate;
                            var name = ngModelCtrl.$$parentForm.$name;
                            if (attrs.datepickerPattern === 'StartDate') {
                                minDate = scope.$eval(attrs.minDate);
                                maxDate = scope.$eval(attrs.maxDate);
                                var dStart = value.split('/');
                                var dateStart = new Date(dStart[2], dStart[0] - 1, dStart[1]).setHours(0, 0, 0, 0);
                                if (minDate !== undefined && minDate !== null && minDate !== '') {
                                    minDate = new Date(minDate).setHours(0, 0, 0, 0);
                                    if (dateStart < minDate) {
                                        isValid = false;
                                        ngModelCtrl.$setValidity('datepickerPattern', isValid);
                                    }
                                }

                                if (maxDate !== undefined && maxDate !== null && maxDate !== '') {
                                    maxDate = new Date(maxDate).setHours(0, 0, 0, 0);
                                    if (dateStart > maxDate) {
                                        isValid = false;
                                        ngModelCtrl.$setValidity('datepickerPattern', isValid);
                                    }
                                    else {
                                        if (scope[name].EndDate) {
                                            var newEnd = scope[name].EndDate.$viewValue.split('/');
                                            if ((new Date(dStart[2], dStart[0] - 1, dStart[1]) <= new Date(newEnd[2], newEnd[0] - 1, newEnd[1])) &&
                                                    (new Date(newEnd[2], newEnd[0] - 1, newEnd[1]) <= (scope.fo.lv.EndDate))) {

                                                scope[name].EndDate.$setValidity('datepickerPattern', true);
                                            }
                                        }
                                    }
                                }
                            }

                            if (attrs.datepickerPattern === 'EndDate') {

                                minDate = scope.$eval(attrs.minDate);
                                maxDate = scope.$eval(attrs.maxDate);
                                var dEnd = angular.copy(value.split('/'));
                                var dateEnd = new Date(dEnd[2], dEnd[0] - 1, dEnd[1]).setHours(0, 0, 0, 0);
                                if (minDate !== undefined && minDate !== null && minDate !== '') {
                                    minDate = new Date(minDate).setHours(0, 0, 0, 0);

                                    if (dateEnd < minDate) {
                                        isValid = false;
                                        ngModelCtrl.$setValidity('datepickerPattern', isValid);
                                    }
                                    else {
                                        if (scope[name].StartDate.$viewValue !== undefined && scope[name].StartDate.$viewValue !== null) {
                                            var newDate = scope[name].StartDate.$viewValue.split('/');
                                            if ((new Date(dEnd[2], dEnd[0] - 1, dEnd[1]) >= new Date(newDate[2], newDate[0] - 1, newDate[1]) &&
                                                (new Date(newDate[2], newDate[0] - 1, newDate[1]) >= (scope.fo.lv.StartDate)))) {
                                                scope[name].StartDate.$setValidity('datepickerPattern', true);
                                            }
                                        }
                                    }
                                }

                                if (maxDate !== undefined && maxDate !== null && maxDate !== '') {
                                    maxDate = new Date(maxDate).setHours(0, 0, 0, 0);

                                    if (dateEnd > maxDate) {
                                        isValid = false;
                                        ngModelCtrl.$setValidity('datepickerPattern', isValid);
                                    }
                                }

                            }
                        }

                        if (!isValid) {
                            return undefined;
                        }

                        var fragments = value.split('/');
                        var result = new Date(fragments[2], fragments[0] - 1, fragments[1]);
                        return result;
                    }
                    else {
                        ngModelCtrl.$setValidity('datepickerPattern', true);
                    }

                    return value;
                }

                if (typeof value === 'object') {
                    ngModelCtrl.$setValidity('datepickerPattern', true);
                    return value;
                }

                return value;
            });

            ngModelCtrl.$parsers.push(function (value) {
                return value;
            });
        }

        function validate(ngModelCtrl) {

            var dateMonth = (ngModelCtrl.$$lastCommittedViewValue).split('/');
            if (dateMonth[0] !== null) {
                if (dateMonth[0] === '04' || dateMonth[0] === '06' || dateMonth[0] === '09' || dateMonth[0] === '11') {
                    dRegex = new RegExp(/(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[00])[- \/.](19|20)\d\d/);
                }
                else if (dateMonth[0] === '01' || dateMonth[0] === '03' || dateMonth[0] === '05' || dateMonth[0] === '07' ||
                     dateMonth[0] === '08' || dateMonth[0] === '10' || dateMonth[0] === '12') {
                    dRegex = new RegExp(/(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9]|3[01])[- \/.](19|20)\d\d/);
                }
                else if (dateMonth[0] === '02') {
                    var leapYear = ((dateMonth[2] % 4 === 0) && (dateMonth[2] % 100 !== 0)) || (dateMonth[2] % 400 === 0);
                    if (leapYear === true) {
                        dRegex = new RegExp(/(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-9])[- \/.](19|20)\d\d/);
                    }
                    else {
                        dRegex = new RegExp(/(0[1-9]|1[012])[- \/.](0[1-9]|[12][0-8])[- \/.](19|20)\d\d/);
                    }

                }
            }
        }

    }

})();