(function () {
    'use strict';

    angular
        .module('app.order')
        .controller('OrdersCreate', OrdersCreate);

    OrdersCreate.$inject = ['helper', '$location', '$state', 'message', 'stackView', '$scope', '$timeout', '$ngBootbox', 'initialDataOfCreateOrder', 'orderCreateFactory'];

    function OrdersCreate(helper, $location, $state, message, stackView, $scope, $timeout, $ngBootbox, initialDataOfCreateOrder, orderCreateFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Order';
        fo.lv.sizeList = [];
        fo.lv.stateList = [];
        fo.lv.showAddressDetails = false;
        fo.lv.isNextEnable = true;
        fo.lv.isPayEnable = false;
        fo.lv.isPaymentDetailEnable = false;
        fo.lv.isWidthRequired = true;
        fo.lv.isHeightRequired = true;
        fo.lv.totalSelectedColors = 0;
        fo.lv.stateNameTextBox = false;
        fo.lv.isRequired = false;
        fo.lv.vectorFileAmount = 30;
        fo.lv.sizeSelection = false;
        fo.lv.purchaseAmount = 0;
        fo.lv.shippingPrice = 4;
        fo.lv.isAddressFormInvalid = false;
        fo.lv.minLength = 0;
        fo.lv.maxLength = 0;
        fo.lv.selectedColorsList = [];
        fo.lv.Purchasedesign = true;
        
        function initializeController() {
            fo.vm = initialDataOfCreateOrder.viewModel;
            fo.vm.PurchaseDesignImage = true;
            fo.vm.CVV = '';
            fo.lv.countryList = [{ Name: 'United States', ID: 184 }];
            fo.lv.yearList = initialDataOfCreateOrder.yearList;
            fo.lv.monthList = initialDataOfCreateOrder.monthList;
            fo.lv.quantityList = [{ 'Text': 1, 'Value': 1 },
            { 'Text': 2, 'Value': 2 },
            { 'Text': 3, 'Value': 3 },
            { 'Text': 4, 'Value': 4 },
            { 'Text': 5, 'Value': 5 },
            { 'Text': 6, 'Value': 6 },
            { 'Text': 7, 'Value': 7 },
            { 'Text': 8, 'Value': 8 },
            { 'Text': 9, 'Value': 9 },
            { 'Text': 10, 'Value': 10 }
            ];

            fo.vm.Quantity = fo.lv.quantityList[0].Text;

            for (var l = 0; l <= 4; l++) {
                fo.lv.selectedColorsList.push({
                    ColorSequence: 0,
                    Name: null,
                    IsSelected: false,
                    ImageURL: null,
                });
            }

            if ((fo.vm.CountryID === 184 || fo.vm.CountryID === 33)) {
                fo.lv.stateNameTextBox = false;
                orderCreateFactory.getStateList(fo.vm.CountryID).then(function (data) {
                    fo.lv.stateList = data;
                });
            }
            else {
                fo.lv.stateNameTextBox = true;

            }
            validatePostalCode(fo.vm.CountryID);
            fo.lv.StateName = fo.vm.StateName;
            fo.lv.address = (fo.vm.Address1 ? fo.vm.Address1 + ', ' : '') +
                (fo.vm.Address2 ? fo.vm.Address2 + ', ' : '') +
                (fo.vm.City ? fo.vm.City + ', ' : '') +
                (fo.lv.StateName ? fo.lv.StateName + ', ' : '') +
                (fo.vm.PostalCode ? fo.vm.PostalCode + ', ' : '') +
                (fo.vm.CountryName ? fo.vm.CountryName : '');

            console.log('initial fo.vm', fo.vm);

            if (fo.vm.DesignImageBuffer) {
                if (fo.vm.DesignImageBuffer !== null && fo.vm.DesignImageBuffer !== ' ') {
                    fo.lv.uploadImage = 'data:image/png;base64,' + fo.vm.DesignImageBuffer.toString();
                }
            }
        }

        initializeController();

        fo.getStateList = function (id) {
            fo.vm.PostalCode = null;
            validatePostalCode(id);

            if (id === null || id === undefined || id === '') {
                fo.lv.isRequired = true;
                fo.lv.stateList = [];
                fo.vm.CountryID = 0;
                fo.lv.StateName = null;
                fo.vm.StateID = 0;
            }
            else {
                fo.lv.isRequired = false;

            }

            if (id === 184 || id === 33) {
                fo.lv.stateNameTextBox = false;
                orderCreateFactory.getStateList(id).then(function (data) {
                    console.log('state list ', data);
                    fo.lv.stateList = data;
                });

            }
            else {
                fo.lv.StateName = null;
                fo.vm.StateID = 0;
                fo.lv.stateNameTextBox = true;
            }
        };

        fo.selectedState = function () {
            for (var p1 = 0; p1 < fo.lv.stateList.length; p1++) {
                if (fo.vm.StateID === fo.lv.stateList[p1].Value) {
                    fo.lv.StateName = fo.lv.stateList[p1].Text;
                }
            }
        };

        function validateAddress() {
            fo.lv.allFeildVacant = false;
            fo.lv.allFeildFilled = false;

            if (fo.vm.CountryID && fo.vm.CountryID !== 184 && fo.vm.CountryID !== 33) {
                fo.vm.StateName = fo.lv.StateName;
            }
            if (fo.vm.CountryID) {
                for (var p = 0; p < fo.lv.countryList.length; p++) {
                    if (fo.vm.CountryID === fo.lv.countryList[p].ID) {
                        fo.vm.CountryName = fo.lv.countryList[p].Name;
                    }
                }
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
                fo.lv.showAddressDetails = false;

            }

            if ((fo.vm.Address1 !== null && fo.vm.Address1 !== undefined && fo.vm.Address1 !== '') &&
                (fo.vm.City !== null && fo.vm.City !== undefined && fo.vm.City !== '') &&
                (fo.lv.StateName !== null && fo.lv.StateName !== '' && fo.lv.StateName !== undefined) &&
                (fo.vm.PostalCode !== null && fo.vm.PostalCode !== undefined && fo.vm.PostalCode !== '') &&
                (fo.vm.CountryID !== 0 && fo.vm.CountryID !== '0' && fo.vm.CountryID !== undefined)) {

                fo.lv.allFeildFilled = true;
                fo.lv.isRequired = false;
                fo.lv.isAddressFormInvalid = false;
                fo.lv.showAddressDetails = false;
            }
            else {
                if (!fo.lv.allFeildVacant) {
                    fo.lv.isRequired = true;
                    fo.lv.isAddressFormInvalid = true;
                    fo.lv.showAddressDetails = true;
                }
            }
        }

        fo.addAddress = function () {
            validateAddress();
            if (fo.lv.allFeildVacant || fo.lv.allFeildFilled) {
                fo.lv.address = (fo.vm.Address1 ? fo.vm.Address1 + ', ' : '') +
                    (fo.vm.Address2 ? fo.vm.Address2 + ', ' : '') +
                    (fo.vm.City ? fo.vm.City + ', ' : '') +
                    (fo.lv.StateName ? fo.lv.StateName + ', ' : '') +
                    (fo.vm.PostalCode ? fo.vm.PostalCode + ', ' : '') +
                    (fo.vm.CountryName ? fo.vm.CountryName : '');
            }

        };

        fo.cancel = function () {
            var options = {
                message: 'Do you want to close the form?',
                buttons: {
                    success: {
                        label: ' ',
                        className: 'fa fa-check-page',
                        callback: function () {
                            $timeout(function () {
                            }, 100);
                            stackView.closeView();
                        }
                    }
                }
            };
            if ($scope.CreateOrderForm.$dirty) {
                $ngBootbox.customDialog(options);
            }
            else {
                stackView.closeView();
            }
        };

        fo.editAddress = function () {
            fo.lv.showAddressDetails = true;
        };

        fo.next = function () {
            fo.lv.isNextEnable = false;
            fo.lv.isPaymentDetailEnable = true;
            fo.lv.isPayEnable = true;
            window.scrollBy(0, screen.width);
        };

        fo.pay = function () {
            console.log('on releae', fo.vm);
            if ($scope.CreateOrderForm.$invalid) {
                fo.lv.isFormInvalid = true;
                return;
            }
            if (fo.lv.totalSelectedColors === 0) {
                message.showClientSideErrors('Please select atlease one color.');
                return;
            }
            console.log('fo.vm.PurchaseDesignImage', fo.vm.PurchaseDesignImage);
            console.log('fo.vm.PurchaseVectorFile', fo.vm.PurchaseVectorFile);
            if (fo.vm.PurchaseDesignImage === false && fo.vm.PurchaseVectorFile === false) {
                message.showClientSideErrors('Total amount should be greater than $0.00.');
                return;
            }
            else {
                fo.lv.isNextEnable = false;
                fo.vm.OrderDate = helper.ConvertDateCST(new Date());
                fo.vm.OrderDate = helper.formatDate(fo.vm.OrderDate);
                fo.vm.ShippingPrice = 4.00;

                for (var m = 0; m < fo.lv.selectedColorsList.length; m++) {
                    for (var n = 0; n < fo.vm.ColorList.length; n++) {
                        if (fo.vm.ColorList[n].IsSelected) {
                            if (fo.lv.selectedColorsList[m].Name === fo.vm.ColorList[n].Name) {
                                fo.vm.ColorList[n].ColorSequence = fo.lv.selectedColorsList[m].ColorSequence;
                            }
                        }
                        else {
                            fo.vm.ColorList[n].ColorSequence = 0;
                        }
                    }
                }

                console.log('on submit', fo.vm);
                orderCreateFactory.submit(fo.vm).then(function (data) {
                    fo.lv.isPaymentDetailEnable = false;
                    console.log('get success response', data);
                    if (data.Result === 1)          // Success
                    {
                        fo.lv.isPayEnable = false;
                        message.showServerSideMessage(data, true);
                        $scope.CreateOrderForm.$setPristine();
                        $state.go('OrdersList', { redirect: true });
                    }
                    helper.setIsSubmitted(false);
                });

            }

        };

        fo.Purchasedesign = function () {
            console.log('fo.lv.Purchasedesign', fo.lv.Purchasedesign);
            calculatePrice();
            fo.lv.purchaseAmount = fo.lv.purchaseAmount * fo.vm.Quantity;
            if (fo.lv.Purchasedesign) {
                fo.vm.PurchaseDesignImage = true;
                fo.vm.Amount = fo.lv.purchaseAmount + fo.lv.vectorFileAmount;
            }
            else {
                fo.vm.PurchaseDesignImage = false;
                fo.vm.Amount = fo.lv.vectorFileAmount;
            }
           
            if (fo.vm.Amount > 0) {
                fo.vm.Amount = fo.vm.Amount + 4;
            }
        };

        fo.PurchaseVectorFile = function () {
            calculatePrice();
            fo.lv.purchaseAmount = fo.lv.purchaseAmount * fo.vm.Quantity;
            console.log('fo.vm.PurchaseVectorFile', fo.vm.PurchaseVectorFile);
            if (fo.vm.PurchaseVectorFile) {

                if (fo.lv.Purchasedesign) {
                    fo.vm.Amount = fo.lv.purchaseAmount + fo.lv.vectorFileAmount;
                }
                else {
                    fo.vm.Amount = fo.lv.vectorFileAmount;
                }
            }
            else {
                if (fo.lv.Purchasedesign) {
                    fo.vm.Amount = fo.lv.purchaseAmount;
                }
                else {
                    fo.vm.Amount = fo.lv.vectorFileAmount;
                }
            }
            
            if (fo.vm.Amount > 0) {
                fo.vm.Amount = fo.vm.Amount + 4;
            }
        };

        fo.selectedSize = function (name) {
            if (name === 'height') {
                fo.vm.Length = fo.lv.designHeight;
                fo.vm.Width = 0;
                fo.lv.isWidthRequired = false;
                fo.lv.isHeightRequired = true;
                if (fo.lv.designHeight) {
                    fo.lv.designWidth = (fo.lv.designHeight * fo.vm.AspectRatio).toFixed(2);
                }
                else {
                    fo.lv.designWidth = 0;
                }

            }
            if (name === 'width') {
                fo.vm.Length = 0;
                fo.vm.Width = fo.lv.designWidth;
                fo.lv.isWidthRequired = true;
                fo.lv.isHeightRequired = false;
                if (fo.lv.designWidth) {
                    fo.lv.designHeight = (fo.lv.designWidth / fo.vm.AspectRatio).toFixed(2);
                }
                else {
                    fo.lv.designHeight = 0;
                }
            }
            calculatePrice();
            fo.lv.purchaseAmount = fo.lv.purchaseAmount * fo.vm.Quantity;
            if (fo.lv.Purchasedesign) {
                fo.vm.Amount = fo.lv.purchaseAmount + fo.lv.vectorFileAmount;
            }
            else {
                fo.vm.Amount = fo.lv.vectorFileAmount;
            }

            console.log('fo.vm.Quantity', fo.vm.Quantity);
            
            if (fo.vm.Amount > 0) {
                fo.vm.Amount = fo.vm.Amount + 4;
            }

        };

        fo.selectedItem = function (item) {
            fo.lv.totalSelectedColors = 0;

            for (var l1 = 0; l1 < fo.vm.ColorList.length; l1++) {
                if (fo.vm.ColorList[l1].IsSelected) {
                    fo.lv.totalSelectedColors = fo.lv.totalSelectedColors + 1;
                }
            }
            if (fo.lv.totalSelectedColors > 5) {
                item.IsSelected = false;
                return;
            }
            else {

                if (item.IsSelected) {
                    var countNames = 0;
                    for (var i2 = 0; i2 < fo.lv.selectedColorsList.length; i2++) {
                        if (fo.lv.selectedColorsList[i2].Name !== null) {
                            countNames++;
                        }
                    }
                    if (countNames > 0) {
                        for (var i3 = 0; i3 < fo.lv.selectedColorsList.length; i3++) {
                            if (fo.lv.selectedColorsList[i3].Name === null) {

                                fo.lv.selectedColorsList[i3].Name = item.Name;
                                fo.lv.selectedColorsList[i3].IsSelected = item.IsSelected;
                                fo.lv.selectedColorsList[i3].ImageURL = item.ImageURL;
                                fo.lv.selectedColorsList[i3].ColorSequence = i3 + 1;
                                break;
                            }
                        }
                    }
                    else {
                        fo.lv.selectedColorsList[0].Name = item.Name;
                        fo.lv.selectedColorsList[0].ColorSequence = 1;
                        fo.lv.selectedColorsList[0].IsSelected = item.IsSelected;
                        fo.lv.selectedColorsList[0].ImageURL = item.ImageURL;
                    }
                }

                if (item.IsSelected === false) {
                    for (var i5 = 0; i5 < fo.lv.selectedColorsList.length; i5++) {
                        if (fo.lv.selectedColorsList[i5].Name === item.Name) {
                            fo.lv.selectedColorsList[i5].Name = null;
                            fo.lv.selectedColorsList[i5].IsSelected = false;
                            fo.lv.selectedColorsList[i5].ImageURL = null;
                            fo.lv.selectedColorsList[i5].ColorSequence = 0;
                        }
                    }
                }

                //fo.lv.selectedColorsList = [];
                //for (var l = 0; l < fo.vm.ColorList.length; l++) {
                //    if (fo.vm.ColorList[l].IsSelected) {
                //        fo.lv.selectedColorsList.push({
                //            Name: fo.vm.ColorList[l].Name,
                //            IsSelected: fo.vm.ColorList[l].IsSelected,
                //            ImageURL: fo.vm.ColorList[l].ImageURL
                //        });
                //    }
                //}

                calculatePrice();
                fo.lv.purchaseAmount = fo.lv.purchaseAmount * fo.vm.Quantity;
                if (fo.lv.Purchasedesign) {
                    fo.vm.Amount = fo.lv.purchaseAmount + fo.lv.vectorFileAmount;
                }
                else {
                    fo.vm.Amount = fo.lv.vectorFileAmount;
                }
                
                if (fo.vm.Amount > 0) {
                    fo.vm.Amount = fo.vm.Amount + 4;
                }
            }
            console.log('fo.lv.selectedColorsList', fo.lv.selectedColorsList);
        };

        function calculatePrice() {

            fo.lv.vectorFileAmount = 30;
            var maxValue = 0;
            if (parseFloat(fo.lv.designHeight) > parseFloat(fo.lv.designWidth)) {
                maxValue = fo.lv.designHeight;
            }
            else {
                maxValue = fo.lv.designWidth;
            }
            console.log('maxValue', maxValue);
            if (fo.lv.totalSelectedColors > 0) {
                if (fo.lv.totalSelectedColors === 1) {
                    fo.lv.purchaseAmount = maxValue * fo.vm.OneColorPrice;
                }
                else if (fo.lv.totalSelectedColors === 2) {
                    fo.lv.purchaseAmount = maxValue * fo.vm.TwoColorPrice;
                }
                else if (fo.lv.totalSelectedColors >= 3) {
                    fo.lv.purchaseAmount = maxValue * fo.vm.MoreColorPrice;
                }
            }
            else {
                fo.lv.purchaseAmount = 0;
                fo.vm.Amount = 0;
            }

            if (fo.vm.PurchaseVectorFile) {
                fo.lv.vectorFileAmount = 30;
            }
            else {
                fo.lv.vectorFileAmount = 0;
            }

            console.log('price after calculation', fo.lv.purchaseAmount);
        }

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
