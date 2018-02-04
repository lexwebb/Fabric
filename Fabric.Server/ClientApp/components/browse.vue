<template>
    <div>
        <h1>Browse</h1>
        <div class="md-layout md-gutter ">
            <div class="md-layout-item md-size-30 right-border">
                <h3>Data Tree</h3>
                <treeView :treeData="rootNode" @page-edit="onPageEdit"></treeView>
            </div>
            <div class="md-layout-item" v-if="currentEditItem">
                <h3>Edit page</h3>
                <pageEditor :model="currentEditItem"></pageEditor>
            </div>
        </div>
    </div>
</template>

<script>
    import treeView from './treeView/treeView.vue';
    import pageEditor from './pageEditor.vue';

    // Import the EventBus.
    import { EventBus } from '../event-bus';

    export default {
        name: 'browse',
        components: {
            treeView,
            pageEditor,
        },
        data() {
            return {
                rootNode: {},
                currentEditItem: undefined,
            };
        },
        mounted() {
            this.$services.config.get()
                .then((data) => {
                    this.rootNode = data;
                }).catch(() => {
                    EventBus.$emit('show-error', 'Error loading configs');
                });
        },
        methods: {
            onPageEdit(name) {
                this.$services.config.get(name.replace(/^(root\/|root)/, ''))
                    .then((data) => {
                        this.currentEditItem = data;
                    }).catch(() => {
                        EventBus.$emit('show-error', 'Error loading config');
                    });
            },
        },
    };
</script>

<style scoped lang=scss>
    .right-border {
        border-right: 1px solid #ddd;
        min-width: 300px;
    }
</style>
