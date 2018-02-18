import Vue from 'vue';
import Vuex from 'vuex';

import browse from './stores/browse';

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        browse,
    },
});
