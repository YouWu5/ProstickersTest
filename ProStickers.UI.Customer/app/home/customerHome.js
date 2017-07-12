(function () {
    'use strict';

    angular
        .module('app.home')
        .controller('CustomerHome', CustomerHome);

    CustomerHome.$inject = ['stackView', 'localStorageService', 'helper', '$location', '$state', '$ngBootbox', '$timeout', 'customerHomeFactory'];

    function CustomerHome(stackView, localStorageService, helper, $location, $state, $ngBootbox, $timeout, customerHomeFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Pro Stickers';
        fo.lv.appointmentList = [];
        fo.lv.ordersList = [];
        fo.lv.designList = [];
        fo.lv.filesList = [];

        function initializeController() {
            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'CustomerHomeForm') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                stackView.discardViewDetail();
            }
            else {
                customerHomeFactory.getDetailLists().then(function (data) {
                    console.log('data', data);
                    fo.lv.appointmentList = data.AppointmentList;
                    fo.lv.designList = data.DesignList;
                    fo.lv.ordersList = data.OrderList;
                    fo.lv.filesList = data.FilesList;
                    for (var i2 = 0; i2 < fo.lv.designList.length; i2++) {
                        if (fo.lv.designList[i2].DesignImage && fo.lv.designList[i2].DesignImage.ImageBuffer !== null && fo.lv.designList[i2].DesignImage.ImageBuffer !== ' ') {
                            fo.lv.designList[i2].DesignImage.ImageBuffer = 'data:image/png;base64,' + fo.lv.designList[i2].DesignImage.ImageBuffer.toString();
                        }
                    }
                });
            }
            console.log('fo.lv', fo.lv);
        }
        initializeController();

        fo.moreAppointment = function () {
            $state.go('AppointmentList');
        };

        fo.moreOrders = function () {
            $state.go('OrdersList');
        };

        fo.moreDesigns = function () {
            $state.go('DesignsList');
        };

        fo.profile = function () {
            $state.go('CustomerProfile');
        };

        fo.moreFiles = function () {
            $state.go('FilesList');
        };

        fo.OpenAppointmentDetail = function (AppointmentNumber) {
            stackView.pushViewDetail({
                controller: 'CustomerHome',
                formObject: fo, url: '/',
                formName: 'CustomerHomeForm'
            });
            $state.go('AppointmentDetail', { Number: AppointmentNumber, redirect: true });
        };

        fo.OpenOrderDetail = function (OrderNumber) {
            stackView.pushViewDetail({
                controller: 'CustomerHome',
                formObject: fo, url: '/',
                formName: 'CustomerHomeForm'
            });
            $state.go('OrdersDetail', { Number: OrderNumber, redirect: true });
        };

        fo.OpenDesignDetail = function (DesignNo) {
            stackView.pushViewDetail({
                controller: 'CustomerHome',
                formObject: fo, url: '/',
                formName: 'CustomerHomeForm'
            });
            $state.go('DesignsDetail', { Number: DesignNo, redirect: true });
        };

        fo.addAppointment = function () {
            stackView.pushViewDetail({
                controller: 'CustomerHome',
                formObject: fo, url: '/',
                formName: 'CustomerHomeForm'
            });
            $state.go('AppointmentCalendar', { redirect: true });
        };

        fo.contactMe = function () {

            var userID = localStorageService.get('UserSession');
            console.log('userID', userID);
            fo.vm.CustomerID = userID.UserID;
            fo.vm.RequestDateTime = helper.ConvertDateCST(new Date());
            console.log('contact me submit vm', fo.vm);

            customerHomeFactory.contactMe(fo.vm).then(function (data) {
                fo.lv.msg = data.Message.split('.');

                if (fo.lv.msg[1] !== '') {
                    fo.lv.msg[1] = fo.lv.msg[1] + '.';
                }

                if (fo.lv.msg[2] !== '' && fo.lv.msg[2] !== undefined) {
                    fo.lv.msg[2] = fo.lv.msg[2] + '.';
                }
                else {
                    fo.lv.msg[2] = ' ';
                }

                var dataMsg = '<div style="text-align: center">' +
                              '<div>' + fo.lv.msg[0] + '.' + '</div>' +
                              '<div>' + fo.lv.msg[1] + '</div>' +
                              '<div>' + fo.lv.msg[2] + '</div>' +
                              '</div>';

                var options = {
                    title: 'Contact Me Request',
                    message: dataMsg,
                    buttons: {
                        success: {
                            label: ' ',
                            className: 'fa fa-check-page',
                            callback: function () {
                                $timeout(function () {
                                }, 100);

                            }
                        }
                    }
                };
                $ngBootbox.customDialog(options);

            });

        };

    }
})();
