import 'bootstrap';
import Vue from 'vue';
import VueRouter from 'vue-router';
import TreeView from 'vue-json-tree-view';
import './css/site.css';

import app from './app.vue';
import router from './routes';

Vue.use(VueRouter);
Vue.use(TreeView);

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
