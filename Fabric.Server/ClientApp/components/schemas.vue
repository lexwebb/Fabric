<template>
    <div>
        <h1>Schemas</h1>
        <div class="md-layout md-gutter ">
            <div class="md-layout-item md-size-20 right-border">
                <h3>Choose a schema</h3>
                <md-list class="md-double-line">
                    <md-list-item v-for="schema in schemas" @click="onEditSchema(schema.schemaName)" :key="schema.schemaName">
                        <md-avatar>
                            <md-icon>code</md-icon>
                        </md-avatar>
                        <div class="md-list-item-text">
                            <b>{{schema.schemaName}}</b>
                            <span>{{schema.description}}</span>
                        </div>
                    </md-list-item>
                </md-list>
            </div>
            <transition name="fade">
                <div class="md-layout-item" v-if="currentSchema">
                    <div class="schema-title">
                        <h3>Schema - {{currentSchema.schemaName}}</h3>
                        <md-button class="md-icon-button md-dense md-raised md-primary edit-schema-button" @click="editMode = true">
                            <md-icon>edit</md-icon>
                        </md-button>
                    </div>
                    <tree-view :data="currentSchemaJsonObj"></tree-view>
                </div>
            </transition>
            <transition name="fade">
                <div class="md-layout-item edit-schema-raw-container" v-if="currentSchema && editMode">
                    <h3>Edit schema</h3>
                    <md-field>
                        <label>Schema name</label>
                        <md-input v-model="currentSchema.schemaName"></md-input>
                    </md-field>
                    <label>Json</label>
                    <md-field class="edit-schema-raw-editor">
                        <label>Schema</label>
                        <md-textarea v-model="currentSchema.schemaRaw"></md-textarea>
                    </md-field>
                    <div class="flex-row-right">
                        <md-button class="md-raised" @click="onEditSchema(currentSchema.schemaName); editMode = false;">Cancel</md-button>
                        <md-button class="md-raised md-primary" @click="onSaveSchema">Save</md-button>
                    </div>
                </div>
            </transition>
        </div>
    </div>
</template>

<script>
    // Import the EventBus.
    import { EventBus } from '../event-bus';

    export default {
        name: 'schemas',
        data() {
            return {
                schemas: [],
                currentSchema: undefined,
                editMode: false,
            };
        },
        computed: {
            currentSchemaJsonObj() {
                return JSON.parse(this.currentSchema.schemaRaw);
            },
        },
        mounted() {
            fetch('api/schema/')
                .then(response => response.json())
                .then((data) => {
                    this.schemas = data;
                }).catch(() => {
                    EventBus.$emit('show-error', 'Error loading schemas');
                });
        },
        methods: {
            onEditSchema(schemaName) {
                fetch(`api/schema/${schemaName}`)
                    .then(response => response.json())
                    .then((data) => {
                        this.currentSchema = data;
                    }).catch(() => {
                        EventBus.$emit('show-error', 'Error loading schema');
                    });
            },
            onSaveSchema() {

            },
        },
    };
</script>

<style scoped>
    .right-border {
        border-right: 1px solid #ddd;
        min-width: 300px;
    }

    .edit-schema-raw-container {
        display: flex;
        flex-direction: column;
    }

    .edit-schema-raw-editor {
        flex-grow: 1;
    }

        .edit-schema-raw-editor textarea {
            height: 100%;
            overflow-y: auto;
            overflow-x: auto;
            resize: none !important;
            max-height: none !important;
        }

    .schema-title {
        display: flex;
        flex-direction: row;
    }

        .schema-title h3 {
            flex-grow: 1;
        }
</style>
