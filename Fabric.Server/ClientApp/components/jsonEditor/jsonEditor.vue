<template>
    <jsonElement :name="name" :schema="schema" :data="data" :type="object" :depth="0" @dataChanged="childDataChanged"></jsonElement>
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
        data() {
            return {
                modifiedData: {},
            };
        },
        mounted() {
            this.modifiedData = this.data;
        },
        methods: {
            childDataChanged(e) {
                let schema = this.modifiedData;
                const pList = e.path.split('/');
                const len = pList.length;
                for (let i = 0; i < len - 1; i += 1) {
                    const elem = pList[i];
                    if (!schema[elem]) schema[elem] = {};
                    schema = schema[elem];
                }

                schema[pList[len - 1]] = e.data;
            },
        },
        watch: {
            data() {
                this.modifiedData = this.data;
            },
        },
    };
</script>

<style scoped>
    .property-container {
        padding: 0.5em;
    }
</style>


