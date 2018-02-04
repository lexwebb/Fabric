<template>
    <div>
        <div class="md-layout">
            <div class="md-layout-item md-size-20 right-border">
                <buttonTitle :title="'Choose a schema'" :icon="'add'" @onClick="onAddSchema()" />
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
                    <buttonTitle :title="`Schema - ${currentSchema.schemaName}`" :icon="'edit'" :showButton="!editMode" @onClick="onEditSchema()" />
                    <transition name="fade">
                        <tree-view v-if="currentSchemaJsonObj" :data="currentSchemaJsonObj"></tree-view>
                    </transition>
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
                    <md-field class="edit-schema-raw-editor" :class="{ error: !schemaValidation.isValid }">
                        <label>Schema</label>
                        <md-textarea v-model="currentSchema.schemaRaw"></md-textarea>
                    </md-field>
                    <span v-if="!this.schemaValidation.isValid" class="error-message">
                        <b>Error parsing schema:</b> {{schemaValidation.validationMessage}}
                    </span>
                    <div class="flex-row-right">
                        <md-button class="md-raised" @click="onEditSchema(false)">Cancel</md-button>
                        <md-button class="md-raised md-primary" :disabled="!schemaValidation.isValid" @click="onSaveSchema">Save</md-button>
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
                schemaValidation: {
                    isValid: true,
                    validationMessage: '',
                },
            };
        },
        computed: {
            currentSchemaJsonObj() {
                let schema;
                try {
                    schema = JSON.parse(this.currentSchema.schemaRaw);
                } catch (e) {
                    return undefined;
                }
                return schema;
            },
            schemaRaw() {
                return this.currentSchema ? this.currentSchema.schemaRaw : undefined;
            },
        },
        mounted() {
            this.onRoute();
            this.lastEditTimeStamp = (new Date()).getTime();
        },
        methods: {
            onRoute(to) {
                const route = to || this.$route;

                this.$services.schemas.get()
                    .then((data) => {
                        this.schemas = data;
                    })
                    .catch(() => {
                        EventBus.$emit('show-error', 'Error loading schemas');
                    });

                if (route.params.schemaName) {
                    this.$services.schemas.get(route.params.schemaName)
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
            onSaveSchema() {
                if (this.schemaValidation.isValid) {
                    this.$services.schemas.put(this.currentSchema.schemaName, this.schemaRaw)
                        .then(() => {
                            EventBus.$emit('show-message', 'Schema saved successfully');
                        })
                        .catch(() => {
                            EventBus.$emit('show-error', 'Error saving schema');
                        });
                }
            },
        },
        watch: {
            $route(to) {
                this.onRoute(to);
            },
            schemaRaw(newVal) {
                let schema;
                try {
                    schema = JSON.parse(newVal);
                } catch (e) {
                    this.schemaValidation.isValid = false;
                    this.schemaValidation.validationMessage = e.message;
                    return;
                }
                try {
                    this.$ajv.new.compile(schema);
                    this.schemaValidation.isValid = true;
                } catch (e) {
                    this.schemaValidation.isValid = false;
                    this.schemaValidation.validationMessage = e.message;
                }
            },
        },
    };
</script>

<style scoped lang="scss">
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
        &.error {
            &::before,
            &::after {
                border-color: #f44336 !important;
            }
        }
    }
</style>
