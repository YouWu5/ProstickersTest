(function () {
    'use strict';

    angular
        .module('app.layout')
        .controller('Shell', Shell);

    Shell.$inject = ['helper', 'userInfo', '$facebook', 'message', '$state', 'stackView', 'shellFactory', '$scope', 'localStorageService', 'GoogleSignin', 'customerProfileFactory'];

    function Shell(helper, userInfo, $facebook, message, $state, stackView, shellFactory, $scope, localStorageService, GoogleSignin, customerProfileFactory) {
        var shl = this;
        shl.vm = {};
        shl.lv = {};
        shl.lv.title = 'Customer Profile';

        var loginViewModel = {};

        shl.lv.isLogin = false;
        shl.lv.isTermsService = false;

        shl.lv.topHeightPaddingOne = null;
        shl.lv.setFooterPaddingForm = null;
        shl.lv.setAgreementContainerHeight = null;

        shl.lv.customerProfile = false;

        var initializeController = function () {

            var accesstoken = localStorageService.get('accessToken');
            var userInfo = localStorageService.get('UserSession');
            shl.lv.isTermsService = localStorageService.get('aggrement');
            shl.lv.customerProfile = localStorageService.get('customerProfile');

            console.log('accesstoken', accesstoken);
            console.log('userInfo', userInfo);
            console.log('shl.lv.isTermsService', shl.lv.isTermsService);
            console.log('shl.lv.customerProfile', shl.lv.customerProfile);

            if (accesstoken !== null && userInfo !== null && shl.lv.isTermsService && shl.lv.customerProfile) {

                shl.lv.UserName = userInfo.Name;
                shl.lv.AssignedModuleList = userInfo.AssignedPageList;
                shl.lv.isLogin = true;
                shl.lv.isTermsService = true;
                shl.lv.customerProfile = true;
            }
            else if (accesstoken != null && userInfo !== null && !shl.lv.isTermsService) {
                shl.lv.isLogin = true;
                shl.lv.isTermsService = false;
                shl.lv.customerProfile = false;
            }
            else if (accesstoken != null && userInfo !== null && shl.lv.isTermsService && !shl.lv.customerProfile) {
                shl.lv.isLogin = true;
                shl.lv.customerProfile = false;
                shl.lv.UserName = userInfo.Name;
                getCustomerDetails();
            }
            else {
                shl.lv.isLogin = false;
                shl.lv.customerProfile = false;
                shl.lv.isTermsService = false;
            }

            shl.lv.errorMessage = localStorageService.get('LoginErrorMessage');

        };

        initializeController();

        shl.initializeViewsStack = function () {
            stackView.clearViewsStack();
            stackView.pushViewDetail({
                controller: 'HomeController', formObject: null, url: '/1', formName: 'Home'
            });
        };

        shl.service = function () {
            console.log('Service');
            $state.go('serviceTerms');
        };

        shl.privacyPolicy = function () {
            console.log('privacyPolicy');
            $state.go('privacyPolicy');
        };

        ///mobile menu toggle 
        shl.menuToggle = function () {
            $('#wrapper').toggleClass('active');
            $('.overlay').toggleClass('Hidden');
        };

        shl.menuTogglepage = function () {
            $('#wrapper').removeClass('active');
            $('.overlay').removeClass('Hidden');
        };

        var submitAgreementModel = {};
        shl.submitAgreement = function () {
            console.log('shl.lv.isAgree', userInfo);
            if (shl.lv.isAgree) {
                submitAgreementModel.Text = userInfo.UserID;
                submitAgreementModel.Value = null;

                shellFactory.submitAgreement(submitAgreementModel).then(function () {
                    localStorageService.set('aggrement', true);
                    shl.lv.isTermsService = true;
                    getCustomerDetails();

                });
            }

        };

        shl.Login = function (provider) {
            shl.lv.provider = provider;
            if (provider === 'facebook') {
                $facebook.login().then(function (response) {
                    if (response.status === 'connected') {
                        $facebook.api('/me?fields=id,name,first_name,last_name,middle_name,email,picture,gender').then(function (userResponse) {
                            console.log('userResponse', userResponse);

                            shl.lv.userResponse = userResponse;
                            loginViewModel.UserTypeID = 3;
                            loginViewModel.UserID = shl.lv.userResponse.email;
                            loginViewModel.Token = response.authResponse.accessToken;
                            loginViewModel.Provider = provider;
                            userLoginValidate();
                        });
                    }

                });
            }
            else {
                GoogleSignin.signIn().then(function (user) {
                    console.log('GoogleSignin', GoogleSignin);
                    localStorageService.set('accessToken', user.Zi.id_token);
                    var info = GoogleSignin.getBasicProfile();
                    shl.lv.userResponse = GoogleSignin.getUser().getBasicProfile();

                    loginViewModel.UserTypeID = 3;
                    loginViewModel.UserID = info.email;
                    console.log('user info', info);
                    loginViewModel.Token = user.Zi.id_token;
                    loginViewModel.Provider = provider;
                    userLoginValidate();

                });
            }
        };

        function userLoginValidate() {
            shellFactory.ObtainLocalAccessToken(loginViewModel).then(function (LocalAccessTokenResponse) {
                console.log('LocalAccessTokenResponse', LocalAccessTokenResponse);
                localStorageService.set('accessToken', LocalAccessTokenResponse.ReturnedData);

                shellFactory.GetDefault().then(function (defaultData) {
                    console.log('defaultData', defaultData);
                    shl.vm.userInfo = defaultData;

                    if (shl.lv.provider === 'facebook') {
                        shl.vm.userInfo.FirstName = shl.lv.userResponse.first_name;
                        shl.vm.userInfo.MiddleName = shl.lv.userResponse.middle_name;
                        shl.vm.userInfo.LastName = shl.lv.userResponse.last_name;
                        shl.vm.userInfo.ImageURL = shl.lv.userResponse.picture.data.url;
                        shl.vm.userInfo.FullName = shl.lv.userResponse.name;
                        shl.vm.userInfo.ID = shl.lv.userResponse.id;
                        shl.vm.userInfo.EmailID = shl.lv.userResponse.email;
                        shl.vm.userInfo.IsFacebookUser = true;
                    }

                    else {
                        shl.vm.userInfo.FirstName = shl.lv.userResponse.ofa;
                        //  shl.vm.userInfo.MiddleName = shl.lv.userResponse.middle_name;
                        shl.vm.userInfo.LastName = shl.lv.userResponse.wea;
                        shl.vm.userInfo.ImageURL = shl.lv.userResponse.Paa;
                        shl.vm.userInfo.FullName = shl.lv.userResponse.ig;
                        shl.vm.userInfo.ID = shl.lv.userResponse.Eea;
                        shl.vm.userInfo.EmailID = shl.lv.userResponse.U3;
                        shl.vm.userInfo.IsFacebookUser = false;
                    }

                    shellFactory.CustomerSession(shl.vm.userInfo).then(function (customerSessionData) {
                        console.log('customerSessionData', customerSessionData.ReturnedData);

                        userInfo = customerSessionData.ReturnedData;
                        shl.lv.UserName = customerSessionData.ReturnedData.Name;
                        localStorageService.set('UserSession', customerSessionData.ReturnedData);
                        shl.lv.AssignedModuleList = customerSessionData.ReturnedData.AssignedPageList;
                        shl.lv.isLogin = true;
                        if (customerSessionData.ReturnedData.IsPolicyAccepted) {
                            localStorageService.set('aggrement', true);
                            localStorageService.set('customerProfile', true);
                            shl.lv.isTermsService = true;
                            shl.lv.isLogin = true;
                            shl.lv.customerProfile = true;

                            window.location.reload();

                        }
                        else {
                            localStorageService.set('customerProfile', false);
                            shl.lv.customerProfile = false;
                            shl.lv.isTermsService = false;
                        }

                        localStorageService.set('UserSession', customerSessionData.ReturnedData);
                        localStorageService.set('accessToken', LocalAccessTokenResponse.ReturnedData);
                        console.log('shl.lv.AssignedModuleList', shl.lv.AssignedModuleList);

                    });
                });
            });
        }

        shl.LogOut = function () {
            var userInfo = localStorageService.get('UserSession');
            if (userInfo.IsFacebookUser) {
                $facebook.logout();
            }
            localStorageService.set('UserSession', null);
            localStorageService.set('aggrement', null);
            localStorageService.set('accessToken', null);
            //   localStorageService.set('customerProfile', null);
            localStorageService.set('AuthenticateData', null);
            shl.lv.isTermsService = null;
            shl.lv.isLogin = false;

            window.location.reload();
        };

        //////-*------------------------------------------------------////

        function getCustomerDetails() {
            var userInfo = localStorageService.get('UserSession');
            console.log('userInfo.UserID', userInfo);
            customerProfileFactory.getProfileDetailByID(userInfo.UserID).then(function (data) {
                shl.lv.customerViewModel = data;
                console.log('shl.lv.customerViewModel', shl.lv.customerViewModel);

                customerProfileFactory.getCountryList().then(function (data) {
                    shl.lv.countryList = data;
                    shl.lv.countryList.unshift({ Name: 'Select', ID: 0, Regax: '' });
                    console.log('shl.lv.countryList', shl.lv.countryList);
                    var id;
                    if (shl.lv.customerViewModel.CountryID) {
                        id = shl.lv.customerViewModel.CountryID;
                    }
                    else {
                        id = data[0].ID;
                    }
                    console.log('shl.lv.customerViewModel.CountryID', shl.lv.customerViewModel.CountryID);
                    console.log('id', id);

                    if (id === 184 || id === 33) {
                        shl.lv.stateNameTextBox = false;
                        customerProfileFactory.getStateList(id).then(function (data) {
                            shl.lv.stateList = data;
                            shl.selectedState();
                            shl.lv.stateList.unshift({ Text: 'Select', Value: 0 });
                        });

                    }
                    else {
                        shl.lv.StateName = shl.lv.customerViewModel.StateName;
                        shl.lv.customerViewModel.StateID = 0;
                        shl.lv.stateNameTextBox = true;
                    }

                });

            });
        }

        shl.submitProfile = function () {

            validateAddress();

            if ($scope.shl.CustomerProfilesForm.$invalid) {
                console.log('$scope.CustomerProfilesForm', $scope.shl.CustomerProfilesForm.$error);
                helper.scrollToError();
                shl.lv.isFormInvalid = true;
                return;
            }
            else {

                if ((shl.lv.customerViewModel.CountryID === 33 || shl.lv.customerViewModel.CountryID === 184) && shl.lv.customerViewModel.StateID === 0) {
                    message.showClientSideErrors('Please select state', true);
                    helper.scrollToError();
                    shl.lv.isFormInvalid = true;
                    return;
                }

                if (shl.lv.StateName === null || shl.lv.StateName === undefined || shl.lv.StateName === '') {
                    shl.lv.customerViewModel.StateName = shl.lv.StateName;
                }
                if (shl.lv.allFeildVacant || shl.lv.allFeildFilled) {
                    shl.lv.customerViewModel.CountryList = [];

                    console.log('shl.lv.customerViewModel on submit', shl.lv.customerViewModel);
                    shl.lv.customerProfile = true;
                    customerProfileFactory.updateProfile(shl.lv.customerViewModel).then(function (data) {
                        console.log('submit response', data);
                        if (data.Result === 1)          // Success
                        {
                            var customerSessionData = localStorageService.get('UserSession');
                            shl.lv.AssignedModuleList = customerSessionData.AssignedPageList;
                            console.log('shl.lv.AssignedModuleList', shl.lv.AssignedModuleList);
                            message.showServerSideMessage(data, true);
                            localStorageService.set('customerProfile', true);
                            shl.lv.customerProfile = true;
                            $state.go('/', { redirect: true });
                        }
                        helper.setIsSubmitted(false);
                    });
                }
            }

        };

        shl.selectedState = function () {
            angular.forEach(shl.lv.stateList, function (value) {
                if (shl.lv.customerViewModel.StateID === value.Value) {
                    shl.lv.StateName = value.Text;
                    shl.lv.customerViewModel.StateName = shl.lv.StateName;
                }
            });
        };

        shl.getStateList = function (id) {
            shl.lv.customerViewModel.PostalCode = null;
            validatePostalCode(id);

            if (id === null || id === undefined || id === '') {
                shl.lv.stateList = [];
                shl.lv.customerViewModel.CountryID = 0;
                shl.lv.StateName = null;
                shl.lv.customerViewModel.StateID = 0;
                shl.lv.customerViewModel.PostalCode = null;
            }

            if (id === 184 || id === 33) {
                shl.lv.stateNameTextBox = false;
                customerProfileFactory.getStateList(id).then(function (data) {
                    shl.lv.stateList = data;
                    shl.lv.stateList.unshift({ Text: 'Select', Value: 0 });
                });

            }
            else {
                shl.lv.StateName = null;
                shl.lv.customerViewModel.StateID = 0;
                shl.lv.stateNameTextBox = true;
            }
        };

        function validateAddress() {
            shl.lv.allFeildVacant = false;
            shl.lv.allFeildFilled = false;

            if (shl.lv.customerViewModel.CountryID > 0 && shl.lv.customerViewModel.CountryID !== 184 && shl.lv.customerViewModel.CountryID !== 33) {
                shl.lv.customerViewModel.StateName = shl.lv.StateName;
            }
            if (shl.lv.customerViewModel.CountryID) {
                angular.forEach(shl.lv.countryList, function (value) {
                    if (shl.lv.customerViewModel.CountryID === value.ID) {
                        shl.lv.customerViewModel.CountryName = value.Name;
                    }
                });
            }
            else {
                shl.lv.customerViewModel.CountryName = '';
            }
            if ((shl.lv.customerViewModel.Address1 === null || shl.lv.customerViewModel.Address1 === undefined || shl.lv.customerViewModel.Address1 === '') &&
                (shl.lv.customerViewModel.City === null || shl.lv.customerViewModel.City === undefined || shl.lv.customerViewModel.City === '') &&
                (shl.lv.StateName === null || shl.lv.StateName === '' || shl.lv.StateName === undefined) &&
                (shl.lv.customerViewModel.PostalCode === null || shl.lv.customerViewModel.PostalCode === undefined || shl.lv.customerViewModel.PostalCode === '') &&
                (shl.lv.customerViewModel.CountryID === 0 || shl.lv.customerViewModel.CountryID === '0' || shl.lv.customerViewModel.CountryID === undefined)) {

                shl.lv.isRequired = false;
                shl.lv.isAddressFormInvalid = false;
                shl.lv.allFeildVacant = true;
            }

            if ((shl.lv.customerViewModel.Address1 !== null && shl.lv.customerViewModel.Address1 !== undefined && shl.lv.customerViewModel.Address1 !== '') &&
                (shl.lv.customerViewModel.City !== null && shl.lv.customerViewModel.City !== undefined && shl.lv.customerViewModel.City !== '') &&
                (shl.lv.StateName !== null && shl.lv.StateName !== '' && shl.lv.StateName !== undefined) &&
                (shl.lv.customerViewModel.PostalCode !== null && shl.lv.customerViewModel.PostalCode !== undefined && shl.lv.customerViewModel.PostalCode !== '') &&
                (shl.lv.customerViewModel.CountryID !== 0 && shl.lv.customerViewModel.CountryID !== '0' && shl.lv.customerViewModel.CountryID !== undefined)) {

                shl.lv.allFeildFilled = true;
                shl.lv.isRequired = false;
                shl.lv.isAddressFormInvalid = false;
            }
            else {
                if (!shl.lv.allFeildVacant) {
                    shl.lv.isRequired = true;
                    shl.lv.isAddressFormInvalid = true;
                    return;
                }
            }
        }

        function validatePostalCode(countryID) {

            if (countryID === 33) {
                shl.lv.minLength = 7;
                shl.lv.maxLength = 7;
                shl.lv.postalCodePattern = '^([a-zA-Z0-9]{5}|[a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9] [a-zA-Z0-9][a-zA-Z0-9][a-zA-Z0-9])';
            }
            else if (countryID === 184) {
                shl.lv.minLength = 5;
                shl.lv.maxLength = 6;
                shl.lv.postalCodePattern = '^[0-9]*$';
            }
            else {
                shl.lv.minLength = 1;
                shl.lv.maxLength = 20;
                shl.lv.postalCodePattern = '';
            }
        }
        //////////////////////-----------------------------------------////////////////

    }
})();
