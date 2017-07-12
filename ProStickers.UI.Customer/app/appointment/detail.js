(function () {
    'use strict';

    angular
        .module('app.appointment')
        .controller('AppointmentDetail', AppointmentDetail);

    AppointmentDetail.$inject = ['helper', 'message', '$state', '$scope', 'stackView', '$ngBootbox', '$timeout', '$stateParams', 'initialDataOfAppointmentDetail', 'appointmentDetailFactory'];

    function AppointmentDetail(helper, message, $state, $scope, stackView, $ngBootbox, $timeout, $stateParams, initialDataOfAppointmentDetail, appointmentDetailFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Appointment Detail';

        function initializeController() {

            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'AppointmentDetail') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                stackView.discardViewDetail();
            }
            else {
                fo.vm = initialDataOfAppointmentDetail.viewModel.ReturnedData;
            }
             
            console.log('fo.vm', fo.vm);
            fo.lv.status = fo.vm.AppointmentStatus;
            if (fo.lv.status === 'Scheduled') {
                fo.lv.isSaveEnable = false;
                fo.lv.isReasonDisable = false;
                fo.lv.isCancellationReason = false;
                fo.lv.iscancelAppointmentEnable = true;
                fo.lv.showDesignDetail = false;
            }
            else if (fo.lv.status === 'Initiated') {
                fo.lv.isSaveEnable = false;
                fo.lv.isReasonDisable = false;
                fo.lv.isCancellationReason = false;
                fo.lv.iscancelAppointmentEnable = false;
                fo.lv.showDesignDetail = false;
            }
            else if (fo.lv.status === 'Cancelled') {
                fo.lv.isSaveEnable = false;
                fo.lv.isReasonDisable = true;
                fo.lv.isCancellationReason = true;
                fo.lv.iscancelAppointmentEnable = false;
                fo.lv.showDesignDetail = false;
            }
            else if (fo.lv.status === 'Completed') {
                fo.lv.isSaveEnable = false;
                fo.lv.isReasonDisable = false;
                fo.lv.isCancellationReason = false;
                fo.lv.iscancelAppointmentEnable = false;
                fo.lv.showDesignDetail = true;
            }

            if (fo.vm.ImageBuffer !== null && fo.vm.ImageBuffer !== ' ') {
                fo.lv.uploadImage = 'data:image/png;base64,' + fo.vm.ImageBuffer.toString();
            }
        }
        initializeController();

        fo.save = function () {
            console.log('fo.vm at submit detail', fo.vm);
            if ($scope.AppointmentDetailForm.$invalid) {
                console.log('$scope.AppointmentDetailForm', $scope.AppointmentDetailForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            else {
                appointmentDetailFactory.submit(fo.vm).then(function (data) {
                    console.log('submit response at detail', data);
                    if (data.Result === 1)          // Success
                    {
                        message.showServerSideMessage(data, true);
                        $scope.AppointmentDetailForm.$setPristine();
                        stackView.closeThisView();
                    }
                    helper.setIsSubmitted(false);
                });
            }
        };

        fo.cancelAppointment = function () {
            fo.lv.iscancelAppointmentEnable = false;
            fo.lv.isCancellationReason = true;
            fo.lv.isSaveEnable = true;
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
            if ($scope.AppointmentDetailForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.buy = function () {
            stackView.pushViewDetail({
                controller: 'AppointmentDetail',
                formObject: fo, url: 'AppointmentDetail',
                formName: 'AppointmentDetail'
            });
            $state.go('OrdersCreate', { DesignNumber: fo.vm.DesignNumber, AppointmentNumber: fo.vm.AppointmentNumber, redirect: true });
        };

    }
})();
