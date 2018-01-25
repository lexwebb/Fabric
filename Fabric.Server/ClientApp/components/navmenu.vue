<template>
    <div>
        <md-toolbar class="md-transparent" md-elevation="0">
            Navigation
        </md-toolbar>

        <md-list>
            <md-list-item @click="navigate('/')">
                <md-icon>home</md-icon>
                <span class="md-list-item-text">Home</span>
            </md-list-item>

            <md-list-item @click="navigate('/browse')">
                <md-icon>folder_open</md-icon>
                <span class="md-list-item-text">Browse</span>
            </md-list-item>
        </md-list>
    </div>
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
