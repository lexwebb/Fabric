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
                {{property.displayName}} :
                <span v-if="property.value">{{property.value}}</span>
                <span v-else class="null-value">null</span>
            </div>
            <b>Children:</b>
            <div v-for="childGroup in childGroups">
                <div class="child-list-item">
                    {{childGroup.displayName}}:
                    <div v-for="child in childGroup.children" class="child-list-item" :data-child-name="`${childGroup.name}/${child}`">
                        <treeViewItem :model="{name: child}" :path="`${path}/${childGroup.name}/${child}`"></treeViewItem>
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
            model: Object,
            path: {
                type: String,
                required: false,
            },
            open: {
                type: Boolean,
                default: false,
                required: false,
            },
        },
        computed: {
            currentPath() {
                return this.path ? `${this.path}/${this.model.name}` : this.model.name;
            },
            nonChildProperties() {
                const properties = [];
                Object.keys(this.model).forEach((propertyName) => {
                    if (propertyName !== 'children' && propertyName !== 'name') {
                        let value = this.model[propertyName];
                        if (propertyName.includes('Timestamp')) {
                            value = this.$moment.unix(parseInt(value, 10) / 1000).format('DD/MM/YY HH:MM');
                        }

                        properties.push({
                            name: propertyName,
                            displayName: this.$utils.unCamelCase(propertyName),
                            value,
                        });
                    }
                });
                return properties;
            },
            childGroups() {
                const groups = [];
                Object.keys(this.model.children).forEach((group) => {
                    groups.push({
                        name: group,
                        displayName: this.$pluralize(group),
                        children: this.model.children[group],
                    });
                });
                return groups;
            },
        },
        methods: {
            toggle() {
                this.open = !this.open;
            },
        },
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
