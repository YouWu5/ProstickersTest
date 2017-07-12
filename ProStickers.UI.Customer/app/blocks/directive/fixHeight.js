(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('fixHeight', fixHeight);

    fixHeight.$inject = ['helper', '$timeout'];

    function fixHeight(helper, $timeout) {

        var directive = {
            link: link,
            restrict: 'EA'
        };

        return directive;

        function link() {
            $timeout(function () {
                var getHeight = $('.page-head-container').innerHeight();
                if (getHeight !== undefined) {
                    helper.setHeight(getHeight + 65 + 'px');
                }
                else {
                    helper.setHeight(55 + 'px');
                }

            }, 100);
        }
    }
})();

(function () {
    'use strict';
    angular
        .module('app.blocks')
        .directive('fixFormHeight', fixFormHeight);

    fixFormHeight.$inject = ['$window'];

    function fixFormHeight($window) {

        var directive = {
            link: link,
            restrict: 'EA'
        };

        return directive;

        function link(scope) {
            scope.$watch(function () {

                var getHeight = $('.page-head-container').innerHeight();
                if (getHeight !== undefined) {
                    if ($window.innerWidth > 992) {
                        scope.fo.lv.topHeightPadding = getHeight + 35 + 'px';
                        scope.fo.lv.topHeightPaddingOne = getHeight + 12 + 'px';
                    } else {
                        scope.fo.lv.topHeightPadding = getHeight + 20 + 'px';
                        scope.fo.lv.topHeightPaddingOne = getHeight + 12 + 'px';
                    }
                }

            }, true);
        }
    }
})();

(function () {
    'use strict';
    angular
        .module('app.blocks')
        .directive('fixFormHeightShell', fixFormHeightShell);

    fixFormHeightShell.$inject = ['$window', '$timeout'];

    function fixFormHeightShell($window, $timeout) {

        var directive = {
            link: link,
            restrict: 'EA'
        };

        return directive;

        function link(scope) {
            $timeout(function () {
                var getHeight = $('.page-head-container').innerHeight();
                if (getHeight !== undefined) {
                    if ($window.innerWidth > 992) {
                        scope.shl.lv.topHeightPaddingOne = getHeight + 25 + 'px';
                    } else {
                        scope.shl.lv.topHeightPaddingOne = getHeight + 30 + 'px';
                    }
                }
            }, 100);
        }
    }
})();

(function () {
    'use strict';
    angular
        .module('app.blocks')
        .directive('fixFormHeightOne', fixFormHeightOne);

    fixFormHeightOne.$inject = ['$window', '$timeout'];

    function fixFormHeightOne($window, $timeout) {

        var directive = {
            link: link,
            restrict: 'EA'
        };

        return directive;

        function link(scope) {

            $timeout(function () {
                var getHeight = $('.page-head-container').innerHeight();
                var txt = $('.row-height').innerHeight();
                if ($window.innerWidth > 992) {
                    if (getHeight !== undefined) {
                        scope.fo.lv.topHeightPaddingOne = getHeight + txt + 20 + 'px';
                    }
                }
                else {
                    if (getHeight !== undefined) {
                        scope.fo.lv.topHeightPaddingOne = getHeight + 20 + 'px';
                    }
                }
            }, 100);
        }
    }
})();

(function () {
    'use strict';
    angular
        .module('app.blocks')
        .directive('setFooter', setFooter);

    setFooter.$inject = ['$window', '$timeout'];

    function setFooter($window, $timeout) {

        var directive = {
            link: link,
            restrict: 'EA'
        };

        return directive;

        function link(scope) {

            scope.$watch(function () {
                $timeout(function () {
                    var getHeight = $('.page-head-container').innerHeight();
                    var txt = $('.row-height').innerHeight();
                    if (txt !== undefined) {
                        if (getHeight !== undefined) {
                            scope.fo.lv.topHeightPaddingTemp = getHeight + txt + 20;
                        }
                    }
                    else {
                        scope.fo.lv.topHeightPaddingTemp = getHeight + 15;
                    }

                    var windowSize = $window.innerHeight;
                    var sectionHeight = $('.section-height').innerHeight();
                    var sectionTotalHeight = sectionHeight + scope.fo.lv.topHeightPaddingTemp;

                    if (windowSize > sectionTotalHeight) {
                        scope.fo.lv.setFooterPaddingRecord = (windowSize) - (sectionTotalHeight + 202) + 'px';
                        scope.fo.lv.setFooterPaddingNoRecord = (windowSize) - (sectionTotalHeight + 244) + 'px';
                        scope.fo.lv.setFooterPaddingForm = (windowSize) - (sectionTotalHeight + 174) + 'px';
                        scope.fo.lv.setFooterPaddingFormSuggested = (windowSize) - (sectionTotalHeight + 147) + 'px';
                    }
                    else {
                        scope.fo.lv.setFooterPaddingRecord = 0 + 'px';
                        scope.fo.lv.setFooterPaddingNoRecord = 0 + 'px';
                        scope.fo.lv.setFooterPaddingFormSuggested = 0 + 'px';
                        scope.fo.lv.setFooterPaddingForm = 0 + 'px';
                    }

                }, 100);
            }, true);
        }
    }
})();

(function () {
    'use strict';
    angular
        .module('app.blocks')
        .directive('setFooterShell', setFooterShell);

    setFooterShell.$inject = ['$window', '$timeout', 'bowser'];

    function setFooterShell($window, $timeout, bowser) {

        var directive = {
            link: link,
            restrict: 'EA'
        };

        return directive;

        function link(scope) {

            scope.$watch(function () {
                $timeout(function () {
                    var getHeight = $('.page-head-container').innerHeight();
                    var txt = $('.row-height').innerHeight();
                    if (txt !== undefined) {
                        if (getHeight !== undefined) {
                            scope.shl.lv.topHeightPaddingTemp = getHeight + txt + 20;
                        }
                    }
                    else {
                        scope.shl.lv.topHeightPaddingTemp = getHeight + 15;
                    }

                    var windowSize = $window.innerHeight;
                    var sectionHeight = $('.section-height').innerHeight();
                    var sectionTotalHeight = sectionHeight + scope.shl.lv.topHeightPaddingTemp;

                    if (windowSize > sectionTotalHeight) {

                        if (bowser.chrome === true) {
                            scope.shl.lv.setFooterPaddingForm = (windowSize) - (sectionTotalHeight + 186) + 'px';

                        }
                        else {
                            scope.shl.lv.setFooterPaddingForm = (windowSize) - (sectionTotalHeight + 150) + 'px';

                        }

                    }
                    else {
                        scope.shl.lv.setFooterPaddingForm = 0 + 'px';

                    }

                    if (windowSize > sectionHeight) {

                        if (bowser.chrome === true) {

                            scope.shl.lv.setFooterPaddingFormHome = (windowSize) - (sectionHeight + 200) + 'px';
                        }
                        else {

                            scope.shl.lv.setFooterPaddingFormHome = (windowSize) - (sectionHeight + 198) + 'px';
                        }

                    }
                    else {

                        scope.shl.lv.setFooterPaddingFormHome = 0 + 'px';
                    }
                }, 100);

            }, true);

        }
    }
})();

(function () {
    'use strict';
    angular
        .module('app.blocks')
        .directive('agreementContainerHeight', agreementContainerHeight);

    agreementContainerHeight.$inject = ['$window', '$timeout'];

    function agreementContainerHeight($window, $timeout) {

        var directive = {
            link: link,
            restrict: 'EA'
        };

        return directive;

        function link(scope) {


            $timeout(function () {
                var getHeight = $('.page-head-container').innerHeight();
                var windowSize = $window.innerHeight;
                var agreementContainerHeight = windowSize - (getHeight + 249);

                scope.shl.lv.setAgreementContainerHeight = agreementContainerHeight + 'px';
            }, 100);

        }
    }
})();