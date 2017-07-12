(function () {
    'use strict';

    angular
        .module('app.home')
        .controller('designerHome', designerHome);

    designerHome.$inject = ['$state', 'stackView', 'AppointmentListFactory'];

    function designerHome($state, stackView, AppointmentListFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Customer Appointments';
        fo.lv.setFooterPaddingRecord = null;

        function initilizeController() {

            var obj = stackView.getLastViewDetail();

            if (obj.formName === 'DesignerHome') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                submitListOperation();
            }

            else {
                AppointmentListFactory.getAppointmentList().then(function (data) {
                    console.log('InitialDataOfAppointmentList', data);
                    fo.vm = data;
                });
            }
        }

        initilizeController();

        fo.OpenDetail = function (Number, Date) {
            stackView.pushViewDetail({
                controller: 'designerHome',
                formObject: fo, url: '/',
                formName: 'DesignerHome'
            });
            $state.go('AppointmentDetail', { OrderNumber: Number, OrderDate: Date });
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

        fo.showMore = function () {
            $state.go('Appointments');
        };
    }
})();
