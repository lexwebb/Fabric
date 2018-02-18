import RestServiceBase from './restServiceBase';

const services = {
    schemas: new RestServiceBase('schema'),
    config: new RestServiceBase('config'),
};

export default {
    services,
    /* eslint-disable no-param-reassign */
    install(Vue) {
        Vue.prototype.$services = services;
    },
};

export {
    services,
};
