(function () {
    'use strict';

    angular
        .module('app.file')
        .controller('FilesList', FilesList);

    FilesList.$inject = ['localStorageService', 'spinnerService', 'helper', 'message', '$scope', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfFilesList', 'filesListFactory'];

    function FilesList(localStorageService, spinnerService, helper, message, $scope, stackView, $ngBootbox, $timeout, initialDataOfFilesList, filesListFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Files';
        fo.lv.IsCalled = false;
        fo.lv.uploadImage = ' ';
        fo.lv.OtherFileName = null;
        fo.lv.filesList = [];
        function initializeController() {
            fo.vm = initialDataOfFilesList.viewModel;
            fo.lv.filesList = initialDataOfFilesList.filesList.filesList;
            fo.vm.FileNote = initialDataOfFilesList.filesList.FileNote;

            console.log('fo.lv.filesList', fo.lv.filesList);

        }
        initializeController();

        fo.uploadfile = function () {
            var userInfo = localStorageService.get('UserSession');
            var count = 0;
            fo.vm.FileName = '';
            var fullPath = document.getElementById('img').value;
            if (fullPath) {
                var startIndex = (fullPath.indexOf('\\') >= 0 ? fullPath.lastIndexOf('\\') : fullPath.lastIndexOf('/'));
                var filename = fullPath.substring(startIndex);
                if (filename.indexOf('\\') === 0 || filename.indexOf('/') === 0) {
                    fo.vm.FileName = filename.substr(0, filename.lastIndexOf('.'));
                    fo.vm.FileName = fo.vm.FileName.replace(/\s/g, '');
                }
            }
             
            if (fo.lv.uploadImage && fo.lv.uploadImage !== '' && fo.lv.uploadImage !== null && fo.lv.uploadImage !== undefined) {
                fo.vm.FileExtension = fo.lv.uploadImage.split('/')[1].split(';')[0];
            }
             
            if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null && angular.equals(' ', fo.lv.uploadImage) !== true) {
                var image = new RegExp(/^data:image\/(png|jpeg|jpg|gif|bmp);base64,/);
                if (image.test(fo.lv.uploadImage)) {
                    if (fo.lv.uploadImage !== undefined && fo.lv.uploadImage !== null) {
                        fo.vm.FileBuffer = fo.lv.uploadImage;
                        fo.vm.FileBuffer = fo.vm.FileBuffer.replace(/^data:image\/(png|jpeg|jpg|gif|bmp);base64,/, '');
                        fo.vm.FileName = fo.vm.FileName.substr(1);
                        console.log('upload submit vm', fo.vm);
                         
                        fo.lv.fullFileName = userInfo.UserID + '-' + fo.vm.FileName + '.' + fo.vm.FileExtension;
                        
                        for (var m = 0; m < fo.lv.filesList.length; m++) {
                            if (fo.lv.fullFileName === fo.lv.filesList[m].FileNumber) {
                                count++;
                            }
                        }

                        if (count > 0) {
                            var options = {
                                message: 'There is already a file with the same name. Do you want to replace it ?',
                                buttons: {
                                    success: {
                                        label: ' ',
                                        className: 'fa fa-check-page',
                                        callback: function () {
                                            $timeout(function () {
                                            }, 100);

                                            filesListFactory.deleteFile(fo.lv.fullFileName).then(function (data) {
                                                console.log('submit response at detail', data);
                                                if (data.Result === 1)          // Success
                                                {
                                                    filesListFactory.uploadFile(fo.vm).then(function (data) {
                                                        fo.lv.message = data;
                                                        filesListFactory.getFilesList().then(function (data) {
                                                            fo.lv.filesList = data.filesList;
                                                            fo.vm.FileNote = data.FileNote;
                                                            fo.lv.uploadImage = ' ';
                                                            message.showServerSideMessage(fo.lv.message, true);
                                                        });
                                                    });
                                                }

                                            });
                                        }
                                    }
                                }
                            };

                            $ngBootbox.customDialog(options);
                             
                        }
                        else {

                            filesListFactory.uploadFile(fo.vm).then(function (data) {
                                fo.lv.message = data;
                                filesListFactory.getFilesList().then(function (data) {
                                    fo.lv.filesList = data.filesList;
                                    fo.vm.FileNote = data.FileNote;
                                    fo.lv.uploadImage = ' ';
                                    message.showServerSideMessage(fo.lv.message, true);
                                });
                            });
                        }
                    }
                }
            }

        };

        fo.uploadExcel = function () {

        };

        fo.downloadFile = function (item) {
            spinnerService.show('pageContainerSpinner');
            console.log('item.FileNumber', item.FileNumber);
            filesListFactory.downloadFile(item.FileNumber).then(function (data) {
                console.log('data', data);
                if (data !== null) {
                    var b = angular.element('#downloadedImg');
                    var a = b[0];
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
        };

        fo.deleteFile = function (fileNo) {
            console.log('fileNo', fileNo);

            var options = {
                message: 'Are you sure you want to delete?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            spinnerService.show('pageContainerSpinner');
                            filesListFactory.deleteFile(fileNo).then(function (data) {
                                console.log('submit response at detail', data);
                                if (data.Result === 1)          // Success
                                {
                                    filesListFactory.getFilesList().then(function (data) {
                                        fo.lv.filesList = data.filesList;
                                    });
                                    message.showServerSideMessage(data, true);
                                }
                                helper.setIsSubmitted(false);
                            });
                        }
                    }
                }
            };

            $ngBootbox.customDialog(options);

        };

        fo.save = function () {
            console.log('fo.vm at submit detail', fo.vm);
            if ($scope.FilesListForm.$invalid) {
                console.log('$scope.FilesListForm', $scope.FilesListForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            else {
                filesListFactory.submit(fo.vm).then(function (data) {
                    console.log('submit response at detail', data);
                    if (data.Result === 1)          // Success
                    {
                        message.showServerSideMessage(data, false);
                        $scope.FilesListForm.$setPristine();
                    }
                    helper.setIsSubmitted(false);
                });
            }
        };
    }
})();
