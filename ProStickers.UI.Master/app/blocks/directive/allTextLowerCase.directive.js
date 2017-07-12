(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('allTextLowerCase', allTextLowerCase);

    allTextLowerCase.$inject = ['$parse'];

    function allTextLowerCase($parse) {

        var directive = {
            link: link,
            require: 'ngModel',
            restrict: 'AE'
        };

        return directive;

        function link(scope, element, attrs, modelCtrl) {
            var lower = function (inputValue) {
                if (inputValue != null) {
                    inputValue = inputValue.toLowerCase();
                    return inputValue;
                }
            };
            modelCtrl.$parsers.push(lower);
            lower($parse(attrs.ngModel)(scope));
        }
    }
})();