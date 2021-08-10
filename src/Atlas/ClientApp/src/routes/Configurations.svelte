<svelte:head>
    <title>configurations</title>
</svelte:head>
<div class="bordered">
    <ConfigurationBuilder 
        bind:configurationDefinition 
        bind:isAddConfigurationDisabled
        on:keyup={configurationChanged}
        on:click={addConfiguration}
    />
</div>

{#await configPromise}
    <h1>LOADING</h1>
{:then}
    {#each configurations as configuration}
        <ConfigurationDetails
            bind:configuration
            on:click={() => removeConfiguration(configuration)}
        />
    {/each}
{:catch error}
    <p>{error.message}</p>
{/await}

<style>
    .bordered {
    padding-bottom: 50px;
    }
</style>

<script lang="ts">
    import ConfigurationBuilder from '../components/ConfigurationBuilder.svelte';
    import ConfigurationDetails from '../components/ConfigurationDetails.svelte';

    let configurations:string[] = [];
    let configurationDefinition:string = '';
    let isAddConfigurationDisabled:boolean = true;

    let configPromise = getConfigurations();

    async function getConfigurations() : Promise<void> {
        const response = await fetch(`api/configuration/`);
        const text = await response.text();

        if (response.ok) {
            configurations = JSON.parse(text);
        } else {
            throw new Error(text);
        }
    }

    function configurationChanged() : void {
        isAddConfigurationDisabled = configurationDefinition.length < 1;
    }

    async function addConfiguration() : Promise<void> {
        isAddConfigurationDisabled = true;
        const response = await fetch(`api/configuration/`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({Configuration: configurationDefinition})
        });

        if (response.ok) {
            configurations = configurations.concat(configurationDefinition);
            configurationDefinition = '';
        } else {
            throw new Error(await response.text());
        }
    }

    async function removeConfiguration(configuration:string) : Promise<void> {
        const response = await fetch(`api/configuration/`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({configuration: configuration})
        });

        if (response.ok) {
            configurations = configurations.filter(config => config !== configuration);
        } else {
            throw new Error(await response.text());
        }
    }
</script>