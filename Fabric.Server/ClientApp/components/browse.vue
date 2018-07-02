<template>
    <div>
        <navContentLayout navHeaderText="Data Tree" contentHeaderText="Edit Page">
            <div slot="nav">
                <treeView :treeData="rootNode"></treeView>
            </div>
            <div slot="content" v-if="currentEditItem">
                <pageEditor :model="currentEditItem"></pageEditor>
            </div>
            <div slot="footer">
                <div class="flex-row-right">
                    <md-button class="md-raised">Cancel</md-button>
                    <md-button class="md-raised md-primary" @click="save">Save</md-button>
                </div>
            </div>
        </navContentLayout>
        <md-dialog-confirm :md-active.sync="deleteDialogOpen" md-title="Are you sure you want to delete this page?" md-content="Deleteing this page will also delete any child pages it may have, this can be undone by reverting to a previous version." md-confirm-text="Delete" md-cancel-text="Cancel" @md-cancel="onDeleteCancel" @md-confirm="onDeleteConfirm" />
    </div>
</template>

<script>
    import { mapState } from 'vuex';
    import treeView from './treeView/treeView.vue';
    import pageEditor from './pageEditor.vue';
    import navContentLayout from './layouts/navContentLayout.vue';

    // Import the EventBus.
    import { EventBus } from '../event-bus';

    export default {
        name: 'browse',
        data() {
            return {
                deleteDialogOpen: false,
                deletePath: '',
            };
        },
        components: {
            navContentLayout,
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
            EventBus.$on('browse-delete', (path) => {
                this.deletePath = this.$utils.trim(path, 'root/');
                this.deleteDialogOpen = true;
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
            onDeleteCancel() {
                this.deletePath = '';
                this.deleteDialogOpen = false;
            },
            onDeleteConfirm() {
                this.$store.dispatch('browse/deletePage', `root/${this.deletePath}`)
                    .then(() => {
                        EventBus.$emit('show-message', 'Deleted sucessfully!');
                    })
                    .catch(() => {
                        EventBus.$emit('show-error', 'Error deleting page');
                    })
                    .finally(() => {
                        this.deletePath = '';
                        this.deleteDialogOpen = false;
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
</style>
