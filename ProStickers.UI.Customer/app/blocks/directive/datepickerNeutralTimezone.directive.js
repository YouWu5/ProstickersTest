(function () {
    'use strict';

    angular.module('app.blocks')
  .directive('datepickerNeutralTimezone', ['bowser', function (bowser) {
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
                      } else {
                          return null;
                      }
                  });
                  ctrl.$parsers.push(function (value) {
                      if (value !== null) {
                          if (value.getTimezoneOffset() < 0) {
                              var date = new Date(value.getTime() - (60000 * value.getTimezoneOffset()));
                              return date;
                          }
                          else {
                              var date1 = new Date(value.getTime() + (60000 * value.getTimezoneOffset()));
                              return date1;
                          }

                      } else {
                          return null;
                      }
                  });
              }
              else {
                  ctrl.$formatters.push(function (value) {
                      var date;
                      if (value !== null) {
                          date = new Date(value);
                          date = new Date(date.getTime() + (60000 * date.getTimezoneOffset()));
                      }
                      return date;
                  });
                  ctrl.$parsers.push(function (value) {
                      if (value !== null) {
                          if (value.getTimezoneOffset() < 0) {
                              var date = new Date(value.getTime() - (60000 * value.getTimezoneOffset()));
                              return date;
                          }
                          else {
                              var date1 = new Date(value.getTime() + (60000 * value.getTimezoneOffset()));
                              return date1;
                          }

                      } else {
                          return null;
                      }
                  });
              }
          }
      };
  }]);
})();