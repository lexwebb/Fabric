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
    <!--<div class="navbar navbar-inverse">
        <div class="navbar-header">
            <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                <span class="sr-only">Toggle navigation</span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
                <span class="icon-bar"></span>
            </button>
            <a class="navbar-brand" href="/">
                <img src="assets/Fabric-Logo-Mini.png" style="height: 30px; width: 30px; display: inline-block;"/>
                <span style="display: inline">Fabric.Server</span>
            </a>
        </div>
        <div class="clearfix"></div>
        <div class="navbar-collapse collapse">
            <ul class="nav navbar-nav">
                <li>
                    <router-link to="/" :exact="true">
                        <span class="glyphicon glyphicon-home"></span> Home
                    </router-link>
                </li>
                <li>
                    <router-link to="/browse">
                        <span class="glyphicon glyphicon-folder-close"></span> Browse
                    </router-link>
                </li>
            </ul>
        </div>
    </div>-->
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
