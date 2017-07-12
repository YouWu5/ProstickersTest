(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('msgHideShow', msgHideShow);

    msgHideShow.$inject = [];

    function msgHideShow() {

        var directive = {
            restrict: 'EACM',
            link: link
        };

        return directive;

        function link(scope, element) {
            element.find('.mes-open').removeClass('msgToggleBtn');
            $('.mes-close').bind('click', function () {
                $('.mes-open').addClass('msgToggleBtn');
                $('.msg-box').find('div.successMsg').addClass('ng-hide');
            });
            $('.mes-open').bind('click', function () {
                $('.mes-open').removeClass('msgToggleBtn');
                $('.msg-box').find('div.successMsg').removeClass('ng-hide');
            });
            $('.container-page').bind('click', function () {
                $('.mes-open').addClass('msgToggleBtn');
                $('.msg-box').find('div.successMsg').addClass('ng-hide');
            });
            $('.page-with-dashboard').bind('click', function () {
                $('.mes-open').addClass('msgToggleBtn');
                $('.msg-box').find('div.successMsg').addClass('ng-hide');
            });
            $('.pageButton a').bind('click', function () {
                $('.mes-open').removeClass('msgToggleBtn');
                $('.msg-box').find('div.successMsg').removeClass('ng-hide');
            });
        }
    }
})();