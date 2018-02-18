import { services } from '../services';

/* eslint-disable no-param-reassign */
const browse = {
    namespaced: true,
    state: {
        rootNode: {},
        currentPage: undefined,
    },
    mutations: {
        setRootNode(state, value) {
            state.rootNode = value;
        },
        setCurrentPage(state, value) {
            state.currentPage = undefined;
            state.currentPage = value;
        },
    },
    actions: {
        getRootNode({ commit }) {
            return services.config.get().then((data) => {
                commit('setRootNode', data);
            });
        },
        getPage({ commit }, path) {
            return services.config.get(path.replace(/^(root\/|root)/, '')).then((data) => {
                commit('setCurrentPage', data);
            });
        },
    },
};

export default browse;
