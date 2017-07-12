//(function () {
//    'use strict';

//    angular
//        .module('app.availabilityTime')
//        .controller('CreateAppointmentRequest', CreateAppointmentRequest);

//    CreateAppointmentRequest.$inject = ['$location', '$state', 'stackView', '$scope', 'message', 'helper', '$ngBootbox', '$timeout', 'AppointmentCreateFactory', 'InitialDataOfAppointmentCreate'];

//    function CreateAppointmentRequest($location, $state, stackView, $scope, message, helper, $ngBootbox, $timeout, AppointmentCreateFactory, InitialDataOfAppointmentCreate) {
//        /* jshint validthis:true */
//        var fo = this;
//        shl.vm = {};
//        shl.lv = {};
//        shl.lv.title = 'Add Request for Appointment';
//        shl.lv.setFooterPaddingRecord = null;

//        shl.lv.isFormInvalid = false;

//        function initilizeController() {
//            helper.setIsSubmitted(false);
//            shl.vm = InitialDataOfAppointmentCreate.viewModel;

//            shl.lv.fulldayList = InitialDataOfAppointmentCreate.slotList;
//            shl.vm.AppointmentDate = shl.vm.AppointmentDate === '0001-01-01T00:00:00' ? new Date() : shl.vm.AppointmentDate;
//            shl.vm.TimeSlotID = null;
//            shl.lv.startDateOptions = {
//                startingDay: 1,
//                showWeeks: false,
//                initDate: null,
//                minDate: new Date(shl.vm.AppointmentDate)
//            };
//        }

//        initilizeController();

//        fo.isSubmitted = function () {
//            return helper.getIsSubmitted();
//        };

//        fo.open = function ($event, opened) {
//            $event.preventDefault();
//            $event.stopPropagation();
//            if (fo.openedStart === true) {
//                fo.openedStart = false;
//            }
//            else if (opened === 'openedStart') {

//                fo.openedEnd = false;
//                fo.openedStart = true;
//            }
//            if (fo.openedEnd === true) {
//                fo.openedEnd = false;
//            }
//            else if (opened === 'openedEnd') {
//                fo.openedStart = false;
//                fo.openedEnd = true;
//            }
//        };

//        //fo.Close = function () {
//        //    var obj = stackView.getLastViewDetail();
//        //    var options = {
//        //        message: 'Do you want to close the form?',
//        //        buttons: {
//        //            success: {
//        //                label: ' ',
//        //                className: 'fa fa-check-page',
//        //                callback: function () {
//        //                    $timeout(function () {
//        //                    }, 100);
//        //                    if (obj.formName !== 'Home') {
//        //                        stackView.closeView();
//        //                        return;
//        //                    }
//        //                    else {
//        //                        stackView.openView('AvailabilityTime');
//        //                    }
//        //                }
//        //            }
//        //        }
//        //    };
//        //    if ($scope.AppointmentRequestForm.$dirty) {
//        //        $ngBootbox.customDialog(options);
//        //    }
//        //    else {
//        //        if (obj.formName !== 'Home') {
//        //            stackView.closeView();
//        //            return;
//        //        }
//        //        else {
//        //            stackView.openView('AvailabilityTime');
//        //        }
//        //    }
//        //};

//        fo.save = function () {
//            if ($scope.AppointmentRequestForm.$invalid) {
//                shl.lv.isFormInvalid = true;
//                helper.scrollToError();
//                return;
//            }

//            for (var i = 0; i < shl.lv.fulldayList.length; i++) {
//                if (shl.lv.fulldayList[i].Value === shl.vm.TimeSlotID) {
//                    shl.vm.TimeSlot = shl.lv.fulldayList[i].Text;
//                    shl.vm.EndTime = shl.lv.fulldayList[i].EndTime;
//                    shl.vm.StartTime = shl.lv.fulldayList[i].StartTime;
//                }
//            }

//            console.log('view model on save', shl.vm);
//            helper.setIsSubmitted(true);
//            AppointmentCreateFactory.submit(shl.vm).then(function (data) {
//                console.log('data after save', data);
//                message.showServerSideMessage(data, true);
//                helper.setIsSubmitted(false);
//                $state.go('/');
//            });
//        };

//        fo.setTime = function () {
//            shl.vm.AppointmentDate = helper.formatDate(shl.vm.AppointmentDate);
//            AppointmentCreateFactory.getSlotList(shl.vm.AppointmentDate).then(function (data) {
//                if (data.length === 0) {
//                    message.showClientSideErrors('You can not book appointment as you have not saved any available slot for this day.');
//                }
//                shl.lv.fulldayList = data;
//            });
//        };
//    }
//})();
