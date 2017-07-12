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
                scope.maxSize = 20000000;
                var ext = changeEvent.target.files[0].name.split('.').pop();
                var fileSize = scope.fileInput.get(scope.$parent.fo.lv.isImage).files[0].size; // in bytes 
                var _URL = window.URL || window.webkitURL;
                var image = new Image();
                var fileName = changeEvent.target.files[0].name;
                if (fileSize > scope.maxSize) {
                    $timeout(function () {
                        message.showClientSideErrors('Please select image with appropriate size (Maximum 20 MB).');
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
                    if (scope.$parent.fo.lv.isImage === 0) {
                        scope.$parent.fo.lv.DesignName = fileName;
                        scope.$parent.fo.lv.ext1 = ext;
                    }
                    if (scope.$parent.fo.lv.isImage === 1) {
                        scope.$parent.fo.lv.VectorName = fileName;
                        scope.$parent.fo.lv.ext2 = ext;
                    }
                    if (scope.$parent.fo.lv.isImage === 2) {
                        scope.$parent.fo.lv.OtherFileName = fileName;
                        scope.$parent.fo.lv.ext3 = ext;
                    }
                    message.clearMessage();
                }
                image.src = _URL.createObjectURL(this.files[0]);
            });
        }
    }
})();

/*global $ */
(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('fileReadUserImage', fileReadUserImage);

    fileReadUserImage.$inject = ['message', '$timeout'];

    function fileReadUserImage(message, $timeout) {
        var directive = {
            link: link,
            restrict: 'EA',
            scope: {
                fileReadUserImage: '=',
            },
        };
        return directive;

        function link(scope, element) {
            element.bind('change', function (changeEvent) {
                scope.fileInput = $('.filestyle');
                scope.maxSize = 20000000;
                var ext = changeEvent.target.files[0].name.split('.').pop().toLowerCase();
                var fileSize = scope.fileInput.get(0).files[0].size; // in bytes 
                var allow = true;
                var _URL = window.URL || window.webkitURL;
                var image;

                image = new Image();
                image.onload = function () {

                    if (fileSize > scope.maxSize) {
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
                                scope.fileReadUserImage = loadEvent.target.result;
                            });
                        };
                        reader.readAsDataURL(changeEvent.target.files[0]);
                        message.clearMessage();
                    }
                };
                image.src = _URL.createObjectURL(this.files[0]);
            });
        }
    }
})();

/*global $ */
(function () {
    'use strict';

    angular
        .module('app.blocks')
        .directive('fileReadOtherFile', fileReadOtherFile);

    fileReadOtherFile.$inject = ['message', '$timeout'];

    function fileReadOtherFile(message, $timeout) {
        var directive = {
            link: link,
            restrict: 'EA',
            scope: {
                fileReadOtherFile: '=',
            },
        };
        return directive;

        function link(scope, element) {
            element.bind('change', function (changeEvent) {
                scope.fileInput = $('.filestyle');
                scope.maxSize = 20000000;
                var ext = changeEvent.target.files[0].name.split('.').pop().toLowerCase();
                var fileSize = scope.fileInput.get(0).files[0].size; // in bytes 
                var fileName = changeEvent.target.files[0].name;
                var uploadFileName = fileName.replace(ext, '');
                uploadFileName = uploadFileName.substring(0, (uploadFileName.length - 1));
                var _URL = window.URL || window.webkitURL;
                var image;
                image = new Image();
                image.onload = function () {
                    if (fileSize > scope.maxSize) {
                        $timeout(function () {
                            message.showClientSideErrors('Please select image with appropriate size (Maximum 20 MB).');
                        });
                        element.val(null);
                    }
                    else {
                        var reader = new FileReader();
                        reader.onload = function (loadEvent) {
                            scope.$apply(function () {
                                scope.fileReadOtherFile = loadEvent.target.result;
                            });
                        };
                        reader.readAsDataURL(changeEvent.target.files[0]);
                        if (scope.fileReadOtherFile !== null) {
                            $timeout(function () {
                                scope.$parent.fo.lv.ext = ext;
                                scope.$parent.fo.lv.uploadFileName = uploadFileName;
                                scope.$parent.fo.lv.fileName = fileName;
                            });
                        }
                        message.clearMessage();
                    }
                };
                image.src = _URL.createObjectURL(this.files[0]);
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