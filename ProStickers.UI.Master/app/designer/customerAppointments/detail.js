(function () {
    'use strict';

    angular
        .module('app.appointment')
        .controller('AppointmentDetail', AppointmentDetail);

    AppointmentDetail.$inject = ['$location', '$state', 'stackView', '$compile', '$scope', '$ngBootbox', '$timeout', 'message',
        'helper', 'AppointmentDetailFactory', 'InitialDataOfAppointmentDetail', 'spinnerService'];

    function AppointmentDetail($location, $state, stackView, $compile, $scope, $ngBootbox, $timeout, message,
        helper, AppointmentDetailFactory, InitialDataOfAppointmentDetail, spinnerService) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Appointments Details';
        fo.lv.setFooterPaddingRecord = null;
        fo.lv.uploadImage = ' ';
        fo.lv.uploadVector = ' ';
        fo.lv.uploadOtherImage = ' ';
        fo.lv.IsCancelled = false;
        fo.lv.linkChanged = false;
        fo.lv.IsEnabled = false;
        fo.lv.DesignName = null;
        fo.lv.VectorName = null;
        fo.lv.ext1 = null;
        fo.lv.ext2 = null;
        fo.lv.ext3 = null;

        function initilizeController() {
            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'AppointmentDetail') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                $location.$$absUrl = obj.templateUrl;
                stackView.discardViewDetail();
            }
            else {

                fo.vm = InitialDataOfAppointmentDetail.viewModel;
                var a = helper.ConvertDateCST(new Date());
                fo.lv.startTime = a.getFullYear().toString() + '-' +
                    ((a.getMonth() + 1).toString().length > 1 ? (a.getMonth() + 1).toString() : ('0' + (a.getMonth() + 1).toString())) +
                    '-' + ((a.getDate().toString()).length === 1 ? ('0' + a.getDate().toString()) : a.getDate().toString()) + 'T' +
                    (((Math.floor(fo.vm.StartTime / 60)).toString()).length === 1 ? ('0' + (Math.floor(fo.vm.StartTime / 60)).toString()) : (Math.floor(fo.vm.StartTime / 60)).toString()) +
                    ':' + (((fo.vm.StartTime % 60).toString()).length === 1 ? ('0' + (fo.vm.StartTime % 60).toString()) : (fo.vm.StartTime % 60).toString());

                var startTime = new Date(new Date(fo.vm.AppointmentDateTime).getFullYear(), new Date(fo.vm.AppointmentDateTime).getMonth(), new Date(fo.vm.AppointmentDateTime).getDate(),
                    Math.floor(fo.vm.StartTime / 60), ((fo.vm.StartTime % 60) - 10), new Date().getSeconds());
                var endTime = new Date(new Date(fo.vm.AppointmentDateTime).getFullYear(), new Date(fo.vm.AppointmentDateTime).getMonth(), new Date(fo.vm.AppointmentDateTime).getDate(),
                    Math.floor(fo.vm.EndTime / 60), fo.vm.EndTime % 60, new Date().getSeconds());

                if (fo.vm.EndTime === 0) {
                    endTime = new Date(endTime.setDate(endTime.getDate() + 1));
                }

                fo.lv.ImageStatusID = angular.copy(fo.vm.ImageStatusID);

                fo.lv.StartTime = angular.copy(helper.formatDateTime(fo.lv.startTime));

                if (startTime.getTime() > helper.ConvertDateCST(new Date()).getTime()) {
                    fo.lv.IsEnabled = true;
                }

                else if (fo.vm.AppointmentStatus === 'Scheduled' && endTime.getTime() < helper.ConvertDateCST(new Date()).getTime()) {
                    fo.lv.IsCancelled = true;
                }

                //if (fo.vm.AppointmentStatus === 'Initiated' && helper.ConvertDateCST(new Date()).getTime() > new Date(fo.vm.AppointmentDateTime).getTime() + 1500000) {
                //    fo.lv.IsCancelled = true;
                //}

                fo.lv.AppointmentStatus = angular.copy(fo.vm.AppointmentStatus);

                fo.lv.DesignerAppointmentFileList = angular.copy(fo.vm.DesignerAppointmentFileList);
                fo.lv.DesignImageList = angular.copy(fo.vm.DesignImageList);
                fo.lv.FileList = angular.copy(fo.vm.FileList);

                fo.vm.DesignerAppointmentFileList = null;
                fo.vm.DesignImageList = null;
                fo.vm.FileList = null;
                console.log('InitialDataOfAppointmentDetail', fo);
            }
        }

        initilizeController();

        fo.clear = function (value) {
            if (value === 'design') {
                fo.lv.isImage = 0;
                fo.lv.uploadImage = ' ';
            }
            if (value === 'vector') {
                fo.lv.isImage = 1;
                fo.lv.uploadVector = ' ';
            }
            if (value === 'other') {
                fo.lv.isImage = 2;
                fo.lv.uploadOtherImage = ' ';
            }
        };

        fo.uploadOtherFile = function () {
            if (fo.lv.uploadOtherImage !== undefined && fo.lv.uploadOtherImage !== null && angular.equals(' ', fo.lv.uploadOtherImage) !== true) {
                fo.lv.imageModel = {};
                fo.lv.imageModel.FileBuffer = null;
                fo.lv.imageModel.AppointmentNumber = fo.vm.AppointmentNumber;
                fo.lv.imageModel.FileExtension = angular.copy(fo.lv.ext3.toLowerCase());
                fo.lv.imageModel.CustomerID = fo.vm.CustomerID;
                fo.lv.imageModel.FileName = fo.lv.OtherFileName.replace('.' + (fo.lv.ext3).toString(), '');
                if (fo.lv.uploadOtherImage !== undefined && fo.lv.uploadOtherImage !== null) {
                    fo.lv.imageModel.FileBuffer = fo.lv.uploadOtherImage;
                    fo.lv.imageModel.FileBuffer = fo.lv.imageModel.FileBuffer.split(',')[1];
                    AppointmentDetailFactory.uploadImage(fo.lv.imageModel).then(function (data) {
                        if (data.Result === 1) // Success
                        {
                            fo.lv.DesignerAppointmentFileList = data.ReturnedData;
                            fo.lv.uploadOtherImage = ' ';
                        }
                    });
                }
            }
        };

        fo.InitiateCall = function () {
            fo.lv.StartTime = angular.copy(helper.formatDateTime(fo.lv.startTime));
            fo.lv.maxStart = helper.ConvertDateCST(new Date()).setHours(fo.lv.StartTime.split('T')[1].split(':')[0]);
            fo.lv.maxStart = new Date(fo.lv.maxStart).setMinutes(Math.floor(fo.lv.StartTime.split('T')[1].split(':')[1]) + 15);
            fo.lv.maxStart = new Date(fo.lv.maxStart).setSeconds(0);
            fo.lv.minStart = helper.ConvertDateCST(new Date()).setHours(fo.lv.StartTime.split('T')[1].split(':')[0]);
            fo.lv.minStart = new Date(fo.lv.minStart).setMinutes(Math.floor(fo.lv.StartTime.split('T')[1].split(':')[1]) - 10);
            fo.lv.minStart = new Date(fo.lv.minStart).setSeconds(0);

            var templateString = '<div><div><label class="glyphicon-asterisk">Call start time</label>' +
                '<div uib-timepicker ng-model="fo.lv.StartTime" max="fo.lv.maxStart" min="fo.lv.minStart" show-spinners=false hour-step="1"' +
                'minute-step="1" show-meridian="false">' +
                '</div></div></div>';
            var element = $compile(templateString)($scope);

            var options = {
                title: 'Initiate call',
                value: fo.lv.selectValue,
                message: element,
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            fo.vm.CallStartTime = angular.copy(fo.lv.StartTime);
                            fo.vm.AppointmentStatus = 'Initiated';
                            fo.vm.CallStartTime = helper.formatDateTime(fo.vm.CallStartTime);
                            console.log('data on updateStatus', angular.toJson(fo.vm));
                            AppointmentDetailFactory.updateStatus(fo.vm).then(function (data) {
                                console.log('data on updateStatus', data);
                                fo.vm.UpdatedTS = data.ReturnedData;
                                message.showServerSideMessage(data, false);
                                fo.lv.AppointmentStatus = 'Initiated';
                            });
                        }
                    }
                },
                onEscape: function () {
                    fo.lv.StartTime = helper.formatDateTime(fo.lv.startTime);
                }
            };
            $ngBootbox.customDialog(options);
        };

        fo.CompleteCall = function () {
            fo.lv.maxEnd = helper.ConvertDateCST(new Date()).setHours(fo.vm.CallStartTime.split('T')[1].split(':')[0]);
            fo.lv.maxEnd = new Date(fo.lv.maxEnd).setMinutes(Math.floor(fo.vm.CallStartTime.split('T')[1].split(':')[1]) + 25);
            fo.lv.maxEnd = new Date(fo.lv.maxEnd).setSeconds(0);
            fo.lv.minEnd = helper.ConvertDateCST(new Date()).setHours(fo.vm.CallStartTime.split('T')[1].split(':')[0]);
            fo.lv.minEnd = new Date(fo.lv.minEnd).setMinutes(Math.floor(fo.vm.CallStartTime.split('T')[1].split(':')[1]));
            fo.lv.minEnd = new Date(fo.lv.minEnd).setSeconds(0);
            fo.lv.EndTime = helper.formatDateTime(fo.vm.CallStartTime);

            var templateString = '<div><div><label class="glyphicon-asterisk">Call end time</label>' +
                '<div uib-timepicker ng-model="fo.lv.EndTime" max="fo.lv.maxEnd" min="fo.lv.minEnd" show-spinners=false hour-step="1"' +
                'minute-step="1" show-meridian="false">' +
                '</div></div></div>';
            var element = $compile(templateString)($scope);

            var options = {
                title: 'Complete call',
                value: fo.lv.selectValue,
                message: element,
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            fo.vm.CallEndTime = fo.lv.EndTime;
                            fo.vm.AppointmentStatus = 'Completed';
                            fo.vm.CallEndTime = helper.formatDateTime(fo.vm.CallEndTime);
                            console.log('data on updateStatus', angular.toJson(fo.vm));
                            AppointmentDetailFactory.updateStatus(fo.vm).then(function (data) {
                                console.log('data on updateStatus', data);
                                fo.vm.UpdatedTS = data.ReturnedData;
                                message.showServerSideMessage(data, false);
                                fo.lv.AppointmentStatus = 'Completed';
                            });
                        }
                    }
                },
                onEscape: function () {
                }
            };
            $ngBootbox.customDialog(options);
        };

        fo.CancelCall = function () {
            fo.vm.IsCancel = true;
            fo.vm.AppointmentStatus = 'Cancelled';
        };

        fo.download = function (item, type) {
            spinnerService.show('pageContainerSpinner');
            if (type === 'otherFile') {
                AppointmentDetailFactory.downloadOtherFile(item.FileNumber).then(function (data) {
                    if (data !== null) {
                        var b = angular.element('#downloadedImg'); var a = b[0];
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

            if (type === 'DesignImage') {
                AppointmentDetailFactory.downloadDesignImage(item.FileNumber).then(function (data) {
                    if (data !== null) {
                        var b = angular.element('#downloadedDesign'); var a = b[0];
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

            if (type === 'VectorFile') {
                AppointmentDetailFactory.downloadVectorFile(item.FileNumber).then(function (data) {
                    if (data !== null) {
                        var b = angular.element('#downloadedVector'); var a = b[0];
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

            if (type === 'File') {
                AppointmentDetailFactory.downloadFile(item.FileNumber).then(function (data) {
                    if (data !== null) {
                        var b = angular.element('#downloadedFile'); var a = b[0];
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

        fo.openDetail = function (item) {
            stackView.pushViewDetail({
                controller: 'AppointmentDetail',
                formObject: fo, url: 'AppointmentDetail',
                templateUrl: $location.$$absUrl,
                formName: 'AppointmentDetail'
            });
            $state.go('DesignDetails', { Number: item.FileNumber, ID: item.UserID });
        };

        fo.save = function () {
            if ($scope.AppointmentDetailForm.$invalid) {
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }

            if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null && angular.equals(' ', fo.lv.uploadImage) !== true) {
                if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null) {
                    fo.vm.DesignImageExtension = fo.lv.ext1;
                    fo.vm.DesignImageBuffer = fo.lv.uploadImage;
                    fo.vm.ImageStatusID = 1;
                    fo.vm.DesignImageBuffer = fo.vm.DesignImageBuffer.split(',')[1];
                }
            }

            if (fo.lv.uploadVector !== undefined && fo.lv.uploadVector !== null && angular.equals(' ', fo.lv.uploadVector) !== true) {
                if (fo.lv.uploadVector !== undefined && fo.lv.uploadVector !== null) {
                    fo.vm.ImageStatusID = 2;
                    fo.vm.VectorImageExtension = fo.lv.ext2;
                    fo.vm.VectorImageBuffer = fo.lv.uploadVector;
                    fo.vm.VectorImageBuffer = fo.vm.VectorImageBuffer.split(',')[1];
                }
            }

            if (fo.lv.ImageStatusID !== 0) {
                if (fo.vm.DesignImageBuffer !== null || fo.vm.VectorImageBuffer !== null) {
                    fo.vm.ImageStatusID = 3;
                }
            }

            if (fo.vm.DesignImageBuffer !== null && fo.vm.VectorImageBuffer !== null) {
                fo.vm.ImageStatusID = 3;
            }

            if (fo.vm.DesignImageBuffer !== null || fo.vm.VectorImageBuffer !== null) {
                if (parseFloat(fo.vm.AspectRatio) < 0.001) {
                    message.showClientSideErrors('Aspect Ratio should be greater than 0.001.');
                    return;
                }
                if (parseFloat(fo.vm.AspectRatio) > 99.999) {
                    message.showClientSideErrors('Aspect Ratio should not be greater than 99.999.');
                    return;
                }
                if (parseFloat(fo.vm.AspectRatio) < 0) {
                    message.showClientSideErrors('Aspect Ratio should not be negative.');
                    return;
                }
            }

            if (fo.vm.IsCancel === true) {
                AppointmentDetailFactory.updateStatus(fo.vm).then(function (data) {
                    console.log('data on updateStatus', data);
                    message.showServerSideMessage(data, true);
                    $state.go('Appointments');
                });
            }

            else {
                console.log('viewmodel on save', angular.toJson(fo.vm));
                AppointmentDetailFactory.submit(fo.vm).then(function (data) {
                    console.log('data on save', data);
                    message.showServerSideMessage(data, true);
                    if (fo.lv.linkChanged === false) {
                        $state.go('Appointments');
                    } else {
                        $state.reload();
                    }

                });
            }
        };

        fo.Close = function () {
            var obj = stackView.getLastViewDetail();
            console.log('obj', obj);
            var options = {
                message: 'Do you want to close the form?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            if (obj.formName !== 'Home') {
                                stackView.closeView();
                                return;
                            }
                            else {
                                stackView.openView('Appointments');
                            }
                        }
                    }
                }
            };

            if ($scope.AppointmentDetailForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                if (obj.formName !== 'Home') {
                    stackView.closeView();
                    return;
                }
                else {
                    stackView.openView('Appointments');
                }
            }
        };

    }
})();
