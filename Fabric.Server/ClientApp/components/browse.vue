<template>
    <div>
        <h1>Browse</h1>
        <div class="md-layout md-gutter ">
            <div class="md-layout-item md-size-30 right-border">
                <h3>Data Tree</h3>
                <div>
                    <treeView :treeData="rootNode" @page-edit="onPageEdit"></treeView>
                </div>
            </div>
            <div class="md-layout-item" v-if="currentEditItem">
                <h3>Edit page</h3>
                <h4>{{currentEditItem.name}}</h4>
            </div>
        </div>
    </div>
</template>

<script>
    import treeView from './treeView/treeView.vue';

    export default {
        name: 'browse',
        components: {
            treeView,
        },
        data() {
            return {
                rootNode: {},
                currentEditItem: undefined,
            };
        },
        mounted() {
            fetch('api/config/')
                .then(response => response.json())
                .then((data) => {
                    this.rootNode = data;
                }).catch((e) => {
                    alert(e); // TODO replace alert with toast
                });
        },
        methods: {
            onPageEdit(name) {
                fetch(`api/config/${name.replace(/^(root\/|root)/, '')}`)
                    .then(response => response.json())
                    .then((data) => {
                        this.currentEditItem = data;
                    }).catch((e) => {
                        alert(e); // TODO replace alert with toast
                    });
            },
        },
    };
</script>

<style scoped>
    .right-border {
        border-right: 1px solid #ddd;
        min-width: 300px;
    }
</style>
