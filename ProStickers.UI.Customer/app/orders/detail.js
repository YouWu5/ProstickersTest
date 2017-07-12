(function () {
    'use strict';

    angular
        .module('app.order')
        .controller('OrdersDetail', OrdersDetail);

    OrdersDetail.$inject = ['$location', 'helper', '$state', '$scope', 'stackView', '$ngBootbox', '$timeout', 'orderDetailFactory', 'initialDataOfOrderDetail'];

    function OrdersDetail($location, helper, $state, $scope, stackView, $ngBootbox, $timeout, orderDetailFactory, initialDataOfOrderDetail) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Order Detail';
        fo.lv.status = 'Shipped';

        function initializeController() {
            fo.vm = initialDataOfOrderDetail.viewModel.ReturnedData;
            console.log('fo.vm at order detail', fo.vm);
            if (fo.vm.ImageBuffer !== null && fo.vm.ImageBuffer !== '' && fo.vm.ImageBuffer !== undefined) {
                fo.lv.uploadImage = 'data:image/png;base64,' + fo.vm.ImageBuffer.toString();
            }

        }
        initializeController();

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
            if ($scope.OrderDetailForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

    }
})();
