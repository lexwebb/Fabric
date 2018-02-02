module.exports = {
    parserOptions: {
        "parser": "babel-eslint",
        "ecmaVersion": 2017,
        "sourceType": "module"
    },
    env: {
        "browser": true
    },
    extends: [
        // add more generic rulesets here, such as:
        // 'eslint:recommended',
        'airbnb-base'
    ],
    plugins: [
        'html'
    ],
    rules: {
        // override/add rules settings here, such as:
        // 'vue/no-unused-vars': 'error'
        'indent': ["error", 4],
        'linebreak-style': 0,
        'no-debugger': 0,
        'prefer-destructuring': 0,
    }
}
