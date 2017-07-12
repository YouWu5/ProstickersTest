(function () {
    'use strict';

    angular
        .module('app.layout')
        .controller('Messages', Messages);

    Messages.$inject = ['message', 'stackView', 'messagesFactory', 'helper'];

    function Messages(message, stackView, messagesFactory, helper) {

        var msg = this;

        function initializeController() {



        }

        initializeController();

        msg.messageExist = function () {

            if (message.getMessage() === null) {
                return false;
            }
            else {
                return true;
            }
        };

        msg.getMessage = function () {
            msg.height = helper.getHeight();
            
            return message.getMessage();
        };

        msg.clearMessage = function () {
            message.clearMessage();
        };

        msg.openViewFromUserMessage = function (item) {

            if (item.action === 'Detail') {
                if (item.listController !== '') {
                    var obj = stackView.getFormObjectOfController(item.listController);
                    if (obj !== 'NotFound') {
                        obj.lv.id = item.returnedData;
                        stackView.openView(item.viewUrl);
                    }
                }

            }
            if (item.action === 'TransactionDetail') {
                stackView.pushObject(item.returnedData);
                stackView.openView(item.viewUrl);
            }
        };

        msg.printFromUserMessage = function (item) {
            message.clearMessage();
            messagesFactory.printDetail(item.returnedData).then(function (data) {
                location.href = data;
            });
        };
    }
})();