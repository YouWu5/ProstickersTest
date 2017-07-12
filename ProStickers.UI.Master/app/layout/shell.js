(function () {
    'use strict';

    angular
        .module('app.layout')
        .controller('Shell', Shell);

    Shell.$inject = ['userInfo', '$location', 'GoogleSignin', 'message', 'helper', '$state',
        'stackView', 'shellFactory', '$scope', '$http', 'localStorageService', '$window', 'appUrl', 'AppointmentCreateFactory'];

    function Shell(userInfo, $location, GoogleSignin, message, helper, $state, stackView, shellFactory, $scope, $http, localStorageService, $window, appUrl, AppointmentCreateFactory) {
        var shl = this;
        shl.vm = {};
        shl.lv = {};
        shl.lv.title = 'Add request for appointment';
        shl.lv.redirectToShell = false;
        shl.lv.AssignedModuleList = [];
        shl.vm.isRequiredLogin = false;
        shl.lv.UserName = '';
        shl.lv.errorMessage = null;
        shl.lv.isAgree = false;
        shl.lv.topHeightPaddingOne = null;
        shl.lv.setFooterPaddingForm = null;
        shl.lv.setAgreementContainerHeight = null;
        shl.lv.form = false;
        shl.lv.isFormInvalid = false;
        shl.lv.appointmentCount = 0;

        var initializeController = function () {
            var session = localStorageService.get('UserSession');
            if (session !== null) {
                shl.lv.UserName = session.Name;
                shl.lv.AssignedModuleList = session.AssignedPageList;
                shl.vm.isRequiredLogin = true;
                if (session.UserTypeID === 1) { shl.lv.IsMaster = true; }
                if (session.UserTypeID === 2) { shl.lv.IsMaster = false; }
                if (location.hash === '#!/') {
                    $state.go('/');
                }
            }
            if (shl.lv.IsMaster === false && shl.vm.isRequiredLogin === true) {
                getCount();
            }
            shl.lv.errorMessage = localStorageService.get('LoginErrorMessage');
        };

        initializeController();

        function getCount() {
            AppointmentCreateFactory.getAppointmentRequestCount().then(function (data) {
                shl.lv.appointmentCount = data;
            });
            setTimeout(getCount, 45000);
        }

        shl.initializeViewsStack = function () {
            stackView.clearViewsStack();
            stackView.pushViewDetail({
                controller: 'HomeController', formObject: null, url: '/1', formName: 'Home'
            });
        };


        ///mobile menu toggle 
        shl.menuToggle = function () {
            $('#wrapper').toggleClass('active');
            $('.overlay').toggleClass('Hidden');
        };

        shl.menuTogglepage = function () {
            $('#wrapper').removeClass('active');
            $('.overlay').removeClass('Hidden');
        };

        shl.LogOut = function () {
            localStorageService.set('UserSession', null);
            shellFactory.LogOut();
            localStorageService.set('AuthenticateData', null);
            shl.vm.isRequiredLogin = false;
        };
        shl.Login = function () {
            GoogleSignin.signIn().then(function (response) {
                console.log('GoogleSignin', response);

                var loginInfo = GoogleSignin.getBasicProfile();
                var model = {};
                model.Token = response.Zi.id_token;
                model.Provider = 'google';
                model.UserTypeID = 1;
                model.UserID = loginInfo.email;
                shellFactory.ObtainAccessToken(model).then(function (res) {
                    localStorageService.set('accessToken', res.ReturnedData);
                    console.log('res', res);
                    shellFactory.GetUserSession(loginInfo.email, loginInfo.id).then(function (data) {
                        console.log('data', data);
                        if (data.ReturnedData !== null) {
                            userInfo = data;
                            shl.lv.UserName = data.ReturnedData.Name;
                            localStorageService.set('UserSession', data.ReturnedData);
                            shl.lv.AssignedModuleList = data.ReturnedData.AssignedPageList;
                            shl.vm.isRequiredLogin = true;
                            if (data.ReturnedData.UserTypeID === 1) { shl.lv.IsMaster = true; }
                            if (data.ReturnedData.UserTypeID === 2) { shl.lv.IsMaster = false; getCount(); }
                        } else {
                            shl.lv.errorMessage = data.Message;
                        }
                    });
                });
            });
        };
        ////////////////////////////-------appointment popup code start------///////////////////////////



        shl.overlayForm = function () {
            if (shl.lv.form === false) {
                shl.lv.fulldayList = [];
                shl.lv.messages = '';
                shl.lv.color = '';
                helper.setIsSubmitted(false);
                var date = helper.ConvertDateCST(new Date());
                shl.lv.counter = 45;
                setTimeout(reduceCount, 1000);
                AppointmentCreateFactory.getDefault().then(function (data) {
                    console.log('viewmodel', data);
                    if (data !== null) {
                        AppointmentCreateFactory.getAppointmentRequestCount().then(function (data) {
                            shl.lv.appointmentCount = data;
                        });
                        shl.lv.form = true;
                        shl.lv.viewmodel = data;
                        shl.lv.AppointmentDate = shl.lv.viewmodel.AppointmentDate === '0001-01-01T00:00:00' ? new Date(date) : angular.copy(shl.lv.viewmodel.AppointmentDate);
                        shl.lv.startDateOptions = {
                            startingDay: 1,
                            showWeeks: false,
                            initDate: null,
                            minDate: date
                        };
                        shl.lv.minDate = date;
                        AppointmentCreateFactory.getSlotList(helper.formatDate(date)).then(function (data) {
                            if (data.length === 0) {
                                shl.lv.messages = 'You can not book appointment as you have not saved any available slot for this day.';
                                shl.lv.color = 'red';
                            }
                            shl.lv.fulldayList = data;
                            if (shl.lv.fulldayList.length > 0) {
                                shl.lv.viewmodel.TimeSlotID = shl.lv.fulldayList[0].Value;
                            }
                        });
                    }
                    else {
                        message.showClientSideErrors('Data is already changed by someone, Please try again.');
                        AppointmentCreateFactory.getAppointmentRequestCount().then(function (data) {
                            shl.lv.appointmentCount = data;
                        });
                        return;
                    }
                });
            }
        };

        shl.isSubmitted = function () {
            return helper.getIsSubmitted();
        };

        shl.open = function ($event, opened) {
            $event.preventDefault();
            $event.stopPropagation();
            if (shl.openedStart === true) {
                shl.openedStart = false;
            }
            else if (opened === 'openedStart') {

                shl.openedEnd = false;
                shl.openedStart = true;
            }
            if (shl.openedEnd === true) {
                shl.openedEnd = false;
            }
            else if (opened === 'openedEnd') {
                shl.openedStart = false;
                shl.openedEnd = true;
            }
        };


        shl.Close = function () {
            shl.lv.form = false;
            AppointmentCreateFactory.cancelRequest(shl.lv.viewmodel).then(function (data) {
                message.showServerSideMessage(data, true);
                AppointmentCreateFactory.getAppointmentRequestCount().then(function (data) {
                    shl.lv.appointmentCount = data;
                });
            });
        };

        shl.save = function () {
            shl.lv.messages = '';
            if (shl.lv.fulldayList.length > 0) {
                for (var i = 0; i < shl.lv.fulldayList.length; i++) {
                    if (shl.lv.fulldayList[i].Value === shl.lv.viewmodel.TimeSlotID) {
                        shl.lv.viewmodel.TimeSlot = shl.lv.fulldayList[i].Text;
                        shl.lv.viewmodel.EndTime = shl.lv.fulldayList[i].EndTime;
                        shl.lv.viewmodel.StartTime = shl.lv.fulldayList[i].StartTime;
                    }
                }
            }
            else {
                shl.lv.messages = 'You can not book appointment as you have not saved any available slot for this day.';
                shl.lv.color = 'red';
                return;
            }

            if (shl.lv.viewmodel.TimeSlot === null || shl.lv.viewmodel.TimeSlot === undefined ||
                shl.lv.AppointmentDate === null || shl.lv.AppointmentDate === undefined) {
                return;
            }

            shl.lv.viewmodel.AppointmentDate = angular.copy(helper.formatDateTime(shl.lv.AppointmentDate));
            console.log('view model on save', angular.toJson(shl.lv.viewmodel));
            helper.setIsSubmitted(true);
            shl.lv.form = false;
            AppointmentCreateFactory.submit(shl.lv.viewmodel).then(function (data) {
                console.log('data after save', data);
                AppointmentCreateFactory.getAppointmentRequestCount().then(function (data) {
                    shl.lv.appointmentCount = data;
                });
                message.showServerSideMessage(data, true);
                helper.setIsSubmitted(false);
            });
        };

        shl.setTime = function () {
            if (shl.lv.AppointmentDate !== undefined || shl.lv.AppointmentDate !== null) {
                AppointmentCreateFactory.getSlotList(helper.formatDate(shl.lv.AppointmentDate)).then(function (data) {
                    shl.lv.fulldayList = data;
                    if (data.length === 0) {
                        shl.lv.messages = 'You can not book appointment as you have not saved any available slot for this day.';
                        shl.lv.color = 'red';
                        shl.lv.viewmodel.TimeSlotID = null;
                        shl.lv.viewmodel.TimeSlot = null;
                        return;
                    }
                    shl.lv.messages = '';
                    shl.lv.viewmodel.TimeSlotID = shl.lv.fulldayList[0].Value;
                });
            }
        };

        function reduceCount() {
            if (shl.lv.form === true) {
                if (shl.lv.counter > 0) {
                    shl.lv.counter = shl.lv.counter - 1;
                    setTimeout(reduceCount, 1000);
                }
                if (shl.lv.counter === 0) {
                    AppointmentCreateFactory.cancelRequest(shl.lv.viewmodel).then(function (data) {
                        message.showServerSideMessage(data, true);
                        AppointmentCreateFactory.getAppointmentRequestCount().then(function (data) {
                            shl.lv.appointmentCount = data;
                        });
                        shl.lv.form = false;
                    });
                }
            }
        }
        ////////////////////////////-------appointment popup code end------///////////////////////////

    }
})();
