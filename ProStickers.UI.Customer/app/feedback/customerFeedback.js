(function () {
    'use strict';

    angular
        .module('app.feedback')
        .controller('CustomerFeedback', CustomerFeedback);

    CustomerFeedback.$inject = ['helper', 'message', '$scope', '$location', '$state', '$timeout', '$ngBootbox', 'feedbackFactory', 'initialDataOfFeedback'];

    function CustomerFeedback(helper, message, $scope, $location, $state, $timeout, $ngBootbox, feedbackFactory, initialDataOfFeedback) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = null;
        fo.lv = {};
        fo.lv.title = 'Feedback';
        fo.lv.isAlreadySubmit = false;

        function initializeController() {
            fo.vm = initialDataOfFeedback.viewModel.ReturnedData;
            console.log('fo.vm at feedback', fo.vm);
            if (fo.vm !== null && fo.vm !== undefined && fo.vm !== '') {
                if (fo.vm.ImageBuffer !== null && fo.vm.ImageBuffer !== ' ') {
                    fo.lv.uploadImage = 'data:image/png;base64,' + fo.vm.ImageBuffer.toString();
                }
            }
            else {
                fo.lv.isAlreadySubmit = true;
                message.showClientSideErrors(initialDataOfFeedback.viewModel.Message);
                $state.go('CustomerHome', { redirect: true });
            }
           
        }
        initializeController();

        fo.submit = function () {
            console.log('fo.vm at submit', fo.vm);
            if ($scope.CustomerFeedbackForm.$invalid) {
                console.log('$scope.CustomerFeedbackForm', $scope.CustomerFeedbackForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            else {
                fo.vm.FeedbackDate = helper.formatDate(new Date());
                feedbackFactory.submit(fo.vm).then(function (data) {
                    console.log('submit response at detail', data);
                    if (data.Result === 1)          // Success
                    {
                        message.showServerSideMessage(data, true);
                        $scope.CustomerFeedbackForm.$setPristine();
                    }
                    helper.setIsSubmitted(false);
                });
            }
        };

    }
})();
