(function () {
    'use strict';

    angular
        .module('app.home')
        .controller('homeController', homeController);

    homeController.$inject = [];

    function homeController() {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.setFooterPaddingNoRecord = null;

        initializeController();

        function initializeController() {
        }

    }
})();
