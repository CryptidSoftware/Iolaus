const {defaults} = require('jest-config');

module.exports = {
    "transform": {
        "^.+\\.js$": "babel-jest",
        "^.+\\.svelte$": [
            "svelte-jester",
            {
                "preprocess": true
            }
        ],
        "^.+\\.ts$": "ts-jest"
    },
    "moduleFileExtensions": [
        "js",
        "svelte",
        "ts"
    ],
    "setupFilesAfterEnv": [
        "@testing-library/jest-dom/extend-expect"
    ]
}