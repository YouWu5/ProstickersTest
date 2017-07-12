(function () {
    'use strict';

    angular
        .module('app.ordersTracking')
        .controller('TrackingDetail', TrackingDetail);

    TrackingDetail.$inject = ['$location', '$state', 'stackView', '$scope', 'message', 'helper', '$ngBootbox', '$timeout', 'InitialDataOfOrderDetail', 'OrderDetailFactory', 'spinnerService'];

    function TrackingDetail($location, $state, stackView, $scope, message, helper, $ngBootbox, $timeout, InitialDataOfOrderDetail, OrderDetailFactory, spinnerService) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Update Tracking Number';

        initializeController();

        function initializeController() {
            helper.setIsSubmitted(false);
            console.log('InitialDataOfOrderDetail', InitialDataOfOrderDetail);
            fo.vm = InitialDataOfOrderDetail.viewModel.ReturnedData;
            if (fo.vm.ImageBuffer) {
                fo.lv.image = 'data:image/png;base64,' + (fo.vm.ImageBuffer.ImageBuffer).toString();
            }
        }

        fo.isSubmitted = function () {
            return helper.getIsSubmitted();
        };

        fo.download = function () {
            spinnerService.show('pageContainerSpinner');
            OrderDetailFactory.downloadFile(fo.vm.DesignNumber).then(function (data) {
                if (data !== null) {
                    var b = angular.element('#downloadedImg'); var a = b[0];
                    var byteCharacters = atob(data.ReturnedData.ImageBuffer.toString());
                    var byteNumbers = new Array(byteCharacters.length);
                    for (var i = 0; i < byteCharacters.length; i++) {
                        byteNumbers[i] = byteCharacters.charCodeAt(i);
                    }
                    var byteArray = new Uint8Array(byteNumbers);
                    var blob = new Blob([byteArray], { type: data.ReturnedData.FileExtension });
                    var blobUrl = URL.createObjectURL(blob);
                    a.href = blobUrl; a.download = fo.vm.DesignNumber; a.click();
                }
            });
        };

        fo.save = function () {
            if ($scope.TrackingUpdateForm.$invalid) {
                fo.lv.isFormInvalid = true;
                helper.scrollToError();
                return;
            }
            helper.setIsSubmitted(true);
            console.log('viewmodel on submit', angular.toJson(fo.vm));
            OrderDetailFactory.submit(fo.vm).then(function (data) {
                message.showServerSideMessage(data, true);
                console.log('dataa', data);
                $state.go('Tracking');
                helper.setIsSubmitted(false);
            });
        };

        fo.Close = function () {
            var obj = stackView.getLastViewDetail();
            var options = {
                message: 'Do you want to close the form?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            if (obj.formName !== 'Home') {
                                stackView.closeView();
                                return;
                            }
                            else {
                                stackView.openView('Tracking');
                            }
                        }
                    }
                }
            };

            if ($scope.TrackingUpdateForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                if (obj.formName !== 'Home') {
                    stackView.closeView();
                    return;
                }
                else {
                    stackView.openView('Tracking');
                }
            }
        };

    }
})();
