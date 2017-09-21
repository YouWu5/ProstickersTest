(function () {
    'use strict';

    angular
        .module('app.coupons')
        .controller('CouponsCreate', CouponsCreate);

    CouponsCreate.$inject = ['$location', '$scope', 'helper', 'CouponsCreateFactory', '$state', 'message', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfCouponsCreate'];

    function CouponsCreate($location, $scope, helper, CouponsCreateFactory, $state, message, stackView, $ngBootbox, $timeout, initialDataOfCouponsCreate) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Add Coupon';
        fo.lv.showSkypeId = false;
        fo.lv.typelist = [];

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfCouponsCreate.viewModel;
            fo.lv.typelist = initialDataOfCouponsCreate.typeList;
            console.log('fo.vm @ initialize', fo.vm);
            fo.vm.CouponTypeID = 1;
            // fo.lv.uploadImage = ' ';
        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

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
            if ($scope.CouponCreateForm.$dirty || fo.lv.uploadImage !== ' ') {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.Save = function () {
            if ($scope.CouponCreateForm.$invalid) {
                console.log('$scope.CouponCreateForm', $scope.CouponCreateForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            angular.forEach(fo.lv.typelist, function (value) {
                if (value.Value === fo.vm.CouponTypeID) {
                    fo.vm.CouponType = value.Text;
                }
            });
            CouponsCreateFactory.submit(fo.vm).then(function (data) {
                if (data.Result === 1) // Success
                {
                    message.showServerSideMessage(data, true);
                    $scope.CouponCreateForm.$setPristine();
                    stackView.closeThisView();
                }
                helper.setIsSubmitted(false);
            });
        };

        ///////////////// Click Methods Ends Here ///////////////////

    }
})();
