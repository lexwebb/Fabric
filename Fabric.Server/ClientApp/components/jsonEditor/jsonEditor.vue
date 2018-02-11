<template>
     <jsonElement :name="name" :schema="schema" :data="data" :type="object"></jsonElement>
</template>

<script>
    import jsonElement from './jsonElement.vue';

    export default {
        name: 'jsonEditor',
        components: {
            jsonElement,
        },
        props: {
            name: String,
            schema: Object,
            data: Object,
        },
    };
</script>

<style scoped>
    .property-container {
        padding: 0.5em;
    }
</style>


