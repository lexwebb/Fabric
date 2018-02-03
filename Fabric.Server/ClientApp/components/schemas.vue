<template>
    <div>
        <h1>Schemas</h1>
        <div class="md-layout md-gutter ">
            <div class="md-layout-item md-size-20 right-border">
                <buttonTitle :title="'Choose a schema'" :icon="'add'" @onClick="onAddSchema()"/>
                <md-list class="md-double-line">
                    <md-list-item v-for="schema in schemas" @click="onOpenSchema(schema.schemaName)" :key="schema.schemaName">
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
                    <buttonTitle :title="`Schema - ${currentSchema.schemaName}`" :icon="'edit'" @onClick="onEditSchema()"/>
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
                        <md-button class="md-raised" @click="onEditSchema(false)">Cancel</md-button>
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
    import buttonTitle from './buttonTitle.vue';

    export default {
        name: 'schemas',
        components: {
            buttonTitle,
        },
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
            this.onRoute();
        },
        methods: {
            onRoute(to) {
                const route = to || this.$route;

                fetch('api/schema/')
                    .then(response => response.json())
                    .then((data) => {
                        this.schemas = data;
                    })
                    .catch(() => {
                        EventBus.$emit('show-error', 'Error loading schemas');
                    });

                if (route.params.schemaName) {
                    fetch(`api/schema/${route.params.schemaName}`)
                        .then(response => response.json())
                        .then((data) => {
                            this.currentSchema = data;
                        })
                        .catch(() => {
                            EventBus.$emit('show-error', 'Error loading schema');
                        });
                }

                if (route.query.edit) {
                    this.editMode = route.query.edit;
                } else {
                    this.editMode = false;
                }
            },
            onOpenSchema(schemaName) {
                this.$router.push({ name: 'schemas', params: { schemaName } });
            },
            onEditSchema(edit) {
                if (edit === false) {
                    this.$router.push({ name: 'schemas', params: { schemaName: this.currentSchema.schemaName } });
                } else {
                    this.$router.push({ name: 'schemas', params: { schemaName: this.currentSchema.schemaName }, query: { edit: true } });
                }
            },
            onSaveSchema() { },
        },
        watch: {
            $route(to) {
                this.onRoute(to);
            },
        },
    };
</script>

<style scoped lang=scss>
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
        textarea {
            height: 100%;
            overflow-y: auto;
            overflow-x: auto;
            resize: none !important;
            max-height: none !important;
        }
    }
</style>
