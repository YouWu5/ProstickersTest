/*global $ */
(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('fileRead', fileRead);

    fileRead.$inject = ['message', '$timeout'];

    function fileRead(message, $timeout) {
        var directive = {
            link: link,
            restrict: 'EA',
            scope: {
                fileRead: '=',
            },
        };
        return directive;

        function link(scope, element) {
            element.bind('change', function (changeEvent) {
                scope.fileInput = $('.filestyle');
                scope.maxSize = 20971520;
                var ext = changeEvent.target.files[0].name.split('.').pop().toLowerCase();
                var fileSize = scope.fileInput.get(0).files[0].size; // in bytes 
                scope.$parent.fo.lv.OtherFileName = changeEvent.target.files[0].name;
                var allow = true;
                var _URL = window.URL || window.webkitURL;
                var image;

                image = new Image();
                image.onload = function () {

                    var totalSize = 0;
                    for (var r = 0; r < scope.$parent.$parent.fo.lv.filesList.length; r++) {
                        totalSize = totalSize + scope.$parent.$parent.fo.lv.filesList[r].FileSize;
                    }
                    totalSize = totalSize + fileSize;
                    console.log('totalSize', totalSize);

                    if (fileSize > scope.maxSize || totalSize > scope.maxSize) {
                        $timeout(function () {
                            message.showClientSideErrors('Please select image with appropriate size (Maximum 20 MB).');
                        });
                        element.val(null);
                    }

                    else if ($.inArray(ext, ['png', 'jpg', 'jpeg']) === -1) {
                        allow = false;
                        $timeout(function () {
                            message.showClientSideErrors('Please select image with valid extension.');
                        });
                        element.val(null);
                    }

                    else {
                        var reader = new FileReader();
                        reader.onload = function (loadEvent) {
                            scope.$apply(function () {
                                scope.fileRead = loadEvent.target.result;

                            });
                        };
                        reader.readAsDataURL(changeEvent.target.files[0]);
                        message.clearMessage();
                    }
                };
                image.src = _URL.createObjectURL(this.files[0]);
                $timeout(function () {
                    scope.$parent.fo.uploadExcel();
                }, 100);
            });
        }
    }
})();

/*global $ */
(function () {

    'use strict';

    angular
        .module('app.blocks')
        .directive('fileReadExcel', fileReadExcel);

    fileReadExcel.$inject = ['$timeout', 'message'];

    function fileReadExcel($timeout, message) {

        var directive = {
            link: link,
            restrict: 'EA',
            scope: {
                fileReadExcel: '=',
            },
        };
        return directive;

        function link(scope, element) {
            element.bind('change', function (changeEvent1) {
                message.clearMessage();
                var _URL1 = window.URL || window.webkitURL;
                var image1 = new Image();
                var reader1 = new FileReader();
                reader1.onload = function (loadEvent) {
                    scope.$apply(function () {
                        scope.fileReadExcel = loadEvent.target.result;
                    });
                };
                reader1.readAsDataURL(changeEvent1.target.files[0]);
                image1.src = _URL1.createObjectURL(this.files[0]);
                console.log(scope.$parent.fo);
                if (scope.$parent.fo.lv.IsCalled === true) {
                    $timeout(function () {
                        scope.$parent.fo.uploadExcel();
                    });
                }
            });
        }
    }

})();

