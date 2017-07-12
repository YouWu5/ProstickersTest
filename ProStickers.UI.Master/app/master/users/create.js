(function () {
    'use strict';

    angular
        .module('app.users')
        .controller('UsersCreate', UsersCreate);

    UsersCreate.$inject = ['$location', '$scope', 'helper', 'UsersCreateFactory', '$state', 'message', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfUsersCreate'];

    function UsersCreate($location, $scope, helper, UsersCreateFactory, $state, message, stackView, $ngBootbox, $timeout, initialDataOfUsersCreate) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Add User';
        fo.lv.showSkypeId = false;
        fo.lv.rolelist = [];

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfUsersCreate.viewModel;
            fo.lv.rolelist = initialDataOfUsersCreate.roleList;
            console.log('fo.vm @ initialize', fo.vm);
            fo.vm.UserTypeID = 1;
            fo.lv.uploadImage = ' ';
        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

        fo.userRole = function (selectedUserTypeID) {
            if (selectedUserTypeID === 2) { // Designer
                fo.lv.showSkypeId = true;
            }
            else {
                fo.lv.showSkypeId = false;
            }
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
            if ($scope.UserCreateForm.$dirty || fo.lv.uploadImage !== ' ') {
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
            if ($scope.UserCreateForm.$invalid) {
                console.log('$scope.UserCreateForm', $scope.UserCreateForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            angular.forEach(fo.lv.rolelist, function (value) {
                if (value.Value === fo.vm.UserTypeID) {
                    fo.vm.UserType = value.Text;
                }
            });
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
            UsersCreateFactory.submit(fo.vm).then(function (data) {
                if (data.Result === 1) // Success
                {
                    message.showServerSideMessage(data, true);
                    $scope.UserCreateForm.$setPristine();
                    stackView.closeThisView();
                }
                helper.setIsSubmitted(false);
            });
        };

        ///////////////// Click Methods Ends Here ///////////////////

    }
})();
