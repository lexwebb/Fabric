import { services } from '../services';

/* eslint-disable no-param-reassign */
const browse = {
    namespaced: true,
    state: {
        rootNode: {},
        currentPage: undefined,
        currentPath: undefined,
        editor: {
            pageName: '',
            schemaJson: '',
            schemaObj: {},
            dataJson: '',
            dataObj: {},
            schemaLoaded: false,
            editingCode: false,
            editingObj: false,
        },
    },
    getters: {
        isEditingCode: state => state.editor.editingCode,
        isEditingObj: state => state.editor.editingObj,
        isSchemaLoaded: state => state.editor.schemaLoaded,
    },
    mutations: {
        setRootNode(state, value) {
            state.rootNode = value;
        },
        setCurrentPage(state, value) {
            state.currentPage = undefined;
            state.currentPage = value;
        },
        setCurrentPath(state, value) {
            state.currentPath = value;
        },
        setEditorPageName(state, value) {
            state.editor.pageName = value;
        },
        setEditorSchamaJson(state, value) {
            state.editor.schemaJson = value;
        },
        setEditorSchamaObj(state, value) {
            state.editor.schemaObj = value;
        },
        setEditorDataJson(state, value) {
            state.editor.dataJson = value;
        },
        setEditorDataObj(state, value) {
            state.editor.dataObj = value;
        },
        setEditorSchemaLoaded(state, value) {
            state.editor.schemaLoaded = value;
        },
        setEditorEditingCode(state, value) {
            state.editor.editingCode = value;
        },
        setEditorEditingObj(state, value) {
            state.editor.editingObj = value;
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
                commit('setCurrentPath', path);
            });
        },
        deletePage({ commit }, path) {
            return services.config.delete(path.replace(/^(root\/|root)/, '')).then(() =>
                services.config.get().then((data) => {
                    commit('setRootNode', data);
                }));
        },
        loadEditor({ commit, state }) {
            commit('setEditorSchemaLoaded', false);
            return services.schemas.get(state.currentPage.schemaName).then((data) => {
                commit('setEditorPageName', state.currentPage.name);
                commit('setEditorSchamaJson', data.schemaRaw);
                commit('setEditorSchamaObj', JSON.parse(data.schemaRaw));
                commit('setEditorDataJson', JSON.stringify(state.currentPage.pageData, null, 2));
                commit('setEditorDataObj', state.currentPage.pageData);
                commit('setEditorSchemaLoaded', true);
            });
        },
        onEditorChanged({ commit, state }, data) {
            if (!state.editor.editingCode) {
                commit('setEditorEditingCode', false);
                commit('setEditorEditingObj', true);

                commit('setEditorDataJson', JSON.stringify(data, undefined, 4));
            }
        },
        onTextEditorChanged({ commit, state }, data) {
            if (!state.editor.editingObj) {
                commit('setEditorEditingCode', true);
                commit('setEditorEditingObj', false);

                commit('setEditorDataObj', JSON.parse(data));
            }
        },
        resetEditingFlags({ commit }) {
            commit('setEditorEditingCode', false);
            commit('setEditorEditingObj', false);
        },
        save({ dispatch, state }) {
            return services.config
                .post(state.currentPath.replace(/^(root\/|root)/, ''), state.editor.dataJson)
                .then(() => {
                    dispatch('browse/loadEditor');
                });
        },
    },
};

export default browse;
