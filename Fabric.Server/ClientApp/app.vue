<template>
    <div id='app-root' class="page-container md-layout-row">
        <md-app>
            <md-app-toolbar class="md-primary">
                <span class="md-title">
                    <img src="assets/Fabric-Logo-Mini.png" style="height: 30px; width: 30px; display: inline-block;" />
                    Fabric
                </span>
            </md-app-toolbar>
            <md-app-drawer md-permanent="clipped">
                <navmenu />
            </md-app-drawer>
            <md-app-content>
                <router-view></router-view>
            </md-app-content>
        </md-app>
    </div>
</template>

<script>
    import navmenu from './components/navmenu.vue';

    export default {
        name: 'app',
        components: {
            navmenu,
        },
    };
</script>

<style>
    html,
    body,
    #app-root{
        height: 100%;
    }

    #app-root .md-app {
        height: 100%;
    }

    .md-drawer {
        max-width: 300px;
    }

    .md-toolbar.md-theme-default.md-primary {
        background: repeating-linear-gradient( 45deg, #272727, #272727 10px, #222 10px, #222 20px );
    }
</style>
