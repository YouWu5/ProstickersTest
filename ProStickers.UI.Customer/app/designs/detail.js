(function () {
    'use strict';

    angular
        .module('app.design')
        .controller('DesignsDetail', DesignsDetail);

    DesignsDetail.$inject = ['$location', '$state', 'message', '$scope', 'stackView', '$ngBootbox', '$timeout', 'initialDataOfDesignDetail', 'designDetailFactory'];

    function DesignsDetail($location, $state, message, $scope, stackView, $ngBootbox, $timeout, initialDataOfDesignDetail, designDetailFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Design Details';

        function initializeController() {

            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'DesignsDetail') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                stackView.discardViewDetail();
            }
            else {
                fo.vm = initialDataOfDesignDetail.viewModel.ReturnedData;
                if (fo.vm.ImageBuffer !== null && fo.vm.ImageBuffer !== ' ') {
                    fo.lv.uploadImage = 'data:image/png;base64,' + fo.vm.ImageBuffer.toString();
                }
            }
            console.log('design detail vm', fo.vm);
             
        }
        initializeController();

        fo.cancel = function () {
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
            if ($scope.DesignDetailForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.delete = function () {
            var options = {
                message: 'Do you really want to delete design?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            designDetailFactory.deleteDesign(fo.vm.DesignNumber, fo.vm.UpdatedTS).then(function (data) {
                                console.log('deleted data ', data);
                                if (data.Result === 1)          // Success
                                {
                                    message.showServerSideMessage(data, true);
                                    $scope.DesignDetailForm.$setPristine();
                                    stackView.closeThisView();
                                }
                            });
                        }
                    }
                }
            };

            $ngBootbox.customDialog(options);
        };

        fo.buy = function () {
            stackView.pushViewDetail({
                controller: 'DesignsDetail',
                formObject: fo, url: 'DesignsDetail',
                formName: 'DesignsDetail'
            });
            $state.go('OrdersCreate', { DesignNumber: fo.vm.DesignNumber, AppointmentNumber: fo.vm.AppointmentNumber, redirect: true });
        };

        fo.downloadFile = function () {
            designDetailFactory.downloadFile(fo.vm.DesignNumber).then(function (data) {
                console.log('data', data);
                if (data !== null) {
                    fo.lv.image = 'data:image/png;base64,' + data.ImageBuffer.toString();
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
                    a.href = blobUrl; a.download = fo.vm.DesignNumber; a.click();

                }
            });
        };

    }
})();
