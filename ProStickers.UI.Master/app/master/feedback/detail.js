(function () {
    'use strict';

    angular
        .module('app.feedback')
        .controller('FeedbackDetail', FeedbackDetail);

    FeedbackDetail.$inject = ['$location', '$scope', '$state', 'helper', 'FeedbackDetailFactory', 'message', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfFeedbackDetail'];

    function FeedbackDetail($location, $scope, $state, helper, FeedbackDetailFactory, message, stackView, $ngBootbox, $timeout, initialDataOfFeedbackDetail) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.sm = {};
        fo.lv.title = 'Feedback Detail';

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfFeedbackDetail.viewModel.ReturnedData;
            console.log('fo.vm @ initialize', fo.vm);
        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

        fo.Cancel = function () {
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
                            if (obj.formName !== 'FeedbackList') {
                                stackView.closeView();
                                return;
                            }
                            else {
                                stackView.closeView();
                            }
                        }
                    }
                }
            };
            if ($scope.FeedbackDetailForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                if (obj.formName !== 'FeedbackList') {
                    stackView.closeView();
                    return;
                }
                else {
                    stackView.closeView();
                }
            }
        };

        fo.Save = function () {
            if ($scope.FeedbackDetailForm.$invalid) {
                console.log('$scope.FeedbackDetailForm', $scope.FeedbackDetailForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            FeedbackDetailFactory.submit(fo.vm).then(function (data) {
                if (data.Result === 1) // Success
                {
                    message.showServerSideMessage(data, true);
                    $scope.FeedbackDetailForm.$setPristine();
                    stackView.closeView();
                }
                helper.setIsSubmitted(false);
            });
        };

        fo.Delete = function (customerId, designNo) {
            var options = {
                message: 'Do you want to delete the feedback?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            FeedbackDetailFactory.deleteFeedback(customerId, designNo).then(function (data) {
                                if (data.Result === 1) // Success
                                {
                                    message.showServerSideMessage(data, true);
                                    $scope.FeedbackDetailForm.$setPristine();
                                    stackView.closeThisView();
                                }
                            });
                        }
                    }
                }
            };
            $ngBootbox.customDialog(options);
        };

        ///////////////// Click Methods Ends Here ///////////////////

        ////////////////// Helper methods starts Here ////////////////

    }
})();
