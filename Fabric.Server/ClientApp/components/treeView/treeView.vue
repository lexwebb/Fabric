<template>
    <ul>
        <treeViewItem :model="treeData"></treeViewItem>
    </ul>
</template>

<script>
    import treeViewItem from './treeViewItem.vue'

    export default {
        name: 'treeView',
        components: {
            treeViewItem,
        },
        props: {
            treeData: Object
        },
    }
</script>

<style scoped>
    ul {
        padding-left: 1em;
        line-height: 1.5em;
        list-style-type: none;
    }
</style>
