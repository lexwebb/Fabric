<template>
    <md-tabs>
        <md-tab id="edit-page" md-label="Edit page">
            <transition name="fade">
                <jsonEditor v-if="isSchemaLoaded" :schema="schemaObj" :data="dataObj" :name="dataObj.name" @changed="editorChanged" />
            </transition>
        </md-tab>
        <md-tab id="edit-raw" md-label="Edit raw">
            <transition name="fade">
                <codemirror v-if="isSchemaLoaded" :code="dataJson" :options="cmOptions" @input="textEditorChanged" @beforeChange="beforeEditorChange"></codemirror>
            </transition>
        </md-tab>
    </md-tabs>
</template>

<script>
    import { mapState, mapGetters } from 'vuex';
    import jsonEditor from './jsonEditor/jsonEditor.vue';

    export default {
        name: 'pageEditor',
        components: {
            jsonEditor,
        },
        data() {
            return {
                cmOptions: {
                    tabSize: 4,
                    mode: { name: 'javascript', json: true },
                    theme: 'elegant',
                    lineNumbers: true,
                    line: true,
                },
            };
        },
        mounted() {
            this.resize();
        },
        computed: {
            ...mapState('browse', {
                schemaJson: state => state.editor.schemaJson,
                schemaObj: state => state.editor.schemaObj,
                dataJson: state => state.editor.dataJson,
                dataObj: state => state.editor.dataObj,
                rootNode: state => state.editor.rootNode,
                currentEditItem: state => state.editor.currentPage,
            }),
            ...mapGetters('browse', [
                'isEditingCode',
                'isEditingObj',
                'isSchemaLoaded',
            ]),
        },
        methods: {
            editorChanged(data) {
                if (!this.isEditingCode) {
                    this.$store.dispatch('browse/onEditorChanged', data);

                    this.$nextTick(() => {
                        this.$store.dispatch('browse/resetEditingFlags', data);
                    });
                }
            },
            textEditorChanged(data) {
                if (!this.isEditingObj) {
                    this.$store.dispatch('browse/onTextEditorChanged', data);

                    this.$nextTick(() => {
                        this.$store.dispatch('browse/resetEditingFlags', data);
                    });
                }
            },
            beforeEditorChange() {
                // this.resize();
            },
            resize() {
                const tab = document.getElementById('edit-raw');
                const toResize = document.querySelectorAll('.vue-codemirror,.CodeMirror,.CodeMirror-scroll');

                toResize.forEach((element) => {
                    /* eslint-disable no-param-reassign */
                    element.style.maxWidth = `${tab.clientWidth}px`;
                });
            },
        },
    };
</script>

<style scopred lang="scss">
    .main > h3 {
        display: none;
    }
</style>
<style lang=scss>
    .vue-codemirror {
        height: 100%;
        max-width: 1000px;
        .CodeMirror {
            height: 100%;
            max-width: 1000px;
        }
        .CodeMirror-scroll {
            max-width: 1000px;
        }
    }
</style>
