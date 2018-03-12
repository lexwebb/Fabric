import { services } from '../services';

/* eslint-disable no-param-reassign */
const browse = {
    namespaced: true,
    state: {
        rootNode: {},
        currentPage: undefined,
        currentPath: undefined,
        editor: {
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
        loadEditor({ commit, state }) {
            commit('setEditorSchemaLoaded', false);
            return services.schemas.get(state.currentPage.schemaName).then((data) => {
                commit('setEditorSchamaJson', data.schemaRaw);
                commit('setEditorSchamaObj', JSON.parse(data.schemaRaw));
                commit('setEditorDataJson', state.currentPage.pageData);
                commit('setEditorDataObj', JSON.parse(state.currentPage.pageData));
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
