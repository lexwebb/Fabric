<template>
    <md-tabs>
        <md-tab id="edit-page" md-label="Edit page">
            <jsonEditor v-if="schemaLoaded" :schema="schemaObj" :data="dataObj" :name="model.name" />
        </md-tab>
        <md-tab id="edit-raw" md-label="Edit raw">
            <codemirror :code="jsonData" :options="cmOptions"></codemirror>
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
                schema: {},
                schemaLoaded: false,
                cmOptions: {
                    tabSize: 4,
                    mode: { name: 'javascript', json: true },
                    theme: 'elegant',
                    lineNumbers: true,
                    line: true,
                },
            };
        },
        computed: {
            schemaObj() {
                return JSON.parse(this.schema.schemaRaw);
            },
            dataObj() {
                return JSON.parse(this.model.pageData);
            },
            jsonData() {
                return this.model.pageData;
            },
        },
        mounted() {
            this.getSchema();
        },
        methods: {
            getSchema() {
                if (this.model.schemaName) {
                    this.$services.schemas.get(this.model.schemaName)
                        .then((data) => {
                            this.schema = data;
                            this.schemaLoaded = true;
                        })
                        .catch(() => {
                            EventBus.$emit('show-error', 'Error loading schema');
                        });
                }
            },
            updateValue() {

            },
        },
        watch: {
            model() {
                this.getSchema();
            },
        },
    };
</script>

<style scopred lang="scss">
    .main > h3 {
        display: none;
    }
    .well.bootstrap3-row-container {
        .control-label {
            display: flex;
            flex-direction: row;
        }
        .help-block:not(:empty) {
            margin: 0;
            &:before {
                content: 'Description:';
                position: relative;
                margin-right: 0.5em;
                color: grey;
            }
        }
        .row {
            box-shadow: 0 3px 1px -2px rgba(0, 0, 0, 0.2),
                0 2px 2px 0 rgba(0, 0, 0, 0.14), 0 1px 5px 0 rgba(0, 0, 0, 0.12);
            margin: 0.5em;
            padding: 0.2em;
            .btn-group {
                display: inline-block;
                .btn {
                    border: none;
                    background: none;
                    color: #1c8d7d;
                }
            }
        }
        .checkbox {
            label {
                visibility: hidden;
                font-size: 0;
                width: 18px;
                input {
                    visibility: visible;
                }
            }
            &:after {
                content: 'null';
                color: grey;
                margin-right: 0.5em;
            }
        }
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