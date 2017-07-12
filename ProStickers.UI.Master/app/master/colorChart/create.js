(function () {
    'use strict';

    angular
        .module('app.colorChart')
        .controller('ColorChartCreate', ColorChartCreate);

    ColorChartCreate.$inject = ['$location', '$state', '$scope', 'helper', 'ColorChartCreateFactory', 'message', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfColorChartCreate'];

    function ColorChartCreate($location, $state, $scope, helper, ColorChartCreateFactory, message, stackView, $ngBootbox, $timeout, initialDataOfColorChartCreate) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Add Color';

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfColorChartCreate.viewModel;
            console.log('fo.vm @initialize', fo.vm);
            fo.lv.uploadImage = ' ';
            fo.vm.IsAllowForSale = true;
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
                            stackView.closeThisView();
                        }
                    }
                }
            };
            if ($scope.ColorCreateForm.$dirty || fo.lv.uploadImage !== ' ') {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeThisView();
            }
        };

        fo.clearImage = function () {
            $('#img').val(null);
            fo.lv.uploadImage = ' ';
            fo.vm.ImageBuffer = null;
        };

        fo.Save = function () {
            if ($scope.ColorCreateForm.$invalid) {
                console.log('$scope.ColorCreateForm', $scope.ColorCreateForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            if (fo.lv.uploadImage === ' ') {
                message.showClientSideErrors('Please add a color image.');
                fo.lv.isFormInvalid = true;
                return;
            }
            if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null && angular.equals(' ', fo.lv.uploadImage) !== true) {
                var image = new RegExp(/^data:image\/(png|jpeg|jpg|gif|bmp);base64,/);
                if (image.test(fo.lv.uploadImage)) {
                    if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null) {
                        fo.vm.ImageBuffer = fo.lv.uploadImage;
                        fo.vm.ImageBuffer = fo.vm.ImageBuffer.replace(/^data:image\/(png|jpeg|jpg|gif|bmp);base64,/, '');
                        fo.vm.IsNew = true;
                    }
                }
            }
            ColorChartCreateFactory.submit(fo.vm).then(function (data) {
                if (data.Result === 1) // Success
                {
                    message.showServerSideMessage(data, true);
                    $state.reload();
                }
                helper.setIsSubmitted(false);
            });
        };

        ///////////////// Click Methods Ends Here ///////////////////

    }
})();
