<template>
    <jsonElement ref="editor" :name="name" :schema="schema" :data="modifiedData" :type="'object'" :depth="0" @dataChanged="childDataChanged"></jsonElement>
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
                reloading: false,
            };
        },
        created() {
            this.modifiedData = this.data;
        },
        methods: {
            childDataChanged(e) {
                if (!this.reloading) {
                    let schema = this.modifiedData;
                    const pList = e.path.split('/');
                    const len = pList.length;
                    for (let i = 0; i < len - 1; i += 1) {
                        const elem = pList[i];
                        if (!schema[elem]) schema[elem] = {};
                        schema = schema[elem];
                    }

                    schema[pList[len - 1]] = e.data;

                    this.$emit('changed', this.modifiedData);
                }
            },
        },
        watch: {
            data() {
                this.reloading = true;
                this.modifiedData = this.data;

                this.$nextTick(() => {
                    this.reloading = false;
                });
            },
        },
    };
</script>

<style scoped>
    .property-container {
        padding: 0.5em;
    }
</style>


