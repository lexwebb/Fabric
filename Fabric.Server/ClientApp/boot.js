// Regular imports
import Vue from 'vue';
import VueMaterial from 'vue-material';
import 'vue-material/dist/vue-material.min.css';
import 'vue-material/dist/theme/black-green-light.css';
import 'codemirror/lib/codemirror.css';
import 'codemirror/theme/elegant.css';
import 'codemirror/mode/javascript/javascript';
import VueRouter from 'vue-router';
import CodeMirror from 'vue-codemirror';
import treeView from 'vue-json-tree-view';
import pluralize from 'pluralize';
import moment from 'moment';
import './css/site.css';
import ajvPlugin from './ajv-plugin';

// Plugins
import app from './app.vue';
import router from './routes';
import store from './store';

// Custom Plugins
import protoConfig from './config';
import protoServices from './services';
import protoUtils from './utils';

Vue.use(VueRouter);
Vue.use(VueMaterial);
Vue.use(CodeMirror);
Vue.use(treeView);
Vue.use(ajvPlugin);

Vue.use(protoUtils);
Vue.use(protoConfig);
Vue.use(protoServices);

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
    store,
    template: '<app/>',
    components: {
        app,
    },
});
