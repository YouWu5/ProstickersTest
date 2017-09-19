(function () {
    'use strict';

    angular
        .module('app.users')
        .controller('UsersUpdate', UsersUpdate);

    UsersUpdate.$inject = ['$location', '$state', '$scope', 'helper', 'UsersUpdateFactory', 'message', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfUsersUpdate'];

    function UsersUpdate($location, $state, $scope, helper, UsersUpdateFactory, message, stackView, $ngBootbox, $timeout, initialDataOfUsersUpdate) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Edit User';

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfUsersUpdate.viewModel.ReturnedData;
            console.log('fo.vm', fo.vm);
            if (fo.vm.ImageGUID !== null) {
                fo.lv.uploadImage = angular.copy(fo.vm.ImageGUID);
            }
            else {
                fo.lv.uploadImage = ' ';
                fo.vm.ImageBuffer = null;
            }
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
            if ($scope.UserUpdateForm.$dirty) {
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

        fo.Save = function () {
            if ($scope.UserUpdateForm.$invalid) {
                console.log('$scope.UserUpdateForm', $scope.UserUpdateForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            if (fo.lv.uploadImage !== fo.vm.ImageGUID) {
                if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null && angular.equals(' ', fo.lv.uploadImage) !== true) {
                    var image = new RegExp(/^data:image\/(png|jpeg|jpg|gif|bmp);base64,/);
                    if (image.test(fo.lv.uploadImage)) {
                        if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null) {
                            fo.vm.ImageBuffer = fo.lv.uploadImage;
                            fo.vm.ImageBuffer = fo.vm.ImageBuffer.replace(/^data:image\/(png|jpeg|jpg|gif|bmp);base64,/, '');
                            fo.vm.isNew = true;
                        }
                    }
                }
            }
            UsersUpdateFactory.submit(fo.vm).then(function (data) {
                if (data.Result === 1) // Success
                {
                    message.showServerSideMessage(data, true);
                    $scope.UserUpdateForm.$setPristine();
                    stackView.closeThisView();
                }
                helper.setIsSubmitted(false);
            });
        };

        fo.updateActive = function () {
            var options = {
                message: 'Are you sure you want to make the user inactive?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                        }
                    }
                }
            };

            if (!fo.vm.Active) {
                $ngBootbox.customDialog(options);
            }
        };

        ///////////////// Click Methods Ends Here ///////////////////
    }
})();
