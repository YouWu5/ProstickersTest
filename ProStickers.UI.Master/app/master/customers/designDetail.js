(function () {
    'use strict';

    angular
        .module('app.customers')
        .controller('DesignDetail', DesignDetail);

    DesignDetail.$inject = ['$location', '$scope', 'helper', 'DesignDetailFactory', 'message', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfCustomersDesignDetail'];

    function DesignDetail($location, $scope, helper, DesignDetailFactory, message, stackView, $ngBootbox, $timeout, initialDataOfCustomersDesignDetail) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Designer\'s Note';

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfCustomersDesignDetail.viewModel;
            console.log('fo.vm', fo.vm);
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
            if ($scope.DesignDetailForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.Save = function () {
            if ($scope.DesignDetailForm.$invalid) {
                console.log('$scope.DesignDetailForm', $scope.DesignDetailForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            else {
                DesignDetailFactory.submit(fo.vm).then(function (data) {
                    if (data.Result === 1) // Success
                    {
                        message.showServerSideMessage(data, true);
                        $scope.DesignDetailForm.$setPristine();
                        stackView.closeView();
                    }
                    helper.setIsSubmitted(false);
                });
            }
        };

        ///////////////// Click Methods Ends Here //////////////////

    }
})();