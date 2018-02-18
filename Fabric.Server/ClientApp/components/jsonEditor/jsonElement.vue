<template>
    <div>
        <div v-if="hasEnum">
            <md-field>
                <label :for="compName + depth">{{compName}}</label>
                <md-select v-model="internalData" :name="compName + depth" :id="compName + depth">
                    <md-option v-for="option in enumOptions" :key="option" :value="option">{{option}}</md-option>
                </md-select>
            </md-field>
        </div>
        <md-field v-else-if="type === 'string'" md-clearable>
            <label>{{compName}}</label>
            <md-input v-model="internalData"></md-input>
            <span v-if="schema.description" class="md-helper-text">{{schema.description}}</span>
        </md-field>
        <md-field v-else-if="type === 'boolean'" md-clearable>
            <md-checkbox v-model="internalData">{{name}}</md-checkbox>
            <span v-if="schema.description" class="md-helper-text">{{schema.description}}</span>
        </md-field>
        <md-field v-else-if="type === 'number' || type === 'integer'" md-clearable>
            <label>{{compName}}</label>
            <md-input v-model="internalData" type="number"></md-input>
            <span v-if="schema.description" class="md-helper-text">{{schema.description}}</span>
        </md-field>
        <div v-else-if="type === 'object'">
            <md-toolbar :md-elevation="1" :style="{ 'border-color': borderColor}" style="border-left: 3px solid;">
                <span class="md-title">
                    <md-button class="md-icon-button" @click="toggleOpen">
                        <md-icon v-if="open">keyboard_arrow_down</md-icon>
                        <md-icon v-else>keyboard_arrow_right</md-icon>
                    </md-button>
                    <span>{{compName}}</span>
                </span>
            </md-toolbar>
            <md-list class="md-elevation-1" v-if="open">
                <md-list-item v-for="element in children" :key="element.name">
                    <jsonElement :name="element.name" :path="element.name" :required="element.required" :schema="element.schema" :data="element.data" :depth="childDepth" @dataChanged="childDataChanged"></jsonElement>
                </md-list-item>
            </md-list>
        </div>
        <div v-else-if="type === 'array'">
            <md-toolbar :md-elevation="1" :style="{ 'border-color': borderColor}" style="border-left: 3px solid;">
                <span class="md-title">
                    <md-button class="md-icon-button" @click="toggleOpen">
                        <md-icon v-if="open">keyboard_arrow_down</md-icon>
                        <md-icon v-else>keyboard_arrow_right</md-icon>
                    </md-button>
                    <span>{{compName}}</span>
                    <md-button class="md-icon-button md-dense md-raised md-primary" @click="arrayAdd">
                        <md-icon>add</md-icon>
                    </md-button>
                </span>
            </md-toolbar>
            <md-list class="md-elevation-1" v-if="open">
                <md-list-item v-for="(element, index) in internalData" :key="element.name">
                    <jsonElement :schema="schema.items" :path="index" :data="element" :depth="childDepth" :isParentArray=true @dataChanged="childDataChanged"></jsonElement>
                </md-list-item>
            </md-list>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'jsonElement',
        props: {
            name: String,
            path: null,
            required: Boolean,
            schema: Object,
            data: null,
            depth: {
                default: -1,
                type: Number,
            },
            isParentArray: {
                default: false,
                type: Boolean,
            },
        },
        data() {
            return {
                // Perform clone of object to avoid mutating property
                internalData: JSON.parse(JSON.stringify(this.data)),
                open: true,
                reloading: false,
            };
        },
        mounted() {
            if (this.depth > 3) {
                this.open = false;
            }
        },
        computed: {
            compName() {
                return this.name ? this.name : this.internalData.name;
            },
            type() {
                return this.schema.type ? this.schema.type : 'object';
            },
            hasEnum() {
                return !!this.schema.enum;
            },
            enumOptions() {
                return this.schema.enum;
            },
            children() {
                if (this.type !== 'object') {
                    return undefined;
                }

                const required = !this.schema.required ? false
                    : this.schema.required.includes(this.name);
                return Object.entries(this.schema.properties).map(obj => ({
                    name: obj[0],
                    required,
                    schema: obj[1],
                    data: this.internalData[obj[0]],
                }));
            },
            childDepth() {
                return this.depth + 1;
            },
            borderColor() {
                const colors = ['#e57373', '#BA68C8', '#7986CB', '#4FC3F7', '#4DB6AC', '#AED581'];
                switch (this.depth) {
                case 0:
                case 6:
                    return colors[0];
                case 1:
                case 7:
                    return colors[1];
                case 2:
                case 8:
                    return colors[2];
                case 3:
                case 9:
                    return colors[3];
                case 4:
                case 10:
                    return colors[4];
                case 5:
                case 11:
                    return colors[5];
                default:
                    return 'grey';
                }
            },
        },
        methods: {
            toggleOpen() {
                this.open = !this.open ? true : !this.open;
            },
            arrayAdd() {

            },
            childDataChanged(e) {
                if (e && e.path) {
                    if (this.path ||
                        (!Number.isNaN(parseFloat(this.path)) && Number.isFinite(this.path))) {
                        this.$emit('dataChanged', { path: `${this.path}/${e.path}`, data: e.data });
                    } else {
                        this.$emit('dataChanged', { path: e.path, data: e.data });
                    }
                }
            },
        },
        watch: {
            data() {
                this.reloading = true;
                this.internalData = this.data;

                this.$nextTick(() => {
                    this.reloading = false;
                });
            },
            internalData() {
                if (!this.reloading) {
                    this.$emit('dataChanged', { path: this.path, data: this.internalData });
                }
            },
        },
    };
</script>

<style scoped lang=scss>
    .md-list-item {
        div {
            width: 100%;
        }
    }
    .md-title {
        display: flex;
        flex-direction: row;
        width: 100%;
        span:not(.md-title) {
            flex: 1;
        }
    }
    .md-toolbar {
        min-height: 40px;
    }
</style>

<style lang=scss>
    .md-field {
        .md-checkbox-label {
            top: 0 !important;
        }
    }
</style>

