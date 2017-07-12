(function () {
    'use strict';

    angular
        .module('app.users')
        .controller('UsersList', UsersList);

    UsersList.$inject = ['$location', '$state', 'stackView', 'UsersListFactory', '$ngBootbox', '$timeout', 'initialDataOfUsersList', 'message'];

    function UsersList($location, $state, stackView, UsersListFactory, $ngBootbox, $timeout, initialDataOfUsersList, message) {
        /* jshint validthis:true */

        /////////// Variable declaration starts here //////////////

        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Users';
        fo.lv.inActiveModel = {};

        //////////// Variable declaration. ends here//////////////

        /////////// Initilize controller starts here ////////////

        initializeController();

        function initializeController() {
            var obj = stackView.getLastViewDetail();
            if (obj.formName === 'UsersList') {
                fo.vm = obj.formObject.vm;
                fo.lv = obj.formObject.lv;
                stackView.discardViewDetail();
            }
            else {
                fo.vm = initialDataOfUsersList.viewModel;
                makePageNumber();
                console.log('fo.vm @ initialize', fo.vm);
            }
        }

        /////////// Initilize controller ends here //////////////

        ////////////// Click methods start here ////////////////

        fo.OpenDetail = function (id) {
            stackView.pushViewDetail({
                controller: 'UsersList',
                formObject: fo, url: 'UsersList',
                formName: 'UsersList'
            });
            $state.go('UsersUpdate', { ID: id, redirect: true });
        };

        fo.Create = function () {
            stackView.pushViewDetail({
                controller: 'UsersList',
                formObject: fo, url: 'UsersList',
                formName: 'UsersList'
            });
            $state.go('UsersCreate');
        };

        fo.updateActive = function (id, updatedTS, isActive) {
            fo.lv.inActiveModel = {
                UserID: id,
                UpdatedTS: updatedTS,
                Active: isActive
            };
            var options = {
                message: 'Are you sure you want to make the user inactive?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            UsersListFactory.updateActive(fo.lv.inActiveModel).then(function (data) {
                                if (data.Message === 'User updated successfully.') // Success
                                {
                                    message.showServerSideMessage(data, true);
                                    $state.reload();
                                }
                                else {
                                    message.showClientSideErrors(data.Message);
                                    submitListOperation();
                                }
                            });
                        }
                    }
                }
            };
            if (!isActive) {
                $ngBootbox.customDialog(options);
            }
            else {
                UsersListFactory.updateActive(fo.lv.inActiveModel).then(function (data) {
                    if (data.Result === 1) // Success
                    {
                        message.showServerSideMessage(data, true);
                        $state.reload();
                    }
                });
            }
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

        ///////////////// Click Methods Ends Here ///////////////////

        ////////////////// Helper methods starts Here ////////////////

        function submitListOperation() {
            UsersListFactory.submit(fo.vm).then(function (data) {
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
