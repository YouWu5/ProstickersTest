/*global $ */
(function () {
    'use strict';
    angular
         .module('app.blocks').directive('trackActive', ['$location', function ($location) {
             function link(scope, element) {
                 scope.$watch(function () {
                     return $location.path();
                 }, function () {
                     var links = element.find('a');
                     links.removeClass('active');
                     links.parents('li').removeClass('active');

                     angular.forEach(links, function (value) {
                         var a = angular.element(value);
                         var url = '#!' + $location.path();
                         var split = url.split('/');
                         var urlsplit = split[0] + '/' + split[1];
                         if (urlsplit === '#!/1') {
                             $('.sidebar-nav li#active').addClass('active');
                         }
                         if (a.attr('href') === urlsplit) {
                             a.parent('ul.nav-second-level li').addClass('active');
                             a.addClass('active').parents('ul.nav-second-level li').addClass('active');
                             a.addClass('active').parents('ul.nav-second-level li').siblings();


                         }
                     });
                 });
             }
             return { link: link };
         }]);
})();

