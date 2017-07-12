(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('scrollOnValidation', scrollOnValidation);
 
    function scrollOnValidation() {
        
        var directive = {
            link: link,
            restrict: 'EA'
        };
        return directive;

        function link(scope, element) {

            scope.$on('scrollToError', function () {
                var firstInvalid = element[0].querySelector('.ng-invalid');

                if (firstInvalid) {
                    firstInvalid.focus();
                }
            });
        }
    }

})();