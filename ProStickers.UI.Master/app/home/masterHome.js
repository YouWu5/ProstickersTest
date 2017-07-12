(function () {
    'use strict';

    angular
        .module('app.home')
        .controller('masterHome', masterHome);

    masterHome.$inject = ['$location', '$state', 'stackView', 'FeedbackListFactory'];

    function masterHome($location, $state, stackView, FeedbackListFactory) {
        /* jshint validthis:true */
        var fo = this;
        fo.vm = {};
        fo.lv = {};
        fo.lv.title = 'Customer Feedbacks';
        fo.lv.setFooterPaddingNoRecord = null;

        initializeController();

        function initializeController() {
            FeedbackListFactory.getCustomerFeedbackList().then(function (data) {
                fo.vm = data;
                console.log('fo.vm @ initialize', fo.vm);
            });
        }

        fo.viewMore = function () {
            $state.go('FeedbackList');
        };

        fo.OpenDetail = function (customerId, designNo) {
            stackView.pushViewDetail({
                controller: 'masterHome',
                formObject: fo, url: '/',
                formName: 'Home'
            });
            $state.go('FeedbackDetail', { ID: customerId, DesignNo: designNo, redirect: true });
        };

    }
})();
