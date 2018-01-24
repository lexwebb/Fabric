<template>
    <div>
        <h1>Browse</h1>
        <div class="row">
            <div class="col-md-3">
                <h3>Data Tree</h3>
                <div class="round-border">
                    <treeView :treeData ="rootNode"></treeView>
                </div>
            </div>
            <div class="col-md-9">
                <h3>Page info</h3>
                One of three columns
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
            };
        },
        mounted() {
            fetch('api/config/')
                .then(response => response.json())
                .then((data) => {
                    this.rootNode = data;
                });
        },
    };
</script>

<style scoped>
    .round-border {
        border: 1px solid #333;
        border-radius: 5px;
    }
</style>
