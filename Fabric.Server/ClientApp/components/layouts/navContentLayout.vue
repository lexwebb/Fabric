<template>
    <div class="md-layout" v-if="!isSmallScreen">
        <div class="md-layout-item md-size-30 right-border">
            <h3>{{navHeaderText}}</h3>
            <slot name="nav"></slot>
        </div>
        <div class="md-layout-item">
            <h3>{{contentHeaderText}}</h3>
            <slot name="content"></slot>
        </div>
    </div>
    <md-tabs md-alignment="fixed" v-else>
        <md-tab id="tab-nav" :md-label="navHeaderText">
            <slot name="nav"></slot>
        </md-tab>
        <md-tab id="tab-content" :md-label="contentHeaderText">
            <slot name="content"></slot>
        </md-tab>
    </md-tabs>
</template>

<script>
    export default {
        name: 'navContentLayout',
        props: {
            navHeaderText: {
                default: 'Navigation',
                type: String,
            },
            contentHeaderText: {
                default: 'Content',
                type: String,
            },
        },
        data() {
            return {
                isSmallScreen: false,
            };
        },
        methods: {
            onResize() {
                if (window.innerWidth <= 1200) {
                    this.isSmallScreen = true;
                } else {
                    this.isSmallScreen = false;
                }
            },
        },
        created() {
            this.onResize();
            window.addEventListener('resize', this.onResize);
        },
        beforeDestroy() {
            window.removeEventListener('resize', this.onResize);
        },
    };
</script>

<style scoped lang=scss>
    .right-border {
        border-right: 1px solid #ddd;
        min-width: 300px;
    }
</style>
