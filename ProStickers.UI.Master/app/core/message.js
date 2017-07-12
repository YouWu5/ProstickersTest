(function () {
    'use strict';

    angular
        .module('app.core')
        .factory('message', message);

    message.$inject = [];

    function message() {

        var lvMessage = null;
        var isShowRedirect = false;
        var service = {
            //message: message,
            showServerSideMessage: showServerSideMessage,
            showClientSideErrors: showClientSideErrors,
            showClientSideMessage: showClientSideMessage,
            clearMessage: clearMessage,
            getMessage: getMessage
        };

        return service;

        function showServerSideMessage(msg, RedirectValue) {
            isShowRedirect = RedirectValue;
            if (msg !== null) {
                if (msg.Result === 1) {
                    lvMessage = {
                        Result: msg.Result,
                        Message: msg.Message,
                        ReturnedData: msg.ReturnedData,
                        Key: msg.Key,
                        SourceName: msg.SourceName,
                        Class: 'successMsg',
                        ActionList: null
                    };

                }

                if (msg.Result === 2 || msg.Result === 3 || msg.Result === 5) {

                    lvMessage = {
                        Result: msg.Result,
                        Message: msg.Message,
                        ReturnedData: msg.ReturnedData,
                        Key: msg.Key,
                        SourceName: msg.SourceName,
                        Class: 'errorMsg',
                        ActionList: null
                    };
                }

                if (msg.Result === 4) {
                    lvMessage = {
                        Result: msg.Result,
                        Message: msg.Message,
                        ReturnedData: msg.ReturnedData,
                        Key: msg.Key,
                        SourceName: msg.SourceName,
                        Class: 'notificationMsg',
                        ActionList: null
                    };
                }
            }
        }
        function showClientSideErrors(msg) {
            lvMessage = {
                Result: 2,
                Message: msg,
                ReturnedData: null,
                Key: null,
                SourceName: null,
                Class: 'errorMsg',
                ActionList: null
            };
        }

        function showClientSideMessage(msg) {
            lvMessage = {
                Result: msg.Result,
                Message: msg.Message,
                ReturnedData: msg.ReturnedData,
                Key: msg.Key,
                SourceName: msg.SourceName,

                ActionList: null
            };
        }

        function clearMessage() {
            if (isShowRedirect !== true) {
                lvMessage = null;
            }
            else {
                isShowRedirect = false;
            }
        }

        function getMessage() {
            return lvMessage;
        }

    }
})();