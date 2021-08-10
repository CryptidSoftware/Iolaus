<div
    class="py-5"
>
    <div
        class="border-2 border-blue-100 container"
    >
        <p>{configuration}</p>
    </div>
    <button
        on:click
        class="remove-button"
    >
        Remove
    </button>
</div>

<script lang="ts">
    export let configuration:string;
</script>

<style lang="postcss">
    .remove-button {
        @apply font-bold rounded py-2 px-4 bg-red-400 mt-2 text-white hover:bg-red-500;
    }
</style>