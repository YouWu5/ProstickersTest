(function () {
    'use strict';

    angular
        .module('app.orderTracking')
        .controller('OrderTrackingUpdate', OrderTrackingUpdate);

    OrderTrackingUpdate.$inject = ['$location', '$state', '$scope', 'message', 'stackView', '$ngBootbox', '$timeout', 'OrderTrackingUpdateFactory', 'helper', 'initialDataOfOrderTrackingUpdate'];

    function OrderTrackingUpdate($location, $state, $scope, message, stackView, $ngBootbox, $timeout, OrderTrackingUpdateFactory, helper, initialDataOfOrderTrackingUpdate) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Update Tracking Number';

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfOrderTrackingUpdate.viewModel.ReturnedData;
            console.log('fo.vm @ initialize', fo.vm);
            if (fo.vm.ImageBuffer && fo.vm.ImageBuffer.ImageBuffer !== null && fo.vm.ImageBuffer.ImageBuffer !== ' ' && fo.vm.ImageBuffer.ImageBuffer !== undefined) {
                fo.lv.uploadImage = 'data:image/png;base64,' + fo.vm.ImageBuffer.ImageBuffer.toString();
            }
            else {
                fo.lv.uploadImage = ' ';
            }
        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

        fo.Save = function () {
            if ($scope.TrackingUpdateForm.$invalid) {
                console.log('$scope.TrackingUpdateForm', $scope.TrackingUpdateForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            OrderTrackingUpdateFactory.submit(fo.vm).then(function (data) {
                if (data.Result === 1) // Success
                {
                    message.showServerSideMessage(data, true);
                    $state.go('OrderTrackingList');
                }
                helper.setIsSubmitted(false);
            });
        };

        fo.Cancel = function () {
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
            if ($scope.TrackingUpdateForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.downloadFile = function () {
            OrderTrackingUpdateFactory.downloadFile(fo.vm.DesignNumber).then(function (data) {
                console.log('data', data);
                if (data !== null) {
                    fo.lv.image = 'data:image/png;base64,' + data.ReturnedData.ImageBuffer.toString();
                    var b = angular.element('#downloadedImg');
                    var a = b[0];
                    a.href = fo.lv.image; a.download = fo.vm.DesignNumber; a.click();
                }
            });
        };

        ///////////////// Click Methods Ends Here ///////////////////

        ////////////////// Helper methods starts Here ////////////////

    }
})();
