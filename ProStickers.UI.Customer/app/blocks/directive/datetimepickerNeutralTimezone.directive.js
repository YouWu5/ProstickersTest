(function () {
    'use strict';

    angular.module('app.blocks')
  .directive('datetimepickerNeutralTimezone', ['bowser', function (bowser) {
      return {
          restrict: 'A',
          priority: 1,
          require: 'ngModel',
          link: function (scope, element, attrs, ctrl) {
              if (bowser.firefox === true || bowser.msedge === true) {
                  ctrl.$formatters.push(function (value) {
                      if (value !== null) {
                          var date = new Date(value);
                          return date;
                      }
                  });
                  ctrl.$parsers.push(function (value) {
                      if (value !== null) {
                          return value;
                      }
                  });
              }
              else {
                  ctrl.$formatters.push(function (value) {
                      var date;
                      if (value !== null) {
                          date = new Date(Date.parse(value));
                          date = new Date(date.getTime() + (60000 * date.getTimezoneOffset()));
                      }
                      return date;
                  });
                  ctrl.$parsers.push(function (value) {
                      if (value !== null) {
                          return value;
                      }
                  });
              }
          }
      };
  }]);
})();