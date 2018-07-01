<template>
    <div class="editor">
        <div class="editor-header">
            <h4>Editing: {{pageName}}</h4>
            <md-button class="md-icon-button md-dense md-raised md-primary add-button" @click="showEditView"">
                <md-icon>list</md-icon>
            </md-button>
            <md-button class="md-icon-button md-dense md-raised md-primary add-button" @click="showRawView">
                <md-icon>code</md-icon>
            </md-button>
        </div>
        <div class="editor-content">
            <transition name="fade">
                <jsonEditor v-if="isSchemaLoaded && !showRaw" :schema="schemaObj" :data="dataObj" :name="dataObj.name" @changed="editorChanged" />
            </transition>
            <transition name="fade">
                <codemirror v-if="isSchemaLoaded && showRaw" :code="dataJson" :options="cmOptions" @input="textEditorChanged" @beforeChange="beforeEditorChange"></codemirror>
            </transition>
        </div>
    </div>
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
                showRaw: false,
            };
        },
        mounted() {
            this.resize();
        },
        computed: {
            ...mapState('browse', {
                pageName: state => state.editor.pageName,
                schemaJson: state => state.editor.schemaJson,
                schemaObj: state => state.editor.schemaObj,
                dataJson: state => state.editor.dataJson,
                dataObj: state => state.editor.dataObj,
                rootNode: state => state.editor.rootNode,
            }),
            ...mapGetters('browse', [
                'isEditingCode',
                'isEditingObj',
                'isSchemaLoaded',
            ]),
        },
        methods: {
            showRawView() {
                this.showRaw = true;
            },
            showEditView() {
                this.showRaw = false;
            },
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

<style scoped lang="scss">
    .main > h3 {
        display: none;
    }
    .editor {
        border: 1px solid #ddd;
        border-radius: 3px;
        .editor-header {
            display: flex;
            align-items: center;
            padding: 5px 10px;
            background-color: #fafbfc;
            border-bottom: 1px solid #e1e4e8;
            border-top-left-radius: 2px;
            border-top-right-radius: 2px;
            h4 {
                flex: 1;
            }
        }
    }
    .editor-content {
        margin: 0.5em;
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
