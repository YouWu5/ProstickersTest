(function () {
    'use strict';

    angular
        .module('app.appointment')
        .controller('AppointmentCalendar', AppointmentCalendar);

    AppointmentCalendar.$inject = ['$location', 'message', '$state', 'helper', '$scope', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfAddCalendar', 'addCalendarFactory'];

    function AppointmentCalendar($location, message, $state, helper, $scope, stackView, $ngBootbox, $timeout, initialDataOfAddCalendar, addCalendarFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Add Calendar Appointment';
        fo.lv.timeSlotList = [];
        fo.lv.designerList = [];
        fo.lv.showMoreDesigner = false;
        fo.lv.daysList = [];
        fo.lv.selected = false;

        function initializeController() {

            var today = helper.ConvertDateCST(new Date());
            var c = 0;
            for (var i = 0; i < 14; i++) {
                fo.lv.daysList[c++] = {
                    Name: 'day ' + (i + 1), IsChecked: 'false', Date: new Date(today.getFullYear(), today.getMonth(), (today.getDate() + i),
                        today.getHours(), today.getMinutes(), today.getSeconds())
                };
            }

            fo.vm = initialDataOfAddCalendar.viewModel;
            fo.lv.timeSlotList = initialDataOfAddCalendar.timeSlotList;
            console.log('fo.vm at calendar', fo.vm);
            console.log('fo.lv.timeSlotList', fo.lv.timeSlotList);
        }

        initializeController();

        fo.select = function ($index) {
            fo.lv.selected = $index;
        };

        fo.selectDate = function (item) {
            fo.lv.selectedDate = helper.formatDate(item.Date);
            fo.vm.Date = helper.formatDate(item.Date);
            fo.vm.TimeSlotID = 0;
            fo.vm.UserID = null;
            fo.lv.designerName = null;
            fo.vm.RequestDateTime = helper.formatDateTime(item.Date);
            addCalendarFactory.getTimeSlotList(fo.lv.selectedDate).then(function (data) {
                fo.lv.timeSlotList = data;
            });
        };

        fo.selectTimeSlot = function (id) {
            console.log('time slot id', id);
            fo.vm.UserID = null;
            fo.lv.designerName = null;
            for (var k = 0; k < fo.lv.timeSlotList.length; k++) {
                if (id === fo.lv.timeSlotList[k].Value) {
                    fo.vm.TimeSlot = fo.lv.timeSlotList[k].Text;
                    fo.vm.EndTime = fo.lv.timeSlotList[k].EndTime;
                    fo.vm.StartTime = fo.lv.timeSlotList[k].StartTime;
                }
            }
             search();
        };

        fo.cancel = function () {
            var options = {
                message: 'Do you want to close the form?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            stackView.closeView();
                        }
                    }
                }
            };
            if ($scope.CalendarForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.moreDesigner = function (index) {
            fo.lv.showMoreDesigner = true;
            fo.vm.UserID = fo.lv.designerList[index];
            if (fo.vm.TimeSlotID && fo.lv.selectedDate) {
                addCalendarFactory.getAvailableDesignerList(fo.lv.selectedDate, fo.vm.TimeSlotID).then(function (data) {
                    fo.lv.designerList = data;
                    if (data.length === 0) {
                        message.showClientSideErrors('No Designer is available at the selected time. Please select another time.');
                    }
                    console.log('data response more designer', data);
                });
            }

        };

        function search() {
            if (fo.vm.TimeSlotID && fo.lv.selectedDate) {
                fo.lv.showMoreDesigner = false;
                if ($scope.CalendarForm.$invalid) {
                    console.log('$scope.CalendarForm', $scope.CalendarForm.$error);
                    helper.scrollToError();
                    fo.lv.isFormInvalid = true;
                    return;
                }
                else {
                    console.log('fo.vm', fo.vm);
                    if (fo.vm.TimeSlotID && fo.lv.selectedDate) {
                        addCalendarFactory.getAvailableDesigner(fo.lv.selectedDate, fo.vm.TimeSlotID).then(function (data) {
                            console.log('data', data);
                            if (data.Text === null) {
                                message.showClientSideErrors('No Designer is available at the selected time. Please select another time.');
                                return;
                            }
                            fo.lv.designerName = data.Text;
                            fo.vm.UserName = data.Text;
                            fo.vm.UserID = data.Value;
                            console.log('data response', data);
                        });
                    }
                }
            }
        }

        fo.setUserName = function (userID) {
            console.log('userID', userID);
            for (var i3 = 0; i3 < fo.lv.designerList.length; i3++) {
                if (userID === fo.lv.designerList[i3].Value) {
                    fo.vm.UserName = fo.lv.designerList[i3].Text;
                }
            }
        };

        fo.save = function () {
            fo.vm.TimeSlotList = [];
            console.log('fo.vm ', angular.toJson(fo.vm));
            if ($scope.CalendarForm.$invalid) {
                console.log('$scope.CalendarForm', $scope.CalendarForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            else {

                if (!fo.lv.designerName || !fo.vm.UserID) {
                    message.showClientSideErrors('No Designer is available at the selected time. Please select another time.');
                    return;
                }

                if (fo.vm.UserName === null || fo.vm.UserName === undefined || fo.vm.UserName === '') {
                    message.showClientSideErrors('No Designer is available at the selected time. Please select another time.');
                    return;
                }
                console.log('fo.vm check', fo.vm);
                addCalendarFactory.submit(fo.vm).then(function (data) {
                    console.log('submit response', data);

                    if (data.Result === 1)          // Success
                    {
                        message.showServerSideMessage(data, true);
                        $scope.CalendarForm.$setPristine();
                        stackView.closeView();
                    }
                    helper.setIsSubmitted(false);
                });
            }

        };

    }
})();
