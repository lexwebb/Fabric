<template>
    <li>
        <div class="item-title" @click="toggle">
            <span v-if="!open" class="glyphicon glyphicon-chevron-right"></span>
            <span v-else class="glyphicon glyphicon-chevron-down"></span>
            <div class="title-name bold">
                <b>{{model.name}}</b>
            </div>
        </div>

        <div v-if="open" class="item-properties">
            <div v-for="property in nonChildProperties">
                {{property.name}} :
                <span v-if="property.value">{{property.value}}</span>
                <span v-else class="null-value">null</span>
            </div>
            <b>Children:</b>
            <div v-for="childGroup in childGroups">
                <div class="child-list-item">
                    {{childGroup.name}}:
                    <div v-for="child in childGroup.children" class="child-list-item">
                        {{child}}
                    </div>
                </div>
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
        data() {
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
            },
            childGroups() {
                var groups = [];
                for (var group in this.model.children) {
                    groups.push({ name: this.$pluralize(group), children: this.model.children[group] });
                }
                return groups;
            },
        },
        methods: {
            toggle() {
                this.open = !this.open
            },
            changeType() {
                if (!this.isFolder) {
                    Vue.set(this.model, 'children', [])
                    this.addChild()
                    this.open = true
                }
            },
            addChild() {
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
        position: relative;
    }

    .item-properties > * {
        display: block;
        margin-left: 1.5em;
    }

    .null-value {
        color: rebeccapurple;
        font-style: oblique;
    }

    .child-list-item {
        padding-left: 1em;
        border-left: 1px dashed lightgrey;
    }
</style>
