/**@type{import("snowpack").SnowpackUserConfig} */

module.exports = {
    mount: {
        '../wwwroot': {url: '/', static:true},
        src: {url: '/dist'},
    },
    plugins: [
        '@snowpack/plugin-svelte'
    ],
    routes: [
        {"match": "routes", "src": ".*", "dest": "/index.html"}
    ],
    optimize: {

    },
    packageOptions: {

    },
    devOptions: {
        port: 35729,
        output: 'stream',
        open: 'none'
    },
    buildOptions: {

    }
};