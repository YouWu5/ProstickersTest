(function () {
    'use strict';

    angular
        .module('app.appointment')
        .controller('DesignDetails', DesignDetails);

    DesignDetails.$inject = ['$state', 'stackView', '$compile', '$scope', '$ngBootbox', '$timeout', 'message',
        'helper', 'DesignDetailsFactory', 'InitialDataOfDesignDetail'];

    function DesignDetails($state, stackView, $compile, $scope, $ngBootbox, $timeout, message,
        helper, DesignDetailsFactory, InitialDataOfDesignDetail) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Designer\'s Note';
        fo.lv.setFooterPaddingRecord = null;

        function initilizeController() {
            fo.vm = InitialDataOfDesignDetail.viewModel;
            console.log('InitialDataOfDesignDetail', InitialDataOfDesignDetail);
        }

        initilizeController();

        fo.save = function () {
            console.log('viewmodel on save', angular.toJson(fo.vm));
            DesignDetailsFactory.submit(fo.vm).then(function (data) {
                console.log('data on save', data);
                message.showServerSideMessage(data, true);
                stackView.closeView();
            });
        };

        fo.Close = function () {
            var obj = stackView.getLastViewDetail();

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
                                stackView.openView('/');
                            }
                        }
                    }
                }
            };

            if ($scope.DesignDetailsForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                if (obj.formName !== 'Home') {
                    stackView.closeView();
                    return;
                }
                else {
                    stackView.openView('/');
                }
            }
        };

    }
})();
