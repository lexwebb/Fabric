class RestServiceBase {
    constructor(vueInstance, resourceUrl) {
        this.$vue = vueInstance;
        this.apiUrlbase = this.$vue.prototype.$config.apiUrlbase;
        this.resourceUrl = this.$vue.prototype.$utils.trim(resourceUrl, '/');

        this.headers = new Headers();
        this.headers.append('Accept', 'application/json, text/plain, */*');
        this.headers.append('Content-Type', 'application/json');
    }

    get(resourceName) {
        const request = new Request(`${this.apiUrlbase}/${this.resourceUrl}/${resourceName === undefined ? '' : resourceName}`, {
            method: 'GET',
            headers: this.headers,
        });

        return RestServiceBase.doRequest(request);
    }

    put(resourceName, data) {
        const request = new Request(`${this.apiUrlbase}/${this.resourceUrl}/${resourceName}`, {
            method: 'PUT',
            headers: this.headers,
            body: data,
        });

        return RestServiceBase.doRequest(request);
    }

    post(resourceName, data) {
        const request = new Request(`${this.apiUrlbase}/${this.resourceUrl}/${resourceName}`, {
            method: 'POST',
            headers: this.headers,
            body: data,
        });

        return RestServiceBase.doRequest(request);
    }

    delete(resourceName) {
        const request = new Request(`${this.apiUrlbase}/${this.resourceUrl}/${resourceName}`, {
            method: 'DELETE',
            headers: this.headers,
        });

        return RestServiceBase.doRequest(request);
    }

    static doRequest(request) {
        return fetch(request)
            .then((response) => {
                if (!response.ok) {
                    throw Error(response.statusText);
                }
                return response.json();
            });
    }
}

export default RestServiceBase;
