<template>
    <li>
        <div class="item-title" @click="toggle">
            <span v-if="!internalOpen" class="glyphicon glyphicon-chevron-right"></span>
            <span v-else class="glyphicon glyphicon-chevron-down"></span>
            <div class="title-name bold">
                <b>{{currentModel.name}}</b>
            </div>
        </div>

        <div v-if="internalOpen" class="item-properties">
            <div v-for="property in nonChildProperties">
                {{property.displayName}} :
                <span v-if="property.value">{{property.value}}</span>
                <span v-else class="null-value">null</span>
            </div>
            <b v-if="childGroups.length > 0">Children:</b>
            <div v-for="childGroup in childGroups">
                <div class="child-list-item">
                    {{childGroup.displayName}}:
                    <div v-for="child in childGroup.children" class="child-list-item">
                        <treeViewItem :model="{name: child}" :path="`${currentPath}/${childGroup.name}/${child}`"></treeViewItem>
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
        data() {
            return {
                newModel: undefined,
                internalOpen: this.open,
            };
        },
        computed: {
            currentPath() {
                return this.path ? `${this.path}` : this.currentModel.name;
            },
            currentModel() {
                if (this.newModel) {
                    return this.newModel;
                }

                return this.model;
            },
            nonChildProperties() {
                const properties = [];
                Object.keys(this.currentModel).forEach((propertyName) => {
                    if (propertyName !== 'children' && propertyName !== 'name') {
                        let value = this.currentModel[propertyName];
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
                if (this.currentModel.children) {
                    Object.keys(this.currentModel.children).forEach((group) => {
                        groups.push({
                            name: group,
                            displayName: this.$pluralize(group),
                            children: this.currentModel.children[group],
                        });
                    });
                }
                return groups;
            },
        },
        methods: {
            toggle() {
                this.internalOpen = !this.internalOpen;

                if (this.internalOpen) {
                    fetch(`api/config/${this.currentPath.replace(/^(root\/|root)/, '')}`)
                        .then(response => response.json())
                        .then((data) => {
                            this.newModel = data;
                        }).catch((e) => {
                            alert(e); // TODO replace alert with toast
                        });
                }
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
