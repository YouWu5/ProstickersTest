(function () {
    'use strict';

    angular
        .module('app.profile')
        .controller('CustomerProfile', CustomerProfile);

    CustomerProfile.$inject = ['$location', '$scope', '$state', 'helper', 'message', 'initialDataOfCustomerProfile', 'customerProfileFactory', 'localStorageService'];

    function CustomerProfile($location, $scope, $state, helper, message, initialDataOfCustomerProfile, customerProfileFactory, localStorageService) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Customer Profile';
        fo.lv.isRequired = false;
        fo.lv.stateNameTextBox = false;
        fo.vm.FirstName = 'Kenny';
        fo.vm.LastName = 'jeff';
        fo.vm.Email = 'kenny@gmail.com';
        fo.lv.minLength = 0;
        fo.lv.maxLength = 0;

        initializeController();

        function initializeController() {
            fo.vm = initialDataOfCustomerProfile.profileDetail;
            if (!fo.vm.CountryID) {
                fo.vm.CountryID = 0;
            }
            fo.lv.countryList = [{ Name: 'Select', ID: 0 }, { Name: 'United States', ID: 184}];
            validatePostalCode(fo.vm.CountryID);

            if ((fo.vm.CountryID === 184)) {
                fo.lv.stateNameTextBox = false;
                customerProfileFactory.getStateList(fo.vm.CountryID).then(function (data) {
                    fo.lv.stateList = data;
                    fo.lv.stateList.unshift({ Text: 'Select', Value: 0 });
                    fo.lv.StateName = fo.vm.StateName !== null && fo.vm.StateName !== undefined && fo.vm.StateName !== '' ? fo.vm.StateName : null;
                });
            }
            else {
                fo.lv.stateNameTextBox = true;
                fo.lv.StateName = fo.vm.StateName;
            }
        }

        fo.selectedState = function () {
            angular.forEach(fo.lv.stateList, function (value) {
                if (fo.vm.StateID === value.Value) {
                    fo.lv.StateName = value.Text;
                    fo.vm.StateName = fo.lv.StateName;
                }
            });
        };

        fo.getStateList = function (id) {
            fo.vm.PostalCode = null;
            validatePostalCode(id);

            if (id === null || id === undefined || id === '') {
                fo.lv.stateList = [];
                fo.vm.CountryID = 0;
                fo.lv.StateName = null;
                fo.vm.StateID = 0;
                fo.vm.PostalCode = null;
            }

            if (id === 184 || id === 33) {
                fo.lv.stateNameTextBox = false;
                customerProfileFactory.getStateList(id).then(function (data) {
                    fo.lv.stateList = data;
                    fo.lv.stateList.unshift({ Text: 'Select', Value: 0 });
                });

            }
            else {
                fo.lv.StateName = null;
                fo.vm.StateID = 0;
                fo.lv.stateNameTextBox = true;
            }
        };

        function validateAddress() {
            fo.lv.allFeildVacant = false;
            fo.lv.allFeildFilled = false;

            if (fo.vm.CountryID > 0 && fo.vm.CountryID !== 184 && fo.vm.CountryID !== 33) {
                fo.vm.StateName = fo.lv.StateName;
            }
            if (fo.vm.CountryID) {
                angular.forEach(fo.lv.countryList, function (value) {
                    if (fo.vm.CountryID === value.ID) {
                        fo.vm.CountryName = value.Name;
                    }
                });
            }
            else {
                fo.vm.CountryName = '';
            }
            if ((fo.vm.Address1 === null || fo.vm.Address1 === undefined || fo.vm.Address1 === '') &&
                (fo.vm.City === null || fo.vm.City === undefined || fo.vm.City === '') &&
                (fo.lv.StateName === null || fo.lv.StateName === '' || fo.lv.StateName === undefined) &&
                (fo.vm.PostalCode === null || fo.vm.PostalCode === undefined || fo.vm.PostalCode === '') &&
                (fo.vm.CountryID === 0 || fo.vm.CountryID === '0' || fo.vm.CountryID === undefined)) {

                fo.lv.isRequired = false;
                fo.lv.isAddressFormInvalid = false;
                fo.lv.allFeildVacant = true;
            }

            if ((fo.vm.Address1 !== null && fo.vm.Address1 !== undefined && fo.vm.Address1 !== '') &&
                (fo.vm.City !== null && fo.vm.City !== undefined && fo.vm.City !== '') &&
                (fo.lv.StateName !== null && fo.lv.StateName !== '' && fo.lv.StateName !== undefined) &&
                (fo.vm.PostalCode !== null && fo.vm.PostalCode !== undefined && fo.vm.PostalCode !== '') &&
                (fo.vm.CountryID !== 0 && fo.vm.CountryID !== '0' && fo.vm.CountryID !== undefined)) {

                fo.lv.allFeildFilled = true;
                fo.lv.isRequired = false;
                fo.lv.isAddressFormInvalid = false;
            }
            else {
                if (!fo.lv.allFeildVacant) {
                    fo.lv.isRequired = true;
                    fo.lv.isAddressFormInvalid = true;
                    return;
                }
            }
        }

        fo.save = function () {
            validateAddress();
            console.log('fo.vm ', fo.vm);
            if ((fo.vm.CountryID === 0 || fo.vm.CountryID === undefined) && fo.lv.isAddressFormInvalid) {
                message.showClientSideErrors('Please select country', true);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }

            if ((fo.vm.CountryID === 184) && fo.vm.StateID === 0) {
                message.showClientSideErrors('Please select state', true);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }

            if ($scope.CustomerProfileForm.$invalid) {
                console.log('$scope.CustomerProfileForm', $scope.CustomerProfileForm.$error);
                helper.scrollToError();
                fo.lv.isFormInvalid = true;
                return;
            }
            
            if (fo.lv.StateName === null || fo.lv.StateName === undefined || fo.lv.StateName === '') {
                fo.vm.StateName = fo.lv.StateName;
            }
            if (fo.lv.allFeildVacant || fo.lv.allFeildFilled) {
                fo.vm.CountryList = [];
                customerProfileFactory.updateProfile(fo.vm).then(function (data) {
                    console.log('submit response', data);
                    if (data.Result === 1)          // Success
                    {
                        localStorageService.set('IsUserMenuVisible', true);
                        message.showServerSideMessage(data, true);
                        $state.go('/', { redirect: true });
                    }
                    helper.setIsSubmitted(false);
                });
            }
        };

        function validatePostalCode(countryID) {

            if (countryID === 33) {
                fo.lv.minLength = 7;
                fo.lv.maxLength = 7;
                fo.lv.postalCodePattern = '^([a-zA-Z0-9]{5}|[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9] [a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9])';
            }
            else if (countryID === 184) {
                fo.lv.minLength = 5;
                fo.lv.maxLength = 6;
                fo.lv.postalCodePattern = '^[0-9]*$';
            }
            else {
                fo.lv.minLength = 1;
                fo.lv.maxLength = 20;
                fo.lv.postalCodePattern = '';
            }
        }

    }
})();