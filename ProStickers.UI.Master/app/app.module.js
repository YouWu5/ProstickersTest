/* jshint -W117*/
(function () {
    'use strict';

    angular.module('app', [
        // Angular modules 
        'ui.router',
        'ngMessages',
        'google-signin',
        // Custom modules
        'app.blocks',
        'app.core',
        'app.layout',
        'app.home',
        'app.availabilityTime',
        'app.appointment',
        'app.ordersTracking',
        'app.colorChart',
        'app.customers',
        'app.feedback',
        'app.orderTracking',
        'app.predefinedSize',
        'app.report',
        'app.users',
        // 3rd Party Modules
        'ui.bootstrap',
        'angularSpinners',
        'ngBootbox',
        'LocalStorageModule',
        'jlareau.bowser'
    ]);

    // angular.module('app').constant('appUrl', 'https://api.prostickerslive.com/');
    angular.module('app').constant('appUrl', 'https://prostickerstest-api.azurewebsites.net/');

    angular.module('app').value('userInfo', {});

    customInterceptor.$inject = ['$q', '$injector', 'appUrl', '$location', '$rootScope', 'message', 'spinnerService', 'helper', 'localStorageService'];
    function customInterceptor($q, $injector, appUrl, $location, $rootScope, message, spinnerService, helper, localStorageService) {
        return {
            'responseError': function (rejection) {
                // do something on error
                if (rejection !== undefined && rejection !== null) {
                    if (rejection.status === 400) {

                        var errorObject = rejection.data.Message;
                        helper.setIsSubmitted(false);
                        if (errorObject !== null && errorObject !== '' && errorObject !== undefined && errorObject.Result !== null &&
                            errorObject.Result !== undefined && errorObject.Result !== '') {
                            var result = errorObject.Result;
                            if (result === 2 || result === 3 || result === 5) {                                 //2 for UDError and 3 for SDError , 5 for concurrency
                                message.showClientSideErrors(errorObject.Message);
                            }
                            else {
                                console.log('API throws Error for Developer: ', errorObject);  // dont remove this logger this is used to get error for developmen puropse.

                            }
                        }
                    }

                    if (rejection.status === 401) {
                        localStorageService.set('LoginErrorMessage', rejection.data.Message);
                        localStorageService.set('UserSession', null);
                        window.location.reload();
                        console.log('User is unauthorized, please login again');
                    }

                    if (rejection.status === 404) {
                        message.showClientSideErrors('There was an error connecting to server.');
                    }

                    if (rejection.status === 409) {
                        console.log('Multiple login for same account');
                    }

                    if (rejection.status === 410) {
                        localStorageService.set('LoginErrorMessage', 'Session expired. Please login again');
                        localStorageService.set('accessToken', null);
                        localStorageService.set('UserSession', null);
                        window.location.reload();
                        console.log('Session Expired');
                    }

                    if (rejection.status === 500) {
                        helper.setIsSubmitted(false);
                        message.showClientSideErrors('Oops! something went wrong. Please try again.');
                    }

                    if (rejection.status === 0) {
                        helper.setIsSubmitted(false);
                        message.showClientSideErrors('There is network problem. Please check Internet connection.');
                    }

                    if (rejection.config.method === 'PUT' || rejection.config.method === 'POST') {
                        $rootScope.$broadcast('httpLoaderEnd', rejection.config.headers.loaderID);
                    }
                    if (rejection.status === 406 || rejection.status === 412) {
                        helper.setIsSubmitted(false);
                        message.showClientSideErrors(rejection.data.Message);
                    }
                    spinnerService.hide('layoutSpinner');
                    spinnerService.hide('pageContainerSpinner');
                    return $q.reject(rejection);
                }
                else {
                    spinnerService.hide('layoutSpinner');
                    spinnerService.hide('pageContainerSpinner');
                    return $q.reject(rejection);
                }

            },

            'request': function (config) {

                var accessToken = localStorageService.get('accessToken');

                if (accessToken !== null) {
                    config.headers['x-zumo-auth'] = accessToken;

                }
                var usersession = localStorageService.get('UserSession');
                if (usersession !== null) {
                    config.headers.usersession = angular.toJson(usersession);
                }
                if (config.method !== 'GET') {
                    $rootScope.$broadcast('httpLoaderStart', 'pageContainerSpinner');
                }
                return config;
            },

            'response': function (response) {
                if (response.config.method !== 'GET') {
                    $rootScope.$broadcast('httpLoaderEnd', 'pageContainerSpinner');
                }
                return response;
            }

        };
    }
    angular.module('app').factory('httpRequestInterceptor', customInterceptor);

    function runConfig($rootScope, $state, message, spinnerService, localStorageService, bowser) {

        $rootScope.$on('$stateChangeStart', stateChangeStart);
        $rootScope.$on('$stateChangeSuccess', stateChangeSuccess);
        $rootScope.$on('$stateChangeError', stateChangeError);

        $rootScope.$state = $state;
        $rootScope.$on('httpLoaderStart', function (event, loaderID) {
            spinnerService.show(loaderID, 'Loading...');
        });

        $rootScope.$on('httpLoaderEnd', function (event, loaderID) {
            spinnerService.hide(loaderID);
        });


        function stateChangeStart() {
            if (bowser.chrome === true || bowser.msedge === true) {
                message.showServerSideMessage(null, false);
            }
            var winEvent = window.event;
            if (winEvent !== undefined && winEvent instanceof ProgressEvent) {
                spinnerService.hide('layoutSpinner');
                event.preventDefault();
            }
            else {
                window.history.forward();
                message.clearMessage();
                spinnerService.show('layoutSpinner');
            }
            document.body.scrollTop = document.documentElement.scrollTop = 0;
        }

        function stateChangeSuccess() {
            var session = localStorageService.get('UserSession');
            var count = 0;
            if (session !== null) {
                if ($state.current.name !== '/' && $state.current.name !== '' && $state.current.name !== '404') {
                    for (var i = 0; i < session.AssignedPageList.length; i++) {
                        if ($state.current.name === session.AssignedPageList[i].Url) {
                            count++;
                        }
                    }
                    //   if (count === 0) { $state.go('404'); }
                }
            }
            spinnerService.hide('layoutSpinner');
            document.body.scrollTop = document.documentElement.scrollTop = 0;
        }

        function stateChangeError() {
            spinnerService.hide('layoutSpinner');
        }

    }

    angular.module('app').filter('removeStringSpaces', [function () {
        return function (string) {
            if (!angular.isString(string)) {
                return string;
            }
            return string.replace(/[\s]/g, '');
        };
    }]);

    angular.module('app').config(['$httpProvider', 'GoogleSigninProvider', function ($httpProvider, GoogleSigninProvider) {
        $httpProvider.interceptors.push('httpRequestInterceptor');
        GoogleSigninProvider.init({ client_id: '925308745320-6mktlln37hndb1sqr296ec6g8ti7vav6.apps.googleusercontent.com' });
    }])
        .run(runConfig);

})();