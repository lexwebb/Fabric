import Vue from 'vue';
import Router from 'vue-router';

import home from './components/home.vue';
import browse from './components/browse.vue';

Vue.use(Router);

const routes = [
    {
        path: '/',
        component: home,
        meta: { title: 'Home' },
    },
    {
        path: '/browse',
        component: browse,
        meta: { title: 'Browse' },
    },
];

export default new Router({
    routes,
});
