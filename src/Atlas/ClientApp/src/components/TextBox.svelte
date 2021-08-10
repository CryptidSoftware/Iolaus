<textarea
    {placeholder}
    class='container border-2 border-blue-200 mb-2 mt-2 placeholder-blue-500 text-blue-500'
    rows=4
    bind:value
    on:keyup
/>

<style>
    textarea {
        resize:none
    }
</style>

<script lang="ts">
    export let placeholder:string;
    export let value:string;
</script>