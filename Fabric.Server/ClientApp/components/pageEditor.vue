<template>
    <div>
        <json-editor class="main" v-if="schemaLoaded" :schema="schemaObj" :initial-value="dataObj" @update-value="updateValue($event)" theme="bootstrap3" icon="fontawesome4">
        </json-editor>
    </div>
</template>

<script>
    // Import the EventBus.
    import { EventBus } from '../event-bus';

    export default {
        name: 'pageEditor',
        props: {
            model: Object,
        },
        data() {
            return {
                schema: {},
                schemaLoaded: false,
            };
        },
        computed: {
            schemaObj() {
                return JSON.parse(this.schema.schemaRaw);
            },
            dataObj() {
                return JSON.parse(this.model.pageData);
            },
        },
        mounted() {
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
        methods: {
            updateValue() {

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
            box-shadow: 0 3px 1px -2px rgba(0,0,0,.2),0 2px 2px 0 rgba(0,0,0,.14),0 1px 5px 0 rgba(0,0,0,.12);
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