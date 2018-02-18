<template>
    <div id='app-root' class="page-container md-layout-row">
        <md-app md-waterfall md-mode="fixed">
            <md-app-toolbar class="md-primary" md-elevation="0">
                <md-button class="md-icon-button" @click="toggleMenu" v-if="!menuVisible">
                    <md-icon>menu</md-icon>
                </md-button>
                <div class="md-title">
                    <img src="assets/Fabric-Logo-Mini.png" class="title-image" />
                    <span v-if="this.$route.name === 'home'" class="title-text">Fabric</span>
                    <span v-else class="title-text">Fabric > {{this.$route.meta.title}}</span>
                </div>
            </md-app-toolbar>
            <md-app-drawer :md-active.sync="menuVisible" md-persistent="mini" class="nav-draw">
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
                <transition name="fade">
                    <router-view></router-view>
                </transition>
            </md-app-content>
        </md-app>
        <md-snackbar md-position="center" :md-duration="snack.duration" :md-active.sync="snack.showSnackbar" md-persistent :class="{ error: snack.isError}">
            <span>{{snack.text}}</span>
            <md-button :class="{ 'md-accent': !snack.isError, error: snack.isError }" @click="snack.showSnackbar = false">Close</md-button>
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
                    isError: false,
                },
            };
        },
        mounted() {
            // Listen for the show-error event and its payload.
            EventBus.$on('show-error', (message) => {
                this.snack.text = message;
                this.snack.isError = true;
                this.snack.showSnackbar = true;
            });

            EventBus.$on('show-message', (message) => {
                this.snack.text = message;
                this.snack.isError = false;
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

<style lang="scss">
    @import '~vue-material/dist/theme/engine';

    // Import the theme engine
    @include md-register-theme('default', ( 
                                            primary: #1c8d7d,
                                            accent: #21b6a4
                                        ));

    @import '~vue-material/dist/theme/all';
    // Apply the theme
    html,
    body,
    #app-root {
        height: 100%;
    }
    #app-root .md-app {
        height: 100%;
        .md-layout {
            height: 100%;
            .md-layout-item {
                height: 100%;
            }
        }
    }
    h1,
    h2,
    h3,
    h4,
    h5 {
        margin: 0.5em 0;
    }
    .md-title {
        margin-left: 3px !important;
    }
    .md-drawer {
        max-width: 300px;
        background-color: #424242 !important;
        border-right: none !important;
        ul {
            background-color: transparent !important;
            button,
            button i {
                color: white !important;
            }
        }
    }
    .md-toolbar.md-theme-default.md-primary {
        background: repeating-linear-gradient(
            45deg,
            #272727,
            #272727 10px,
            #222 10px,
            #222 20px
        );
    }
    .md-app-content > * {
        display: flex;
        flex-direction: column;
        height: 100%;
    }
    .md-content {
        padding: 0px;
    }
    .md-layout-item {
        padding: 0.5em !important;
    }
    .md-app-content > * > .md-layout {
        flex-grow: 1;
        flex-wrap: nowrap;
    }
    .fade-enter-active,
    .fade-leave-active {
        transition: opacity 0.2s;
    }
    .fade-enter,
    .fade-leave-to {
        opacity: 0;
    }
    .flex-row-right {
        display: flex;
        flex-direction: row;
        justify-content: flex-end;
    }
    .error-message {
        color: #f44336;
        font-style: italic;
    }
    .md-title {
        height: 40px;
        line-height: 40px;
        .title-image {
            height: 40px;
            display: inline-block;
            padding: 5px;
        }
        .title-text {
            height: 40px;
            line-height: 40px;
        }
    }
    .md-snackbar.error {
        background: #f44336;
        .md-button.error {
            color: white;
        }
    }
    .scrollable {
        overflow: auto;
    }
</style>
