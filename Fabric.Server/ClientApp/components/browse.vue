<template>
    <div>
        <div class="md-layout">
            <div class="md-layout-item md-size-30 right-border">
                <h3>Data Tree</h3>
                <treeView :treeData="rootNode"></treeView>
            </div>
            <div class="md-layout-item edit-container" v-if="currentEditItem">
                <h3>Edit page - {{currentEditItem.name}}</h3>
                <pageEditor :model="currentEditItem"></pageEditor>
                <div class="flex-row-right">
                    <md-button class="md-raised">Cancel</md-button>
                    <md-button class="md-raised md-primary" @click="save">Save</md-button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import { mapState } from 'vuex';
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
        computed: {
            ...mapState({
                rootNode: state => state.browse.rootNode,
                currentEditItem: state => state.browse.currentPage,
            }),
        },
        mounted() {
            this.onRoute();
            EventBus.$on('browse-edit', (path) => {
                this.$router.push({ name: 'browse', params: { path: this.$utils.trim(path, 'root/') } });
            });
        },
        methods: {
            onRoute(to) {
                const route = to || this.$route;

                this.$store.dispatch('browse/getRootNode')
                    .catch(() => {
                        EventBus.$emit('show-error', 'Error loading configs');
                    });

                if (route.params.path) {
                    this.$store.dispatch('browse/getPage', `root/${route.params.path}`)
                        .then(() => {
                            this.$store.dispatch('browse/loadEditor')
                                .catch(() => {
                                    EventBus.$emit('show-error', 'Error loading editor');
                                });
                        })
                        .catch(() => {
                            EventBus.$emit('show-error', 'Error loading config');
                        });
                }
            },
            save() {
                this.$store.dispatch('browse/save')
                    .then(() => {
                        EventBus.$emit('show-message', 'Saved sucessfully!');
                    })
                    .catch(() => {
                        EventBus.$emit('show-error', 'Error saving config');
                    });
            },
        },
        watch: {
            $route(to) {
                this.onRoute(to);
            },
        },
    };
</script>

<style scoped lang="scss">
    .right-border {
        border-right: 1px solid #ddd;
        min-width: 300px;
    }
</style>

<style lang="scss">
    .edit-container {
        display: flex;
        flex-direction: column;
        height: 100%;
        .md-tabs {
            flex-grow: 1;
            min-height: 100px;
            .md-content {
                height: 100% !important;
                max-height: 100%;
                .md-tabs-container {
                    height: 100%;
                    .md-tab {
                        height: 100%;
                        overflow: auto;
                    }
                }
            }
        }
    }
</style>
