(function () {
    'use strict';

    angular
        .module('app.core')
        .factory('helper', helper);

    helper.$inject = ['$filter', '$rootScope', 'localStorageService'];
    var isSubmitted = false;

    function helper($filter, $rootScope, localStorageService) {
        var userInfoHelper = null;

        var service = {
            getHeight: getHeight,
            setHeight: setHeight,
            getIsSubmitted: getIsSubmitted,
            setIsSubmitted: setIsSubmitted,
            formatDate: formatDate,
            formatDateObject: formatDateObject,
            scrollToError: scrollToError,
            formatDateTime: formatDateTime,
            ConvertDateCST: ConvertDateCST
        };

        return service;

        function scrollToError() {
            $rootScope.$broadcast('scrollToError');
        }

        function getIsSubmitted() {
            return isSubmitted;
        }

        function setIsSubmitted(flag) {
            isSubmitted = flag;
        }

        function setHeight(obj) {
            userInfoHelper = obj;
        }

        function getHeight() {
            return userInfoHelper;
        }

        function formatDate(date) {
            return ($filter('date')(date, 'yyyy-MM-dd'));
        }
        function formatDateTime(date) {
            return ($filter('date')(date, 'yyyy-MM-ddTHH:mm'));

        }
        function formatDateObject(date) {
            return new Date(date);
        }

        function ConvertDateCST(date) {
            var session = localStorageService.get('UserSession');
            var a = parseFloat(session.UtcDateTimeOffset.split(':')[0]);
            var now = new Date(date);
            var now_utc = new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds());
            now_utc.setHours(now_utc.getHours() + a);
            return now_utc;
        }
    }
})();