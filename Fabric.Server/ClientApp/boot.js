import './css/site.css';
import 'bootstrap';
import app from './app.vue';
import Vue from 'vue';
import VueRouter from 'vue-router';
import TreeView from 'vue-json-tree-view';
Vue.use(VueRouter);
Vue.use(TreeView);

const routes = [
    {
        path: '/',
        component: require('./components/home.vue'),
        meta: { title: 'Home' }
    },
    {
        path: '/browse',
        component: require('./components/browse.vue'),
        meta: { title: 'Browse' }
    }
];

const router = new VueRouter({ mode: 'history', routes: routes });
router.beforeEach((to, from, next) => {
    document.title = `${to.meta.title} - Fabric.Server`;
    next();
});

// ReSharper disable once ConstructorCallNotUsed
new Vue({
    el: '#app-root',
    router: router,
    template: '<app/>',
    components: { app }
});
