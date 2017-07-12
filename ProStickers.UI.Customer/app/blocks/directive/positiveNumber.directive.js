/*global $ */
(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('positiveNumber', positiveNumber);
     
    function positiveNumber() {
       
        var directive = {
            link: link,
            restrict: 'EA',
            require: '^ngModel',
        };
        return directive;

        function link(scope, element, attrs, ngModel) {
            element.on('keydown', function (e) {

               
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
                    // Allow: Ctrl+A
                    (e.keyCode === 65 && e.ctrlKey === true) ||
                    // Allow: Ctrl+V
                    (e.keyCode === 86 && e.ctrlKey === true) ||
                    // Allow: Ctrl+C
                    (e.keyCode === 88 && e.ctrlKey === true) ||
                    // Allow: Ctrl+X
                    (e.keyCode === 67 && e.ctrlKey === true) ||
                    // Allow: home, end, left, right
                    (e.keyCode >= 35 && e.keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
              
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            });

            if (!ngModel) {
                return;
            }

            function inputValue(val) {
                if (val) {
                    var f = parseFloat(val);

                    if (attrs.min !== undefined) {
                        var minValue = scope.$eval(attrs.min);

                        if (f < minValue) {
                            ngModel.$setValidity('min', false);
                        }
                        else {
                            ngModel.$setValidity('min', true);
                        }
                    }

                    if (attrs.max !== undefined) {
                        var maxValue = scope.$eval(attrs.max);

                        if (f > maxValue) {
                            ngModel.$setValidity('max', false);
                        }
                        else {
                            ngModel.$setValidity('max', true);
                        }
                    }

                    var old = ngModel.$modelValue;

                    if (new RegExp(/^[0-9]*$/).test(val)) {
                        return val;
                    }
                    else {
                        ngModel.$setViewValue(old);
                        ngModel.$render();
                        return old;
                    }
                }
                return null;
            }

            ngModel.$parsers.push(inputValue);
        }
    }

})();