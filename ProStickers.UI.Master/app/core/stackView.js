(function () {
    'use strict';

    angular
        .module('app.core')
        .factory('stackView', stackView);

    stackView.$inject = ['$location', '$state'];

    function stackView($location, $state) {

        var viewsStack = [{
            controller: '',
            formObject: '',
            url: '/',
            formName: 'Home',
            templateUrl: '/app/home/home.html'
        }];

        var service = {
            viewsStack: viewsStack,
            pushViewDetail: pushObject,
            getLastFormObject: getLastFormObject,
            popFormObject: popFormObject,
            getLastViewDetail: getLastObject,
            getViewDetailAtIndex: getObjectAtIndex,
            removeViewDetailAtIndex: removeObjectAtIndex,
            getLastViewUrl: getLastViewUrl,
            getFormObjectOfController: getFormObjectOfController,
            getViewDetailOfController: getObjectOfController,
            isLastView: isLastView,
            discardViewDetail: discardObject,
            clearViewsStack: clearViewsStack,
            openView: openView,
            closeView: closeView,
            getViewStack: getViewStack,
            setViewStack: setViewStack,
            drillDownToView: drillDownToView,
            openParentView: openParentView,
            closeThisView: closeThisView,
            pushObject: pushObject,
            openHome: openHome
        };

        return service;

        function pushObject(obj) {
            if (viewsStack === null || viewsStack === '' || viewsStack === undefined) {
                viewsStack = [];
            }
            viewsStack.push({
                controller: obj.controller,
                formObject: obj.formObject,
                url: obj.url,
                formName: obj.formName,
                templateUrl: obj.templateUrl
            });
        }

        function getLastFormObject() {
            if (viewsStack.length > 0) {
                var obj = viewsStack[(viewsStack.length - 1)].formObject;
                return obj;
            }
            else {
                return null;
            }
        }

        function popFormObject() {
            if (viewsStack.length > 0) {

                var obj = viewsStack[(viewsStack.length - 1)].formObject;
                viewsStack.splice((viewsStack.length - 1), 1);

                return obj;
            }
            else {
                return null;
            }
        }

        function getLastObject() {
            if (viewsStack !== null && viewsStack.length > 0) {
                var obj = viewsStack[(viewsStack.length - 1)];
                return obj;
            }
            else {
                return viewsStack;
            }
        }

        function getObjectAtIndex(index) {
            return viewsStack[index];
        }

        function removeObjectAtIndex(index) {
            viewsStack.splice(index, 1);
        }

        function getLastViewUrl() {
            if (viewsStack.length > 0) {
                return viewsStack[viewsStack.length - 1].url;
            }
            else {
                return '#/1';
            }
        }

        function getFormObjectOfController(controller) {

            var index = viewsStack.map(function (x) { return x.controller; }).indexOf(controller);

            if (index !== -1) {
                return viewsStack[index].formObject;
            }
            else {
                return 'NotFound';
            }
        }

        function getObjectOfController(controller) {

            var index = viewsStack.map(function (x) { return x.controller; }).indexOf(controller);
            if (index !== -1) {
                return viewsStack[index];
            }
            else {
                return 'NotFound';
            }
        }

        function isLastView(controller) {
            var obj = getLastObject();
            if (obj !== null) {
                if (obj.controller === controller) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }

        function discardObject() {

            if (viewsStack.length > 0) {
                viewsStack.splice((viewsStack.length - 1), 1);
            }
        }

        function clearViewsStack() {
            viewsStack = [];
        }

        function openView(url) {
            $state.go(url, { redirect: true });
        }

        function closeView() {
            var url = getLastViewUrl();
            //var obj = viewsStack[(viewsStack.length - 1)].formObject
            //viewsStack.splice((viewsStack.length - 1), 1);

            $state.go(url, { redirect: true });
        }

        function getViewStack() {
            return viewsStack;
        }

        function setViewStack(stack) {
            viewsStack = stack;
        }

        function drillDownToView(url, formObject) {
            pushObject(formObject);
            $location.path(url);
        }

        function closeThisView() {
            var url = getLastViewUrl();

            viewsStack.splice((viewsStack.length - 1), 1);
            $state.go(url, { redirect: true });
        }

        function openParentView() {
            var url = getLastViewUrl();
            $location.path(url);
        }

        function openHome() {
            while (viewsStack.length > 1) {
                viewsStack.splice((viewsStack.length - 1), 1);
            }
        }
    }
})();