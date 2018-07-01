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
        displayName: 'Home',
        meta: { title: 'Home' },
    },
    {
        path: '/browse/:path*',
        component: browse,
        name: 'browse',
        displayName: 'Browse',
        meta: { title: 'Browse' },
    },
    {
        path: '/schemas/:schemaName?',
        component: schemas,
        name: 'schemas',
        displayName: 'Schemas',
        meta: { title: 'Schemas' },
    },
];

const router = new Router({
    mode: 'history',
    routes,
});

router.beforeEach((to, from, next) => {
    // hack to allow for forward slashes in path ids
    if (to.fullPath.includes('%2F')) {
        next(to.fullPath.replace('%2F', '/'));
    }
    next();
});

export default router;
