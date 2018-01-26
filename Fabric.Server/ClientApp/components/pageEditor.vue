<template>
    <div>
        <h4>{{model.name}}</h4>
    </div>
</template>

<script>
    export default {
        name: 'pageEditor',
        props: {
            model: Object,
        },
    };
</script>
