(function () {
    'use strict';

    angular
        .module('app.availabilityTime')
        .controller('AvailabilityTime', AvailabilityTime);

    AvailabilityTime.$inject = ['$location', '$state', 'stackView', 'helper', '$scope', 'message', 'AvailabilityTimeFactory'];

    function AvailabilityTime($location, $state, stackView, helper, $scope, message, AvailabilityTimeFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Availability Time';
        fo.lv.setFooterPaddingRecord = null;
        fo.lv.currentDate = helper.ConvertDateCST(new Date());
        fo.lv.Date = null;
        fo.lv.openDays = 'halfday';
        fo.lv.showTimeSlot = false;
        fo.lv.isFormInvalid = false;
        function initilizeController() {
            helper.setIsSubmitted(false);
            var obj = stackView.getLastViewDetail();
            console.log('obj', obj);
            if (obj.formName === 'AvailabilityTime') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
            }

            else {
                fo.lv.startDateOptions = {
                    startingDay: 1,
                    showWeeks: false,
                    initDate: null,
                    minDate: fo.lv.currentDate
                };
            }
        }

        initilizeController();

        fo.open = function ($event, opened) {
            $event.preventDefault();
            $event.stopPropagation();
            if (fo.openedStart === true) {
                fo.openedStart = false;
            }
            else if (opened === 'openedStart') {
                fo.openedEnd = false;
                fo.openedStart = true;
            }
            if (fo.openedEnd === true) {
                fo.openedEnd = false;
            }
            else if (opened === 'openedEnd') {
                fo.openedStart = false;
                fo.openedEnd = true;
            }
        };

        fo.setTime = function () {
            fo.lv.showTimeSlot = false;
        };

        fo.search = function () {
            if ($scope.AvailabilityTimeFrom.$invalid) {
                fo.lv.isFormInvalid = true;
                return;
            }

            AvailabilityTimeFactory.getList(helper.formatDate(fo.lv.Date), false).then(function (data) {
                console.log('data', data);
                fo.vm = data;
                fo.lv.openDays = 'halfday';
                fo.lv.showTimeSlot = true;
                isAll();
            });

        };

        fo.isSubmitted = function () {
            return helper.getIsSubmitted();
        };

        fo.save = function () {
            if ($scope.AvailabilityTimeFrom.$invalid) {
                fo.lv.isFormInvalid = true;
                return;
            }

            console.log('viewmodel on save', angular.toJson(fo.vm));
            helper.setIsSubmitted(true);
            AvailabilityTimeFactory.submit(fo.vm).then(function (data) {
                console.log('data after save', data);
                message.showServerSideMessage(data, false);
                fo.lv.showTimeSlot = false;
                fo.lv.Date = null;
                $scope.AvailabilityTimeFrom.$setUntouched();
                helper.setIsSubmitted(false);
            });
        };

        fo.slotChanged = function (value) {
            var date = angular.copy(helper.formatDate(fo.lv.Date));
            if (value === 'fullday') {
                AvailabilityTimeFactory.getList(date, true).then(function (data) {
                    fo.vm = data;
                    isAll();
                });
            }
            if (value === 'halfday') {
                AvailabilityTimeFactory.getList(date, false).then(function (data) {
                    fo.vm = data;
                    isAll();
                });
            }

        };

        fo.checkboxClicked = function (type) {
            var count = 0;
            if (type === 'one') {
                fo.lv.IsChecked = false;
                for (var s = 0; s < fo.vm.TimeSlotList.length; s++) {
                    if (fo.vm.TimeSlotList[s].IsChecked === true) {
                        fo.vm.TimeSlotList[s].TimeSlotStatus = 1;
                        count++;
                    }
                    if (fo.vm.TimeSlotList[s].IsChecked === false) {
                        fo.vm.TimeSlotList[s].TimeSlotStatus = 0;
                    }
                }
                if (count === fo.vm.TimeSlotList.length) {
                    fo.lv.IsChecked = true;
                }
            }
            if (type === 'all') {
                if (fo.lv.IsChecked === true) {
                    for (var p = 0; p < fo.vm.TimeSlotList.length; p++) {
                        fo.vm.TimeSlotList[p].IsChecked = true;
                        fo.vm.TimeSlotList[p].TimeSlotStatus = 1;
                    }
                }
                if (fo.lv.IsChecked === false) {
                    for (var q = 0; q < fo.vm.TimeSlotList.length; q++) {
                        fo.vm.TimeSlotList[q].IsChecked = false;
                        fo.vm.TimeSlotList[q].TimeSlotStatus = 0;
                    }
                }
            }
        };

        function isAll() {
            fo.lv.IsChecked = false;
            var count = 0;
            for (var j = 0; j < fo.vm.TimeSlotList.length; j++) {
                if (fo.vm.TimeSlotList[j].TimeSlotStatus === 0) {
                    fo.vm.TimeSlotList[j].IsChecked = false;
                }

                else {
                    count++;
                    fo.vm.TimeSlotList[j].IsChecked = true;
                }
            }
            if (count === fo.vm.TimeSlotList.length) {
                fo.lv.IsChecked = true;
            }
        }

    }
})();
