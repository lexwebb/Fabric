<template>
    <div>
        <json-editor v-if="schemaLoaded" :schema="schemaObj" :initial-value="dataObj" @update-value="updateValue($event)" theme="bootstrap3" icon="fontawesome4">
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
    .well.bootstrap3-row-container {
        border: 1px solid #ddd;
        border-radius: 3px;
        padding: 0.5em;
    }
    .control-label {
        display: flex;
        flex-direction: row;
    }
    .help-block {
        border: 1px solid #ddd;
        border-radius: 3px;
        &:before {
            content: 'About:';
            position: absolute;
            margin-top: -16px;
            color: grey;
        }
        padding-top: 15px;
    }
    .row h3 {
        display: none;
    }
</style>