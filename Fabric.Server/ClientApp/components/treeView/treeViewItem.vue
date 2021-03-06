﻿<template>
    <li class="item-row">
        <div class="item-title" @click="toggle">
            <md-icon v-if="!internalOpen">keyboard_arrow_right</md-icon>
            <md-icon v-else>keyboard_arrow_down</md-icon>
            <div class="title-name bold">
                <b>{{currentModel.name}}</b>
            </div>
            <a v-if="currentModel.name !== 'root'" class="noselect" @click="editItem" v-on:click.stop>
                <md-icon class="item_button noselect">edit</md-icon>
            </a>
            <a v-if="currentModel.name !== 'root'" class="noselect" @click="deleteItem" v-on:click.stop>
                <md-icon class="item_button noselect">delete</md-icon>
            </a>
        </div>
        <div v-if="internalOpen" class="item-properties">
            <div v-for="property in nonChildProperties" :key="property.name">
                {{property.displayName}} :
                <span v-if="property.value">{{property.value}}</span>
                <span v-else class="null-value">null</span>
            </div>
            <b v-if="childGroups.length > 0">Children:</b>
            <div v-for="childGroup in childGroups" :key="childGroup.name">
                <div class="child-list-item">
                    {{childGroup.displayName}}:
                    <div v-for="child in childGroup.children" :key="child.name" class="child-list-item">
                        <treeViewItem :model="{name: child}" :path="`${currentPath}/${childGroup.name}/${child}`"></treeViewItem>
                    </div>
                </div>
            </div>
        </div>
    </li>
</template>

<script>
    // Import the EventBus.
    import { EventBus } from '../../event-bus';

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
                    if (propertyName !== 'children' && propertyName !== 'name' && propertyName !== 'pageData') {
                        let value = this.currentModel[propertyName];
                        if (propertyName.includes('Timestamp')) {
                            value = this.$moment
                                .unix(parseInt(value, 10) / 1000)
                                .format('gggg/MM/DD LT');
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
                        })
                        .catch(() => {
                            EventBus.$emit('show-error', 'Error loading config');
                        });
                }
            },
            editItem() {
                EventBus.$emit('browse-edit', this.currentPath);
            },
            deleteItem() {
                EventBus.$emit('browse-delete', this.currentPath);
            },
        },
    };
</script>

<style scoped lang=scss>
    $color_1: rebeccapurple;
    $color_2: grey;

    .item-row {
        display: flex;
        flex-direction: column;
        .item-title {
            display: flex;
            flex-direction: row;
            transition: background-color 0.3s;
            &:hover {
                background-color: #eee;
            }
            > * {
                display: inline;
                cursor: pointer;
            }
            .title-name {
                flex: 1;
            }
            .item_button {
                color: $color_2;
                text-decoration: none;
                cursor: pointer;
                color: lightgray;
            }
        }
        .item-properties {
            > * {
                display: block;
                margin-left: 1.5em;
            }
            .child-list-item {
                padding-left: 1em;
                border-left: 1px dashed lightgrey;
            }
            .null-value {
                color: $color_1;
                font-style: oblique;
            }
        }
    }
</style>
