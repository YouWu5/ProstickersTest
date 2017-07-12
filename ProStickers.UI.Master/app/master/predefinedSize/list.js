(function () {
    'use strict';

    angular
        .module('app.predefinedSize')
        .controller('PredefinedSizeList', PredefinedSizeList);

    PredefinedSizeList.$inject = ['$location', '$state', 'PredefinedSizeListFactory', '$ngBootbox', '$timeout', 'initialDataOfPredefinedSizeList', 'stackView', '$scope', 'message', 'helper'];

    function PredefinedSizeList($location, $state, PredefinedSizeListFactory, $ngBootbox, $timeout, initialDataOfPredefinedSizeList, stackView, $scope, message, helper) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Pre Defined Price';

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfPredefinedSizeList.viewModel.ReturnedData;
            console.log('fo.vm @ initialize', fo.vm);
        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

        fo.Save = function () {
            if ($scope.PredefinedSizeListForm.$invalid) {
                console.log('$scope.PredefinedSizeListForm', $scope.PredefinedSizeListForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            PredefinedSizeListFactory.submit(fo.vm).then(function (data) {
                if (data.Result === 1) // Success
                {
                    message.showServerSideMessage(data, true);
                    $state.reload();
                }
                helper.setIsSubmitted(false);
            });
        };

        ///////////////// Click Methods Ends Here ////////////////

    }
})();