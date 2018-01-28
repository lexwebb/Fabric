<template>
    <md-list>
        <md-list-item @click="navigate('/')">
            <md-icon>home</md-icon>
            <span class="md-list-item-text">Home</span>
        </md-list-item>
        <md-list-item @click="navigate('/browse')">
            <md-icon>folder_open</md-icon>
            <span class="md-list-item-text">Browse</span>
        </md-list-item>
        <md-list-item @click="navigate('/schemas')">
            <md-icon>format_align_left</md-icon>
            <span class="md-list-item-text">Schemas</span>
        </md-list-item>
    </md-list>
</template>

<script>
    export default {
        name: 'navmenu',
        methods: {
            navigate(path) {
                this.$router.push(path);
            },
        },
    };
</script>
