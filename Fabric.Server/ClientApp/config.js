const config = {
    apiUrlbase: '/api',
};

export default {
    config,
    /* eslint-disable no-param-reassign */
    install(Vue) {
        Vue.prototype.$config = config;
    },
};

export {
    config,
};
