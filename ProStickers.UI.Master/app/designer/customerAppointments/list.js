(function () {
    'use strict';

    angular
        .module('app.appointment')
        .controller('AppointmentList', AppointmentList);

    AppointmentList.$inject = ['$location', '$state', 'stackView', 'InitialDataOfAppointmentList', 'AppointmentListFactory'];

    function AppointmentList($location, $state, stackView, InitialDataOfAppointmentList, AppointmentListFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Customer Appointments';
        fo.lv.setFooterPaddingRecord = null;
        fo.lv.dateList = [];
        function initilizeController() {

            var obj = stackView.getLastViewDetail();
            console.log('InitialDataOfAppointmentList', InitialDataOfAppointmentList);
            if (obj.formName === 'AppointmentList') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                submitListOperation();
            }

            else {
                fo.lv.dateList = InitialDataOfAppointmentList.dateList;
                fo.lv.statusList = InitialDataOfAppointmentList.statusList;
                fo.vm = InitialDataOfAppointmentList.viewModel;
                fo.vm.SearchList[2].Value = fo.lv.statusList[1].Text;
                fo.lv.AppointmentDate = 1;
            }
        }

        initilizeController();

        fo.OpenDetail = function (Number, Date) {
            stackView.pushViewDetail({
                controller: 'AppointmentList',
                formObject: fo, url: 'Appointments',
                formName: 'AppointmentList'
            });
            $state.go('AppointmentDetail', { OrderNumber: Number, OrderDate: Date });
        };

        fo.search = function () {
            for (var s = 0; s < fo.lv.dateList.length; s++) {
                if (fo.lv.dateList[s].SNo === fo.lv.AppointmentDate) {
                    fo.vm.SearchList[0].Value = fo.lv.dateList[s].FromDate;
                    fo.vm.SearchList[1].Value = fo.lv.dateList[s].ToDate;
                }
            }
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

        function makePageNumber() {
            fo.lv.pageNumberList = [];
            fo.lv.lastPage = Math.ceil(parseFloat(fo.vm.RecordsCount) / parseFloat(fo.vm.PageSize));
            for (var i = 0; (i < fo.vm.PagerLimit && ((fo.vm.CurrentStartPage + i) <= fo.lv.lastPage)) ; i++) {
                fo.lv.pageNumberList.push(fo.vm.CurrentStartPage + i);
            }
        }

        function submitListOperation() {
            console.log('fo.vm @ submitListOperation', fo.vm);
            AppointmentListFactory.submit(fo.vm).then(function (data) {
                console.log('fo.vm @ get', data);
                fo.vm = data.ReturnedData;
                makePageNumber();
            });
        }

    }
})();
