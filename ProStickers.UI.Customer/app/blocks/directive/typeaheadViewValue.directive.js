(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('typeaheadViewValue', typeaheadViewValue);

    typeaheadViewValue.$inject = ['$parse'];

    function typeaheadViewValue($parse) {

        var directive = {
            link: link,
            restrict: 'A',
            require: 'ngModel',
        };
        return directive;

        function link(scope, element, attrs, ngModel) {
            element.bind('blur', function () {
                if (scope.ngModel === undefined || scope.ngModel === null || scope.ngModel === '') {

                    if (ngModel.$modelValue === undefined) {
                        ngModel.$viewValue = null;
                        if (attrs.typeaheadViewValue !== '' && attrs.typeaheadViewValue !== undefined && attrs.typeaheadViewValue !== null) {
                            $parse(attrs.typeaheadViewValue).assign(scope, null);
                            scope.$apply();
                        }
                    }
                    ngModel.$render();
                }
            });
        }
    }

})();