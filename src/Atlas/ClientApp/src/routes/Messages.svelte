<TextBox
    bind:value={message}
/>

<button
    on:click={send}
    class="send-button"
>
    Send
</button>

<TextBox
    bind:value={responseMessage}
/>


<style lang="postcss">
    .send-button {
        @apply font-bold rounded py-2 px-4 bg-green-400 text-white hover:bg-green-500;
    }
</style>

<script lang='ts'>
    import TextBox from '../components/TextBox.svelte';
    let message:string = '';
    let responseMessage:string = '';

    async function send() : Promise<void> {
        const response = await fetch(`api/configuration/send/`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({Message: message})
        });

        if (response.ok) {
            responseMessage = await response.text();
        } else {
            throw new Error(await response.text());
        }
    }
</script>