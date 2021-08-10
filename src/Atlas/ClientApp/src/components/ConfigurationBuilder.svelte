<TextBox
    bind:value={configurationDefinition}
    {placeholder}
    on:keyup
/>

<button
    disabled={isAddConfigurationDisabled}
    on:click
    class="add-button"
>
    Add
</button>

<style lang="postcss">
    .add-button {
        @apply font-bold rounded py-2 px-4 bg-green-400 text-white hover:bg-green-500;
    }

    textarea {
        width:100%;
        resize:none;
    }
</style>

<script lang="ts">
    import TextBox from './TextBox.svelte';
    export let configurationDefinition:string;
    export let isAddConfigurationDisabled:boolean;

    let placeholder:string =
`{
    'Pattern': { '$.jsonPath': 'a_value' },
    'Route' : { 'Type': 'NATS', 'Subject': 'potatoes' }
}`;
</script>