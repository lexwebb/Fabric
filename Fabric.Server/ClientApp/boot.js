import 'bootstrap';
import Vue from 'vue';
import VueRouter from 'vue-router';
import TreeView from 'vue-json-tree-view';
import pluralize from 'pluralize';
import moment from 'moment';
import './css/site.css';
import utils from './utils';

import app from './app.vue';
import router from './routes';

Vue.use(VueRouter);
Vue.use(TreeView);
Vue.use(utils);

// Custom imports
Vue.prototype.$pluralize = pluralize;
Vue.prototype.$moment = moment;

router.beforeEach((to, from, next) => {
    document.title = `${to.meta.title} - Fabric.Server`;
    next();
});

// ReSharper disable once ConstructorCallNotUsed
/* eslint-disable no-new */
new Vue({
    el: '#app-root',
    router,
    template: '<app/>',
    components: { app },
});
