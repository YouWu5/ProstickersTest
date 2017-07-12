(function () {
    'use strict';

    angular
        .module('app.appointment')
        .controller('AppointmentList', AppointmentList);

    AppointmentList.$inject = ['stackView', 'helper', '$location', '$state', '$timeout', '$ngBootbox', 'appointmentListFactory', 'initialDataOfAppointmentList', 'localStorageService'];

    function AppointmentList(stackView, helper, $location, $state, $timeout, $ngBootbox, appointmentListFactory, initialDataOfAppointmentList, localStorageService) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.vm.CallRequestViewModel = {};
        fo.vm.AppointmentListViewModel = {};
        fo.lv.title = 'Appointments';
        fo.lv.IsCalled = false;
        fo.lv.appointmentList = [];

        function initializeController() {

            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'AppointmentList') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                stackView.discardViewDetail();
            }
            else {
                fo.vm.CallRequestViewModel = initialDataOfAppointmentList.viewModel;
                fo.vm.AppointmentListViewModel = initialDataOfAppointmentList.appointmentListViewModel;
                console.log('fo.vm.AppointmentListViewModel 2', fo.vm.AppointmentListViewModel.Data);
                makePageNumber();
            }
        }

        initializeController();

        fo.OpenDetail = function (AppointmentNumber) {
            stackView.pushViewDetail({
                controller: 'AppointmentList',
                formObject: fo, url: 'AppointmentList',
                formName: 'AppointmentList'
            });
            $state.go('AppointmentDetail', { Number: AppointmentNumber, redirect: true });
        };
 
        fo.addAppointment = function () {
            stackView.pushViewDetail({
                controller: 'AppointmentList',
                formObject: fo, url: 'AppointmentList',
                formName: 'AppointmentList'
            });
            $state.go('AppointmentCalendar', { redirect: true });
           
        };
 
        fo.contactMe = function () {
            // fo.vm.CustomerID = '00000005';
            var userID = localStorageService.get('UserSession');
            console.log('userID', userID);
            fo.vm.CallRequestViewModel.CustomerID = userID.UserID;
            fo.vm.CallRequestViewModel.RequestDateTime = helper.ConvertDateCST(new Date());
            console.log('contact me submit vm', fo.vm.CallRequestViewModel);
            appointmentListFactory.contactMe(fo.vm.CallRequestViewModel).then(function (data) {
                fo.lv.msg = data.Message.split('.');

                if (fo.lv.msg[1] !== '') {
                    fo.lv.msg[1] = fo.lv.msg[1] + '.';
                }

                if (fo.lv.msg[2] !== '' && fo.lv.msg[2] !== undefined) {
                    fo.lv.msg[2] = fo.lv.msg[2] + '.';
                }
                else {
                    fo.lv.msg[2] = ' ';
                }

                var dataMsg = '<div style="text-align: center">' +
                              '<div>' + fo.lv.msg[0] + '.' + '</div>' +
                              '<div>' + fo.lv.msg[1] + '</div>' +
                              '<div>' + fo.lv.msg[2] + '</div>' +
                              '</div>';

                var options = {
                    title: 'Contact Me Request',
                    message: dataMsg,
                    buttons: {
                        success: {
                            label: ' ',
                            className: 'fa fa-check-page',
                            callback: function () {
                                $timeout(function () {
                                }, 100);

                            }
                        }
                    }
                };
                $ngBootbox.customDialog(options);

            });
        };

        fo.listOperation = function (actionPerformed, currentPage, sortColumn) {
            switch (actionPerformed) {
                case 'NavigateToFirstPage':
                    {
                        fo.vm.AppointmentListViewModel.PageNumber = 1;
                        fo.vm.AppointmentListViewModel.CurrentLastPage = null;
                        fo.vm.AppointmentListViewModel.CurrentStartPage = 1;
                        fo.vm.AppointmentListViewModel.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();
                        break;
                    }
                case 'NavigateToLastPage':
                    {
                        fo.vm.AppointmentListViewModel.PageNumber = fo.lv.lastPage;
                        fo.vm.AppointmentListViewModel.CurrentLastPage = fo.lv.lastPage;

                        if ((fo.lv.lastPage % fo.vm.AppointmentListViewModel.PagerLimit) === 0) {
                            fo.vm.AppointmentListViewModel.CurrentStartPage = (fo.lv.lastPage - (fo.vm.AppointmentListViewModel.PagerLimit - 1));
                        }
                        else {

                            fo.vm.AppointmentListViewModel.CurrentStartPage = (fo.lv.lastPage - ((fo.lv.lastPage % fo.vm.AppointmentListViewModel.PagerLimit) - 1));
                        }
                        fo.vm.AppointmentListViewModel.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();

                        break;
                    }
                case 'NavigateToPreviousPage':
                    {
                        fo.vm.AppointmentListViewModel.PageNumber = fo.vm.AppointmentListViewModel.PageNumber - 1;
                        fo.vm.AppointmentListViewModel.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();
                        break;
                    }
                case 'NavigateToNextPage':
                    {
                        fo.vm.AppointmentListViewModel.PageNumber = fo.vm.AppointmentListViewModel.PageNumber + 1;
                        fo.vm.AppointmentListViewModel.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();
                        break;
                    }
                case 'NavigateToAtPage':
                    {
                        fo.vm.AppointmentListViewModel.PageNumber = currentPage;
                        fo.vm.AppointmentListViewModel.CurrentOperation = 'CurrentPageChanged';
                        submitListOperation();
                        break;
                    }
                case 'NavigateToOlderPages': {

                    fo.vm.AppointmentListViewModel.PageNumber = fo.vm.AppointmentListViewModel.CurrentStartPage - fo.vm.AppointmentListViewModel.PagerLimit;
                    fo.vm.AppointmentListViewModel.CurrentLastPage = fo.vm.AppointmentListViewModel.CurrentStartPage - 1;
                    fo.vm.AppointmentListViewModel.CurrentStartPage = fo.vm.AppointmentListViewModel.CurrentStartPage - fo.vm.AppointmentListViewModel.PagerLimit;
                    fo.vm.AppointmentListViewModel.CurrentOperation = 'CurrentPageChanged';
                    submitListOperation();
                    break;
                }
                case 'NavigateToNewerPages': {

                    fo.vm.AppointmentListViewModel.PageNumber = fo.vm.AppointmentListViewModel.CurrentLastPage + 1;
                    fo.vm.AppointmentListViewModel.CurrentStartPage = fo.vm.AppointmentListViewModel.CurrentLastPage + 1;
                    fo.vm.AppointmentListViewModel.CurrentLastPage = fo.vm.AppointmentListViewModel.CurrentLastPage + fo.vm.AppointmentListViewModel.PagerLimit;
                    fo.vm.AppointmentListViewModel.CurrentOperation = 'CurrentPageChanged';
                    submitListOperation();
                    break;
                }
                case 'ChangePageSize':
                    {
                        fo.vm.AppointmentListViewModel.PageNumber = 1;
                        fo.vm.AppointmentListViewModel.CurrentLastPage = null;
                        fo.vm.AppointmentListViewModel.CurrentStartPage = 1;
                        fo.vm.AppointmentListViewModel.CurrentOperation = 'PageSizeChanged';
                        submitListOperation();
                        break;
                    }
                case 'Sort':
                    {
                        fo.vm.AppointmentListViewModel.Sort = sortColumn;
                        fo.vm.AppointmentListViewModel.CurrentOperation = 'SortOrderChanged';
                        fo.vm.AppointmentListViewModel.PageNumber = 1;
                        submitListOperation();
                        break;
                    }
            }
        };

        function submitListOperation() {
            console.log('fo.vm @ submit', fo.vm.AppointmentListViewModel);
            appointmentListFactory.submit(fo.vm.AppointmentListViewModel).then(function (data) {
                console.log('fo.vm @ get', fo.vm.AppointmentListViewModel);
                fo.vm.AppointmentListViewModel = data.ReturnedData;
                makePageNumber();
            });
        }

        function makePageNumber() {
            fo.lv.pageNumberList = [];
            fo.lv.lastPage = Math.ceil(parseFloat(fo.vm.AppointmentListViewModel.RecordsCount) / parseFloat(fo.vm.AppointmentListViewModel.PageSize));
            for (var i = 0; (i < fo.vm.AppointmentListViewModel.PagerLimit && ((fo.vm.AppointmentListViewModel.CurrentStartPage + i) <= fo.lv.lastPage)) ; i++) {
                fo.lv.pageNumberList.push(fo.vm.AppointmentListViewModel.CurrentStartPage + i);
            }
        }

    }
})();
