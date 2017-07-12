/*global $ */
(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('positiveDecimalNumber', positiveDecimalNumber);

    //positiveDecimalNumber.$inject = [];

    function positiveDecimalNumber() {
        // Usage:
        //     <positiveDecimalNumber></positiveDecimalNumber>
        // Creates:
        // 
        var directive = {
            link: link,
            restrict: 'EA',
            require: '^ngModel',
        };
        return directive;

        function link(scope, element, attrs, ngModel) {

            element.on('keydown', function (e) {

                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    (e.keyCode === 65 && e.ctrlKey === true) ||
                    (e.keyCode >= 35 && e.keyCode <= 39)) {
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

                    var old = ngModel.$modelValue;
                    var checkRegex = true;
                    var count = 0;

                //    number = val.split('');

                    for (var i = 0; i < val.length; i++) {
                        if (val[i] === '.') {
                            count++;
                        }
                    }

                    if (count > 1 || count === 0) {
                        checkRegex = true;
                    }
                    else {
                        if (val[val.length - 1] === '.') {
                            checkRegex = false;
                        }
                        else {
                            checkRegex = true;
                        }
                    }

                    if (checkRegex === true) {
                        var numReg = (new RegExp(/^([0-9]{0,9}([.][0-9]{1,2})?)$/));
                        if (numReg.test(val)) {
                            return val;
                        }
                        else {
                            ngModel.$setViewValue(old);
                            ngModel.$render();

                            return old;
                        }
                    }
                    else {
                        return val;
                    }
                }
                return null;
            }

            ngModel.$parsers.push(inputValue);

        }
    }

})();