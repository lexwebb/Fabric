<template>
    <md-content>
        <md-field v-if="type === 'string'">
            <label>{{name}}</label>
            <md-input v-model="data"></md-input>
            <span v-if="schema.description" class="md-helper-text">{{schema.description}}</span>
        </md-field>
        <md-field v-if="type === 'boolean'">
            <md-checkbox v-model="data">{{name}}</md-checkbox>
            <span v-if="schema.description" class="md-helper-text">{{schema.description}}</span>
        </md-field>
        <md-field v-if="type === 'number' || type === 'integer'">
            <label>{{name}}</label>
            <md-input v-model="data" type="number"></md-input>
            <span v-if="schema.description" class="md-helper-text">{{schema.description}}</span>
        </md-field>
        <md-content v-if="type === 'object'">
            <md-toolbar :md-elevation="1">
                <span class="md-title">{{name}}</span>
            </md-toolbar>
            <md-list class="md-elevation-1">
                <md-list-item v-for="element in children" :key="element.name">
                    <jsonElement :name="element.name" :required="element.required" :schema="element.schema" :data="element.data"></jsonElement>
                </md-list-item>
            </md-list>
        </md-content>
        <md-content v-if="type === 'array'">
            <md-toolbar :md-elevation="1">
                <span class="md-title">{{name}}</span>
            </md-toolbar>
            <md-list class="md-elevation-1">
                <md-list-item v-for="element in data" :key="element">
                    <jsonElement :schema="schema.items" :data="element"></jsonElement>
                </md-list-item>
            </md-list>
        </md-content>
    </md-content>
</template>

<script>
    export default {
        name: 'jsonElement',
        props: {
            name: String,
            required: Boolean,
            schema: Object,
            data: null,
        },
        computed: {
            type() {
                return this.schema.type ? this.schema.type : 'object';
            },
            children() {
                if (this.type !== 'object') {
                    return undefined;
                }

                const required = this.schema.required;
                return Object.entries(this.schema.properties).map(obj => ({
                    name: obj[0],
                    required,
                    schema: obj[1],
                    data: this.data[obj[0]],
                }));
            },
        },
    };
</script>

<style scoped lang=scss>
    .md-list-item {
        .md-content {
            width: 100%;
        }
    }
</style>

<style lang=scss>
    .md-field {
        .md-checkbox-label {
            top: 0 !important;
        }
    }
</style>

