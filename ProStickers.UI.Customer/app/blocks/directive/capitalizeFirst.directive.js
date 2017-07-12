(function () {
    'use strict';
    angular
        .module('app.blocks')
        .directive('capitalizeFirst', capitalizeFirst);

    capitalizeFirst.$inject = ['$parse'];

    function capitalizeFirst($parse) {

        var directive = {
            link: link,
            require: 'ngModel',
            restrict: 'AE'
        };

        return directive;

        function link(scope, element, attrs, modelCtrl) {
            var capitalize = function (inputValue) {
                var capitalFirst = '';
                if (inputValue !== null) {
                    var res = inputValue.split(' ');
                    angular.forEach(res, function (value) {
                        var capitalized = value.charAt(0).toUpperCase() + value.substring(1) + ' ';
                        capitalFirst += capitalized;
                    });
                    capitalFirst = capitalFirst.trim();
                    return capitalFirst;
                }

            };

            modelCtrl.$parsers.push(capitalize);
            capitalize($parse(attrs.ngModel)(scope));
        }
    }

})();
