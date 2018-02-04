<template>
    <div class="title-button">
        <h3>{{title}}</h3>
        <div v-if="showButton" class="button-container">
            <md-button class="md-icon-button md-dense md-raised md-primary edit-schema-button" @click="onClick()">
                <md-icon>{{icon}}</md-icon>
            </md-button>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'buttonTitle',
        props: {
            title: String,
            icon: String,
            showButton: {
                default: true,
                type: Boolean,
            },
        },
        methods: {
            onClick() {
                this.$emit('onClick');
            },
        },
    };
</script>

<style scoped lang=scss>
    .title-button {
        display: flex;
        flex-direction: row;
        h3 {
            flex-grow: 1;
        }
        .button-container {
            display: flex;
            flex-direction: column;
            justify-content: center;
        }
    }
</style>
