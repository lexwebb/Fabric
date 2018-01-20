import './css/site.css';
import 'bootstrap';
import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use((VueRouter) as any);

const routes = [
    {
        path: '/',
        component: require('./components/home/home.vue.html'),
        meta: { title: 'Home'}
    },
    {
        path: '/browse',
        component: require('./components/browse/browse.vue.html'),
        meta: { title: 'Browse' } 
    }
];

const router = new VueRouter({ mode: 'history', routes: routes });
router.beforeEach((to, from, next) => {
    document.title = `${to.meta.title} - Fabric.Server`;
    next();
});

// ReSharper disable once WrongExpressionStatement
new Vue({
    el: '#app-root',
    router: router,
    render: h => h(require('./components/app/app.vue.html'))
});
