<template>
    <div>
        <h1>Browse</h1>
        <div class="row">
            <div class="col-md-3">
                <h3>Data Tree</h3>
                <div class="round-border">
                    <treeView :treeData ="rootNode" @page-edit="onPageEdit"></treeView>
                </div>
            </div>
            <div v-if="currentEditItem" class="col-md-9">
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
    .round-border {
        border: 1px solid #333;
        border-radius: 5px;
    }
</style>
