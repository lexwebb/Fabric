<template>
    <li>
        <div class="item-title" @click="toggle">
            <span v-if="!open" class="glyphicon glyphicon-chevron-right"></span>
            <span v-else class="glyphicon glyphicon-chevron-down"></span>
            <div class="bold">
                <b>{{model.name}}</b>
            </div>
        </div>

        <div v-if="open" class="item-properties">
            <div v-for="property in nonChildProperties">
                {{property.name}} : 
                <span v-if="property.value">{{property.value}}</span>
                <span v-else class="null-value">null</span>
            </div>
        </div>
    </li>
</template>

<script>
    export default {
        name: 'treeViewItem',
        props: {
            model: Object
        },
        data () {
            return {
                open: true
            }
        },
        computed: {
            nonChildProperties() {
                var properties = [];
                for (var propertyName in this.model) {
                    if (propertyName != 'children' && propertyName != 'name') {
                        properties.push({ name: propertyName, value: this.model[propertyName] })
                    }
                }
                return properties;
            }
        },
        methods: {
            toggle () {
                this.open = !this.open
            },
            changeType () {
                if (!this.isFolder) {
                    Vue.set(this.model, 'children', [])
                    this.addChild()
                    this.open = true
                }
            },
            addChild () {
                this.model.children.push({
                    name: 'new stuff'
                })
            }
        }
    };
</script>

<style scoped>
    .item-title > * {
        display: inline;
        cursor: pointer;
    }

    .item-properties > * {
        display: block;
        margin-left: 1.5em;
    }

    .null-value {
        color: rebeccapurple;
        font-style: oblique;
    }
</style>
