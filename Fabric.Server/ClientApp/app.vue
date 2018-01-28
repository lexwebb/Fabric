<template>
    <div id='app-root' class="page-container md-layout-row">
        <md-app>
            <md-app-toolbar class="md-primary" md-elevation="0">
                <md-button class="md-icon-button" @click="toggleMenu" v-if="!menuVisible">
                    <md-icon>menu</md-icon>
                </md-button>
                <span class="md-title">
                    <img src="assets/Fabric-Logo-Mini.png" style="height: 30px; width: 30px; display: inline-block;" />
                    Fabric
                </span>
            </md-app-toolbar>
            <md-app-drawer :md-active.sync="menuVisible" md-persistent="mini">
                <md-toolbar class="md-transparent" md-elevation="0">
                    Navigation
                    <div class="md-toolbar-section-end">
                        <md-button class="md-icon-button md-dense" @click="toggleMenu">
                            <md-icon>keyboard_arrow_left</md-icon>
                        </md-button>
                    </div>
                </md-toolbar>
                <navmenu />
            </md-app-drawer>
            <md-app-content>
                <router-view></router-view>
            </md-app-content>
        </md-app>
        <md-snackbar md-position="center" :md-duration="snack.duration" :md-active.sync="snack.showSnackbar" md-persistent>
            <span>{{snack.text}}</span>
            <md-button class="md-accent" @click="snack.showSnackbar = false">Close</md-button>
        </md-snackbar>
    </div>
</template>

<script>
    import navmenu from './components/navmenu.vue';
    import { EventBus } from './event-bus';

    export default {
        name: 'app',
        components: {
            navmenu,
        },
        data() {
            return {
                menuVisible: false,
                snack: {
                    showSnackbar: false,
                    duration: 5000,
                    text: 'An unexpected error occured',
                },
            };
        },
        mounted() {
            // Listen for the show-error event and its payload.
            EventBus.$on('show-error', (message) => {
                this.snack.text = message;
                this.snack.showSnackbar = true;
            });
        },
        methods: {
            toggleMenu() {
                this.menuVisible = !this.menuVisible;
            },
        },
    };
</script>

<style>
    html,
    body,
    #app-root {
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
