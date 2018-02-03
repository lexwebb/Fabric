import Vue from 'vue';
import VueMaterial from 'vue-material';
import 'vue-material/dist/vue-material.min.css';
import 'vue-material/dist/theme/black-green-light.css';
import VueRouter from 'vue-router';
import treeView from 'vue-json-tree-view';
import pluralize from 'pluralize';
import moment from 'moment';
import './css/site.css';
import utils from './utils';

import app from './app.vue';
import router from './routes';

Vue.use(VueRouter);
Vue.use(VueMaterial);
Vue.use(treeView);
Vue.use(utils);

// Custom imports
Vue.prototype.$pluralize = pluralize;
Vue.prototype.$moment = moment;

router.beforeEach((to, from, next) => {
    /* eslint-disable no-undef */
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
