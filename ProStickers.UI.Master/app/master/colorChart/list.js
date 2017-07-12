(function () {
    'use strict';

    angular
        .module('app.colorChart')
        .controller('ColorChartList', ColorChartList);

    ColorChartList.$inject = ['$location', '$state', 'ColorChartListFactory', 'stackView', '$ngBootbox', '$timeout', 'helper', 'message', 'initialDataOfColorChartList'];

    function ColorChartList($location, $state, ColorChartListFactory, stackView, $ngBootbox, $timeout, helper, message, initialDataOfColorChartList) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Color Chart';

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'ColorChartList') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                stackView.discardViewDetail();
            }
            else {
                fo.vm = initialDataOfColorChartList.viewModel;
                makePageNumber();
                console.log('fo.vm @ initialize', fo.vm);
            }

        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

        fo.OpenDetail = function (id) {
            stackView.pushViewDetail({
                controller: 'ColorChartList',
                formObject: fo, url: 'ColorChartList',
                formName: 'ColorChartList'
            });
            $state.go('ColorChartUpdate', { ID: id, redirect: true });
        };

        fo.Create = function () {
            stackView.pushViewDetail({
                controller: 'ColorChartList',
                formObject: fo, url: 'ColorChartList',
                formName: 'ColorChartList'
            });
            $state.go('ColorChartCreate');
        };

        fo.listOperation = function (actionPerformed, currentPage, sortColumn) {
            switch (actionPerformed) {
                case 'NavigateToFirstPage':
                    {
                        fo.vm.PageNumber = 1;
                        fo.vm.CurrentLastPage = null;
                        fo.vm.CurrentStartPage = 1;
                        fo.vm.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();
                        break;
                    }

                case 'NavigateToLastPage':
                    {
                        fo.vm.PageNumber = fo.lv.lastPage;
                        fo.vm.CurrentLastPage = fo.lv.lastPage;

                        if ((fo.lv.lastPage % fo.vm.PagerLimit) === 0) {
                            fo.vm.CurrentStartPage = (fo.lv.lastPage - (fo.vm.PagerLimit - 1));
                        }
                        else {

                            fo.vm.CurrentStartPage = (fo.lv.lastPage - ((fo.lv.lastPage % fo.vm.PagerLimit) - 1));
                        }
                        fo.vm.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();

                        break;
                    }

                case 'NavigateToPreviousPage':
                    {
                        fo.vm.PageNumber = fo.vm.PageNumber - 1;
                        fo.vm.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();
                        break;
                    }

                case 'NavigateToNextPage':
                    {
                        fo.vm.PageNumber = fo.vm.PageNumber + 1;
                        fo.vm.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();
                        break;
                    }

                case 'NavigateToAtPage':
                    {
                        fo.vm.PageNumber = currentPage;
                        fo.vm.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();
                        break;
                    }

                case 'NavigateToOlderPages': {

                    fo.vm.PageNumber = fo.vm.CurrentStartPage - fo.vm.PagerLimit;
                    fo.vm.CurrentLastPage = fo.vm.CurrentStartPage - 1;
                    fo.vm.CurrentStartPage = fo.vm.CurrentStartPage - fo.vm.PagerLimit;
                    fo.vm.CurrentOperation = 'CurrentPageChanged';
                    submitListOperation();
                    break;
                }

                case 'NavigateToNewerPages': {

                    fo.vm.PageNumber = fo.vm.CurrentLastPage + 1;
                    fo.vm.CurrentStartPage = fo.vm.CurrentLastPage + 1;
                    fo.vm.CurrentLastPage = fo.vm.CurrentLastPage + fo.vm.PagerLimit;
                    fo.vm.CurrentOperation = 'CurrentPageChanged';
                    submitListOperation();
                    break;
                }

                case 'ChangePageSize':
                    {
                        fo.vm.PageNumber = 1;
                        fo.vm.CurrentLastPage = null;
                        fo.vm.CurrentStartPage = 1;
                        fo.vm.CurrentOperation = 'PageSizeChanged';
                        submitListOperation();
                        break;
                    }

                case 'Sort':
                    {
                        fo.vm.Sort = sortColumn;
                        fo.vm.CurrentOperation = 'SortOrderChanged';
                        fo.vm.PageNumber = 1;
                        submitListOperation();
                        break;
                    }
            }
        };

        fo.update = function (model) {
            ColorChartListFactory.update(model).then(function (data) {
                console.log('data', data);
                message.showServerSideMessage(data, false);
                submitListOperation();
            });
        };

        fo.delete = function (model) {
            var options = {
                message: 'Are you sure you want to delete this color?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            ColorChartListFactory.deleteColor(model).then(function (data) {
                                console.log('data', data);
                                submitListOperation();
                                message.showServerSideMessage(data, false);
                            });
                        }
                    }
                }
            };
            $ngBootbox.customDialog(options);
        };

        ///////////////// Click Methods Ends Here ///////////////////

        ////////////////// Helper methods starts Here ////////////////

        function submitListOperation() {
            ColorChartListFactory.submit(fo.vm).then(function (data) {
                fo.vm = data.ReturnedData;
                makePageNumber();
            });
        }

        function makePageNumber() {
            fo.lv.pageNumberList = [];
            fo.lv.lastPage = Math.ceil(parseFloat(fo.vm.RecordsCount) / parseFloat(fo.vm.PageSize));
            for (var i = 0; (i < fo.vm.PagerLimit && ((fo.vm.CurrentStartPage + i) <= fo.lv.lastPage)); i++) {
                fo.lv.pageNumberList.push(fo.vm.CurrentStartPage + i);
            }
        }

        ////////////////// Helper methods ends Here /////////////////

    }
})();
