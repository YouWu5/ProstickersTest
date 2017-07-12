(function () {
    'use strict';

    angular
        .module('app.design')
        .controller('DesignsList', DesignsList);

    DesignsList.$inject = ['stackView', '$location', '$state', 'designListFactory', 'initialDataOfDesignList'];

    function DesignsList(stackView, $location, $state, designListFactory, initialDataOfDesignList) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Designs';
        fo.lv.designList = [];

        function initializeController() {

            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'DesignsList') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                stackView.discardViewDetail();
            }
            else {
                fo.vm = initialDataOfDesignList.viewModel;               
                makePageNumber();
                for (var i2 = 0; i2 < fo.vm.Data.length; i2++) {                    
                    if (fo.vm.Data[i2].DesignImage && fo.vm.Data[i2].DesignImage.ImageBuffer !== null && fo.vm.Data[i2].DesignImage.ImageBuffer !== ' ') {
                        fo.vm.Data[i2].DesignImage.ImageBuffer = 'data:image/png;base64,' + fo.vm.Data[i2].DesignImage.ImageBuffer.toString();
                    }
                }
            }

            console.log('fo.vm.Data after', fo.vm.Data);
        }

        initializeController();

        fo.OpenDetail = function (DesignNumber) {
            stackView.pushViewDetail({
                controller: 'DesignsList',
                formObject: fo, url: 'DesignsList',
                formName: 'DesignsList'
            });
            $state.go('DesignsDetail', { Number: DesignNumber, redirect: true });
        };

        fo.OpenBuy = function (DesignNumber, AppointmentNumber) {
            stackView.pushViewDetail({
                controller: 'DesignsList',
                formObject: fo, url: 'DesignsList',
                formName: 'DesignsList'
            });
            $state.go('OrdersCreate', { DesignNumber: DesignNumber, AppointmentNumber: AppointmentNumber, redirect: true });
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

        function submitListOperation() {
            console.log('fo.vm @ submit', fo.vm);
            designListFactory.submit(fo.vm).then(function (data) {
                console.log('fo.vm @ get', fo.vm);
                fo.vm = data.ReturnedData;
                makePageNumber();
            });
        }

        function makePageNumber() {
            fo.lv.pageNumberList = [];
            fo.lv.lastPage = Math.ceil(parseFloat(fo.vm.RecordsCount) / parseFloat(fo.vm.PageSize));
            for (var i = 0; (i < fo.vm.PagerLimit && ((fo.vm.CurrentStartPage + i) <= fo.lv.lastPage)) ; i++) {
                fo.lv.pageNumberList.push(fo.vm.CurrentStartPage + i);
            }
        }

    }
})();
