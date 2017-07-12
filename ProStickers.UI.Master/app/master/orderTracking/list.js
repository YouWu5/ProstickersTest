(function () {
    'use strict';

    angular
        .module('app.orderTracking')
        .controller('OrderTrackingList', OrderTrackingList);

    OrderTrackingList.$inject = ['$scope', '$state', 'OrderTrackingListFactory', 'helper', 'initialDataOfOrderTrackingList', 'stackView'];

    function OrderTrackingList($scope, $state, OrderTrackingListFactory, helper, initialDataOfOrderTrackingList, stackView) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Order Tracking Number';

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'OrderTrackingList') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                submitListOperation();
            }
            else {
                fo.vm = initialDataOfOrderTrackingList.viewModel;
                console.log('fo.vm @ initialize', fo.vm);
                fo.lv.statusList = initialDataOfOrderTrackingList.statusList;
                fo.lv.currentDate = helper.ConvertDateCST(new Date());
                fo.lv.StartDate = fo.lv.EndDate = fo.lv.currentDate;
                fo.vm.SearchList[2].Value = fo.lv.StatusID = 1;
            }
            setMinMaxDate(new Date());
        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

        fo.open = function ($event, opened) {
            $event.preventDefault();
            $event.stopPropagation();
            if (fo.openedStart === true) {
                fo.openedStart = false;
            }
            else if (opened === 'openedStart') {

                fo.openedEnd = false;
                fo.openedStart = true;
            }
            if (fo.openedEnd === true) {
                fo.openedEnd = false;
            }
            else if (opened === 'openedEnd') {
                fo.openedStart = false;
                fo.openedEnd = true;
            }
        };

        fo.setDate = function (startDate, endDate, name) {
            if (name === 'startDate' && new Date(startDate) > new Date(endDate)) {
                fo.lv.EndDate = startDate;
            }
            if (name === 'endDate' && new Date(endDate) < new Date(startDate)) {
                fo.lv.StartDate = endDate;
            }
            setMinMaxDate(fo.lv.StartDate);
        };

        fo.OpenDetail = function (orderNo) {
            stackView.pushViewDetail({
                controller: 'OrderTrackingList',
                formObject: fo, url: 'OrderTrackingList',
                formName: 'OrderTrackingList'
            });
            $state.go('OrderTrackingUpdate', { OrderNo: orderNo, redirect: true });
        };

        fo.search = function () {
            if ($scope.orderListForm.$invalid) {
                console.log('$scope.orderListForm', $scope.orderListForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            fo.vm.SearchList[0].Value = helper.formatDate(fo.lv.StartDate);
            fo.vm.SearchList[1].Value = helper.formatDate(fo.lv.EndDate);
            fo.vm.SearchList[2].Value = fo.lv.StatusID;
            fo.vm.SearchList[3].Value = fo.lv.Customer;
            fo.vm.CurrentOperation = 'SearchParamChanged';
            fo.vm.Data = [];
            submitListOperation();
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

        fo.setIdOfCustomer = function (obj) {
            fo.vm.SearchList[3].Value = obj.Text;
        };

        fo.getCustomerList = function (searchParam) {
            return OrderTrackingListFactory.getCustomerList(searchParam).then(function (data) {
                return data;
            });
        };

        ///////////////// Click Methods Ends Here ///////////////////

        ////////////////// Helper methods starts Here ////////////////

        function setMinMaxDate(dateFrom) {
            fo.lv.startDateOptions = {
                startingDay: 1,
                showWeeks: false,
                initDate: null,
            };
            fo.lv.endDateOptions = {
                startingDay: 1,
                showWeeks: false,
                initDate: null,
                minDate: dateFrom,
            };
        }

        function submitListOperation() {
            console.log('fo.vm @ submit', fo.vm);
            OrderTrackingListFactory.submit(fo.vm).then(function (data) {
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

        ////////////////// Helper methods ends Here /////////////////

    }
})();
