module.exports = {
    extends: [
        // add more generic rulesets here, such as:
        // 'eslint:recommended',
        'airbnb-base',
        'plugin:vue/essential'
    ],
    rules: {
        // override/add rules settings here, such as:
        // 'vue/no-unused-vars': 'error'
        'indent': ["error", 4],
        'linebreak-style': 0
    }
}
