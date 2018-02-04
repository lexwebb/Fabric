import Ajv from 'ajv';

function createAjvAccessor() {
    return {
        get new() {
            const ajv = new Ajv({
                schemaId: 'auto',
            });
            /* eslint-disable global-require */
            ajv.addMetaSchema(require('ajv/lib/refs/json-schema-draft-06.json'));

            return ajv;
        },
    };
}

export default {
    /* eslint-disable no-param-reassign */
    install(Vue) {
        Vue.prototype.$ajv = createAjvAccessor();
    },
};
