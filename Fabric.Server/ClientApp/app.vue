<template>
    <div id='app-root' class="container-fluid">
        <div class="row">
            <div class="col-sm-2">
                <navmenu />
            </div>
            <div class="col-sm-10">
                <router-view></router-view>
            </div>
        </div>
    </div>
</template>

<script>
    import navmenu from './components/navmenu.vue';

    export default {
        name: 'app',
        components: {
            navmenu,
        },
    }
</script>
