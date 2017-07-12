(function () {
    'use strict';

    angular
        .module('app.colorChart')
        .controller('ColorChartUpdate', ColorChartUpdate);

    ColorChartUpdate.$inject = ['$location', '$state', '$scope', 'helper', 'ColorChartUpdateFactory', 'message', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfColorChartUpdate'];

    function ColorChartUpdate($location, $state, $scope, helper, ColorChartUpdateFactory, message, stackView, $ngBootbox, $timeout, initialDataOfColorChartUpdate) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here ////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Edit Color';

        //////////// Variable declaration. ends here/////////////

        /////////// Initilize controller starts here ///////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfColorChartUpdate.viewModel;
            console.log('fo.vm @ initialize', fo.vm);
            if (fo.vm.ImageBuffer !== null && fo.vm.ImageBuffer !== ' ') {
                fo.lv.uploadImage = 'data:image/png;base64,' + fo.vm.ImageBuffer.toString();
            }
            else {
                fo.lv.uploadImage = ' ';
                fo.vm.ImageBuffer = null;
            }
        }

        /////////// Initilize controller ends here /////////////

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
            if ($scope.ColorUpdateForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.clearImage = function () {
            $('#img').val(null);
            fo.lv.uploadImage = ' ';
            fo.vm.ImageBuffer = null;
        };

        fo.Delete = function () {
            var options = {
                message: 'Are you sure you want to delete this color?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            ColorChartUpdateFactory.deleteColor(fo.vm).then(function (data) {
                                console.log('data', data);
                                message.showServerSideMessage(data, true);
                                stackView.closeThisView();
                            });
                        }
                    }
                }
            };
            $ngBootbox.customDialog(options);
        };

        fo.Save = function () {
            if ($scope.ColorUpdateForm.$invalid) {
                console.log('$scope.ColorUpdateForm', $scope.ColorUpdateForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            if (fo.lv.uploadImage === ' ') {
                message.showClientSideErrors('Please add a color image.');
                fo.lv.isFormInvalid = true;
                return;
            }
            if (fo.vm.ImageBuffer !== fo.lv.uploadImage) {
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
            }
            ColorChartUpdateFactory.submit(fo.vm).then(function (data) {
                if (data.Result === 1) // Success
                {
                    message.showServerSideMessage(data, true);
                    $scope.ColorUpdateForm.$setPristine();
                    stackView.closeThisView();
                }
                helper.setIsSubmitted(false);
            });
        };

        //////////////////Click Methods Ends Here///////////////

    }
})();
