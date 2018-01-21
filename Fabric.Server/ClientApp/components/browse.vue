<template>
    <div>
        <h1>Browse</h1>
        <div class="row">
            <div class="col-md-3">
                <tree-view :data="rootNode" :options="{maxDepth: 3}"></tree-view>
            </div>
            <div class="col-md-9">
                One of three columns
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'browse',
        data() {
            return {
                rootnode: {},
            };
        },
        mounted() {
            fetch('api/config/')
                .then(response => response.json())
                .then(data => {
                    this.rootNode = data;
                });
        }
    }
</script>
