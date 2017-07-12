(function () {
    'use strict';

    angular
        .module('app.customers')
        .controller('CustomersDetail', CustomersDetail);

    CustomersDetail.$inject = ['$location', '$scope', '$state', 'helper', 'CustomersDetailFactory', 'message', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfCustomersDetail', 'spinnerService'];

    function CustomersDetail($location, $scope, $state, helper, CustomersDetailFactory, message, stackView, $ngBootbox, $timeout, initialDataOfCustomersDetail, spinnerService) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Customer Detail';
        fo.lv.isStateNameDropDown = false;
        fo.lv.isStateNameRequired = false;
        fo.lv.isRequired = false;
        fo.lv.patternAvailable = false;
        fo.lv.stateList = [];
        fo.lv.ext = '';
        fo.lv.uploadFileName = '';
        fo.lv.fileName = '';
        fo.lv.minLength = 0;
        fo.lv.maxLength = 0;

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'CustomersDetail') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                $location.$$absUrl = obj.templateUrl;
                stackView.discardViewDetail();
            }
            else {
                fo.vm = initialDataOfCustomersDetail.viewModel;
                fo.vm.RemoveUserFileList = [];
                fo.vm.RemoveCustomerFileList = [];
                fo.vm.RemoveCustomerDesignList = [];
                fo.lv.uploadImage = ' ';
                fo.lv.countryList = [{ Name: 'United States', ID: 184 }];
                validatePostalCode(fo.vm.CountryID);
                if (fo.vm.CountryID === 184) {
                    fo.lv.isStateNameDropDown = true;
                    CustomersDetailFactory.getStateList(fo.vm.CountryID).then(function (data) {
                        fo.lv.stateList = data;
                        fo.lv.StateName = fo.vm.StateName !== null && fo.vm.StateName !== undefined && fo.vm.StateName !== '' ? fo.vm.StateName : null;
                    });
                }
                else {
                    fo.lv.isStateNameDropDown = false;
                    fo.lv.StateName = fo.vm.StateName;
                }
            }
            console.log('fo.vm', fo.vm);
        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

        fo.getStateList = function (countryID) {
            fo.vm.PostalCode = null;
            validatePostalCode(countryID);
            if (countryID === null || countryID === undefined || countryID === '') {
                fo.lv.isRequired = true;
                fo.lv.stateList = [];
                fo.vm.CountryID = 0;
                fo.lv.StateName = null;
                fo.vm.StateID = 0;
                fo.vm.PostalCode = null;
            }
            else {
                fo.lv.isRequired = false;
            }
            if (countryID === 184) {
                fo.lv.isStateNameDropDown = true;
                CustomersDetailFactory.getStateList(countryID).then(function (data) {
                    fo.lv.stateList = data;
                });
            }
            else {
                fo.lv.isStateNameDropDown = false;
                fo.lv.StateName = null;
                fo.vm.StateID = 0;
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
            if ($scope.CustomerDetailForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.Save = function () {
            validateAddress();
            if (fo.lv.isAddressFormInvalid === true && fo.vm.CountryID === 0) {
                message.showClientSideErrors('Please select country', true);
                fo.lv.isFormInvalid = true;
                return;
            }

            if ($scope.CustomerDetailForm.$invalid) {
                console.log('$scope.CustomerDetailForm', $scope.CustomerDetailForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            if (fo.vm.CountryID > 0 && fo.vm.StateID === 0 && (fo.lv.StateName === null || fo.lv.StateName === undefined || fo.lv.StateName === '')) {
                message.showClientSideErrors('Please select/enter state', true);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            if (fo.lv.StateName === null || fo.lv.StateName === undefined || fo.lv.StateName === '') {
                fo.vm.StateName = fo.lv.StateName;
            }
            if (fo.lv.allFeildVacant || fo.lv.allFeildFilled) {
                CustomersDetailFactory.submit(fo.vm).then(function (data) {
                    if (data.Result === 1) // Success
                    {
                        message.showServerSideMessage(data, true);
                        $scope.CustomerDetailForm.$setPristine();
                        stackView.closeThisView();
                    }
                    helper.setIsSubmitted(false);
                });
            }
        };

        fo.selectedState = function () {
            angular.forEach(fo.lv.stateList, function (value) {
                if (fo.vm.StateID === value.Value) {
                    fo.lv.StateName = value.Text;
                    fo.vm.StateName = fo.lv.StateName;
                }
            });
        };

        fo.delete = function () {
            var options = {
                message: 'Do you want to delete the customer detail?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            CustomersDetailFactory.deleteCustomer(fo.vm.CustomerID, fo.vm.UpdatedTS).then(function (data) {
                                if (data.Result === 1) // Success
                                {
                                    message.showServerSideMessage(data, true);
                                    $scope.CustomerDetailForm.$setPristine();
                                    stackView.closeThisView();
                                }
                            });
                        }
                    }
                }
            };
            $ngBootbox.customDialog(options);
        };

        fo.deleteFile = function (type, number, index, fromList, toList) {
            if (type === 'customerFile') {
                fo.lv.message = 'Do you want to delete customer uploaded file ? ';
            }
            else if (type === 'designFile') {
                fo.lv.message = 'Do you want to delete the design ' + number + '?';
            }
            else if (type === 'userFile') {
                fo.lv.message = 'Do you want to delete this file ?';
            }
            var options = {
                message: fo.lv.message,
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            toList.push(fromList[index]);
                            fromList.splice(index, 1);
                        }
                    }
                }
            };
            $ngBootbox.customDialog(options);
        };

        fo.downloadFile = function (item, type) {
            spinnerService.show('pageContainerSpinner');
            if (type === 'customerFile') {
                CustomersDetailFactory.downloadCustomerFile(item.FileNumber).then(function (data) {
                    if (data !== null) {
                        var b = angular.element('#downloadedImg1'); var a = b[0];
                        var byteCharacters = atob(data.ImageBuffer.toString());
                        var byteNumbers = new Array(byteCharacters.length);
                        for (var i = 0; i < byteCharacters.length; i++) {
                            byteNumbers[i] = byteCharacters.charCodeAt(i);
                        }
                        var byteArray = new Uint8Array(byteNumbers);
                        var blob = new Blob([byteArray], { type: data.FileExtension });
                        var blobUrl = URL.createObjectURL(blob);
                        a.href = blobUrl; a.download = item.FileNumber; a.click();
                    }
                });
            }
            else if (type === 'vectorFile') {
                CustomersDetailFactory.downloadVectorFile(item.DesignNumber).then(function (data) {
                    if (data !== null) {
                        var b = angular.element('#downloadedImg2'); var a = b[0];
                        var byteCharacters = atob(data.ImageBuffer.toString());
                        var byteNumbers = new Array(byteCharacters.length);
                        for (var i = 0; i < byteCharacters.length; i++) {
                            byteNumbers[i] = byteCharacters.charCodeAt(i);
                        }
                        var byteArray = new Uint8Array(byteNumbers);
                        var blob = new Blob([byteArray], { type: data.FileExtension });
                        var blobUrl = URL.createObjectURL(blob);
                        a.href = blobUrl; a.download = item.DesignNumber; a.click();
                    }
                });
            }
            else if (type === 'designImage') {
                CustomersDetailFactory.downloadDesignImageFile(item.DesignNumber).then(function (data) {
                    if (data.ReturnedData !== null) {
                        var b = angular.element('#downloadedImg3'); var a = b[0];
                        var byteCharacters = atob(data.ReturnedData.ImageBuffer.toString());
                        var byteNumbers = new Array(byteCharacters.length);
                        for (var i = 0; i < byteCharacters.length; i++) {
                            byteNumbers[i] = byteCharacters.charCodeAt(i);
                        }
                        var byteArray = new Uint8Array(byteNumbers);
                        var blob = new Blob([byteArray], { type: data.ReturnedData.FileExtension });
                        var blobUrl = URL.createObjectURL(blob);
                        a.href = blobUrl; a.download = item.DesignNumber; a.click();
                    }
                    else {
                        message.showClientSideErrors(data.Message);
                    }
                });
            }
            else if (type === 'userFile') {
                CustomersDetailFactory.downloadUserFile(item.FileNumber).then(function (data) {
                    if (data !== null) {
                        var b = angular.element('#downloadedImg4'); var a = b[0];
                        var byteCharacters = atob(data.ImageBuffer.toString());
                        var byteNumbers = new Array(byteCharacters.length);
                        for (var i = 0; i < byteCharacters.length; i++) {
                            byteNumbers[i] = byteCharacters.charCodeAt(i);
                        }
                        var byteArray = new Uint8Array(byteNumbers);
                        var blob = new Blob([byteArray], { type: data.FileExtension });
                        var blobUrl = URL.createObjectURL(blob);
                        a.href = blobUrl; a.download = item.FileNumber; a.click();
                    }
                });
            }
        };

        fo.uploadOtherFile = function () {
            if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null && angular.equals(' ', fo.lv.uploadImage) !== true) {
                fo.lv.uploadFileModel = {};
                fo.lv.uploadFileModel.FileBuffer = null;
                fo.lv.uploadFileModel.UserID = fo.vm.UserID;
                fo.lv.uploadFileModel.CustomerID = fo.vm.CustomerID;
                fo.lv.uploadFileModel.FileExtension = fo.lv.ext;
                fo.lv.uploadFileModel.FileName = fo.lv.uploadFileName;
                var image = new RegExp(/^data:image\/(png|jpeg|jpg|gif|bmp);base64,/);
                if (image.test(fo.lv.uploadImage)) {
                    if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null) {
                        fo.lv.uploadFileModel.FileBuffer = fo.lv.uploadImage;
                        fo.lv.uploadFileModel.FileBuffer = fo.lv.uploadFileModel.FileBuffer.replace(/^data:image\/(png|jpeg|jpg|gif|bmp);base64,/, '');
                        console.log('fo.lv.uploadFileModel', fo.lv.uploadFileModel);
                        console.log('fo.vm', fo.vm);
                        CustomersDetailFactory.uploadFile(fo.lv.uploadFileModel).then(function (data) {
                            if (data.Result === 1) // Success
                            {
                                message.showServerSideMessage(data, true);
                                fo.lv.tempList = angular.copy(data.ReturnedData);
                                fo.lv.uploadImage = ' ';
                                if (fo.vm.RemoveUserFileList !== null && fo.vm.RemoveUserFileList !== undefined && fo.vm.RemoveUserFileList !== '' && fo.vm.RemoveUserFileList.length > 0) {
                                    angular.forEach(fo.vm.RemoveUserFileList, function (removedValue) {
                                        angular.forEach(fo.lv.tempList, function (value, key) {
                                            if (removedValue.FileNumber === value.FileNumber) {
                                                fo.lv.tempList.splice(key, 1);
                                            }
                                        });
                                    });
                                    fo.vm.UserFileList = fo.lv.tempList;
                                }
                                else {
                                    fo.vm.UserFileList = fo.lv.tempList;
                                }
                            }
                        });
                    }
                }
            }
        };

        fo.OpenDesignDetail = function (designNumber, userID) {
            stackView.pushViewDetail({
                controller: 'CustomersDetail',
                formObject: fo, url: 'CustomersDetail',
                formName: 'CustomersDetail',
                templateUrl: $location.$$absUrl
            });
            $state.go('DesignDetail', { DesignID: designNumber, UserID: userID, redirect: true });
        };

        ///////////////// Click Methods Ends Here //////////////////

        ////////////////// Helper methods starts Here //////////////

        function validateAddress() {
            fo.lv.allFeildVacant = false;
            fo.lv.allFeildFilled = false;
            if (fo.vm.CountryID > 0 && fo.vm.CountryID !== 231 && fo.vm.CountryID !== 38) {
                fo.vm.StateName = fo.lv.StateName;
            }
            if (fo.vm.CountryID) {
                angular.forEach(fo.lv.countryList, function (value) {
                    if (fo.vm.CountryID === value.Value) {
                        fo.vm.CountryName = value.Text;
                    }
                });
            }
            else {
                fo.vm.CountryName = '';
            }
            if ((fo.vm.Address1 === null || fo.vm.Address1 === undefined || fo.vm.Address1 === '') &&
                (fo.vm.City === null || fo.vm.City === undefined || fo.vm.City === '') &&
                (fo.lv.StateName === null || fo.lv.StateName === '' || fo.lv.StateName === undefined) &&
                (fo.vm.PostalCode === null || fo.vm.PostalCode === undefined || fo.vm.PostalCode === '') &&
                (fo.vm.CountryID === 0 || fo.vm.CountryID === '0' || fo.vm.CountryID === undefined)) {
                fo.lv.isRequired = false;
                fo.lv.isAddressFormInvalid = false;
                fo.lv.allFeildVacant = true;
                fo.lv.showAddressDetails = false;
            }
            if ((fo.vm.Address1 !== null && fo.vm.Address1 !== undefined && fo.vm.Address1 !== '') &&
                (fo.vm.City !== null && fo.vm.City !== undefined && fo.vm.City !== '') &&
                (fo.lv.StateName !== null && fo.lv.StateName !== '' && fo.lv.StateName !== undefined) &&
                (fo.vm.PostalCode !== null && fo.vm.PostalCode !== undefined && fo.vm.PostalCode !== '') &&
                (fo.vm.CountryID !== 0 && fo.vm.CountryID !== '0' && fo.vm.CountryID !== undefined)) {
                fo.lv.allFeildFilled = true;
                fo.lv.isRequired = false;
                fo.lv.isAddressFormInvalid = false;
                fo.lv.showAddressDetails = false;
            }
            else {
                if (!fo.lv.allFeildVacant) {
                    fo.lv.isRequired = true;
                    fo.lv.isAddressFormInvalid = true;
                    fo.lv.showAddressDetails = true;
                }
            }
        }

        function validatePostalCode(countryID) {
            if (countryID === 33) {
                fo.lv.minLength = 7;
                fo.lv.maxLength = 7;
                fo.lv.postalCodePattern = '^([a-zA-Z0-9]{5}|[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9] [a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9])';
            }
            else if (countryID === 184) {
                fo.lv.minLength = 5;
                fo.lv.maxLength = 6;
                fo.lv.postalCodePattern = '^[0-9]*$';
            }
            else {
                fo.lv.minLength = 1;
                fo.lv.maxLength = 20;
                fo.lv.postalCodePattern = '';
            }
        }

        ////////////////// Helper methods ends Here ////////////////

    }
})();