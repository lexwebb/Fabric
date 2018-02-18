<template>
    <md-tabs>
        <md-tab id="edit-page" md-label="Edit page">
            <transition name="fade">
                <jsonEditor v-if="schemaLoaded" :schema="schemaObj" :data="dataObj" :name="model.name" @changed="editorChanged" />
            </transition>
        </md-tab>
        <md-tab id="edit-raw" md-label="Edit raw">
            <transition name="fade">
                <codemirror v-if="schemaLoaded" :code="dataJson" :options="cmOptions" @input="textEditorChanged"></codemirror>
            </transition>
        </md-tab>
    </md-tabs>
</template>

<script>
    import jsonEditor from './jsonEditor/jsonEditor.vue';

    // Import the EventBus.
    import { EventBus } from '../event-bus';

    export default {
        name: 'pageEditor',
        components: {
            jsonEditor,
        },
        props: {
            model: Object,
        },
        data() {
            return {
                schemaJson: '',
                schemaObj: {},
                dataJson: '',
                dataObj: {},
                schemaLoaded: false,
                cmOptions: {
                    tabSize: 4,
                    mode: { name: 'javascript', json: true },
                    theme: 'elegant',
                    lineNumbers: true,
                    line: true,
                },
                editingCode: false,
                editingObj: false,
            };
        },
        mounted() {
            this.getSchema();
        },
        methods: {
            getSchema() {
                if (this.model.schemaName) {
                    this.$services.schemas.get(this.model.schemaName)
                        .then((data) => {
                            this.schemaJson = data.schemaRaw;
                            this.schemaObj = JSON.parse(data.schemaRaw);
                            this.dataJson = this.model.pageData;
                            this.dataObj = JSON.parse(this.model.pageData);
                            this.schemaLoaded = true;
                        })
                        .catch(() => {
                            EventBus.$emit('show-error', 'Error loading schema');
                        });
                }
            },
            editorChanged(data) {
                if (!this.editingCode) {
                    this.editingObj = true;
                    this.editingCode = false;

                    this.dataJson = JSON.stringify(data, undefined, 4);

                    this.$nextTick(() => {
                        this.editingObj = false;
                        this.editingCode = false;
                    });
                }
            },
            textEditorChanged(data) {
                if (!this.editingObj) {
                    this.editingObj = false;
                    this.editingCode = true;

                    this.dataObj = JSON.parse(data);

                    this.$nextTick(() => {
                        this.editingObj = false;
                        this.editingCode = false;
                    });
                }
            },
        },
        watch: {
            model() {
                this.schemaLoaded = false;
                this.getSchema();
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
        .CodeMirror {
            height: 100%;
        }
    }
</style>
