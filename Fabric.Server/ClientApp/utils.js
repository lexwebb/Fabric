function unCamelCase(input) {
    return (
        input
            // insert a space between lower & upper
            .replace(/([a-z])([A-Z])/g, '$1 $2')
            // space before last upper in a sequence followed by lower
            .replace(/\b([A-Z]+)([A-Z])([a-z])/, '$1 $2$3')
            // uppercase the first character
            .replace(/^./, str => str.toUpperCase())
    );
}

function trimLeft(input, charlist) {
    let newCharList = charlist;
    if (charlist === undefined) {
        /* eslint-disable no-useless-escape */
        newCharList = 's';
    }

    return input.replace(new RegExp(`^(${newCharList})`), '');
}

function trimRight(input, charlist) {
    let newCharList = charlist;
    if (charlist === undefined) {
        /* eslint-disable no-useless-escape */
        newCharList = 's';
    }

    return input.replace(new RegExp(`(${newCharList})$`), '');
}

function trim(input, charlist) {
    let newCharList = charlist;
    if (charlist === undefined) {
        /* eslint-disable no-useless-escape */
        newCharList = 's';
    }

    return input.replace(new RegExp(`^(${newCharList})|(${newCharList})$`), '');
}

const utils = {
    unCamelCase,
    trimLeft,
    trimRight,
    trim,
};

export default {
    utils,
    /* eslint-disable no-param-reassign */
    install(Vue) {
        Vue.prototype.$utils = utils;
    },
};

export { utils };
