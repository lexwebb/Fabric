import RestServiceBase from './restServiceBase';


function createServices(vue) {
    return {
        schemas: new RestServiceBase(vue, 'schema'),
        config: new RestServiceBase(vue, 'config'),
    };
}

export default {
    /* eslint-disable no-param-reassign */
    install(Vue) {
        Vue.prototype.$services = createServices(Vue);
    },
};
