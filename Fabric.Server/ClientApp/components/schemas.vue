<template>
    <div>
        <div class="md-layout-item md-size-30 right-border">
            <h3>Schemas</h3>
            <md-list class="md-double-line">
                <md-list-item v-for="schema in schemas" @click="onEditSchema(schema.schemaName)">
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
        <div class="md-layout-item" v-if="currentSchema">
            <h3>Edit schema</h3>
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
            };
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
                console.log(schemaName);
            },
        },
    };
</script>

<style scoped>
    .right-border {
        border-right: 1px solid #ddd;
        min-width: 300px;
    }
</style>
