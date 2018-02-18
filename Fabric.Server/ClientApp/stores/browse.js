import { services } from '../services';

/* eslint-disable no-param-reassign */
const browse = {
    namespaced: true,
    state: {
        rootNode: {},
        currentEditItem: undefined,
    },
    mutations: {
        setRootNode(state, value) {
            state.rootNode = value;
        },
        setCurrentItem(state, value) {
            state.currentEditItem = value;
        },
    },
    actions: {
        getRootNode({
            commit,
        }) {
            debugger;
            return services.config.get()
                .then((data) => {
                    commit('setRootNode', data);
                });
        },
    },
};

export default browse;
