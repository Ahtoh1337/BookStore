import Cookies from 'js-cookie'

async function authFetch(url, request) {
    const accessToken = Cookies.get('accessToken');
    if (accessToken) {
        const authHeader = 'Bearer ' + accessToken;
        request.headers = {...request.headers, Authorize: authHeader};
        console.log(authHeader);
    } else console.log('No accessToken');

    return fetch(url, request);
}

export default authFetch;