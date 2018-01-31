<template>
    <ul>
        <treeViewItem :model="treeData" :open="true" @page-edit="onPageEdit"></treeViewItem>
    </ul>
</template>

<script>
    import treeViewItem from './treeViewItem.vue';

    export default {
        name: 'treeView',
        components: {
            treeViewItem,
        },
        props: {
            treeData: Object,
        },
        methods: {
            onPageEdit(e) {
                this.$emit('page-edit', e);
            },
        },
    };
</script>

<style scoped lang=scss>
    ul {
        padding-left: 1em;
        line-height: 1.5em;
        list-style-type: none;
    }
</style>
