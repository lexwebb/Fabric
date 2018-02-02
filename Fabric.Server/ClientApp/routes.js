import Vue from 'vue';
import Router from 'vue-router';

import home from './components/home.vue';
import browse from './components/browse.vue';
import schemas from './components/schemas.vue';

Vue.use(Router);

const routes = [
    {
        path: '/',
        component: home,
        name: 'home',
        meta: { title: 'Home' },
    },
    {
        path: '/browse/:path?',
        component: browse,
        name: 'browse',
        meta: { title: 'Browse' },
    },
    {
        path: '/schemas/:schemaName?',
        component: schemas,
        name: 'schemas',
        meta: { title: 'Schemas' },
    },
];

export default new Router({
    routes,
});
