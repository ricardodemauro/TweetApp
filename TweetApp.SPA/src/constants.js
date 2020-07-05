export const CALLBACK_PATH = '/implicit/callback';
export const OKTA_DOMAIN = 'dev-669728.okta.com';
export const CLIENT_ID = '0oa4gpq1oyOsCygrt357';

export const ISSUER = `https://${OKTA_DOMAIN}/oauth2/default`;
export const HOST = window.location.host;
export const SCOPES = 'openid profile email';

const REDIRECT_URI_HTTP = `http://${HOST}${CALLBACK_PATH}`;
const REDIRECT_URI_HTTPS = `https://${HOST}${CALLBACK_PATH}`;

const localApi = 'http://localhost:7071/api/Tweet/';
const azApi = 'https://tweetappapi0033.azurewebsites.net/api/tweet/';

const redirectUri = () => {
  if(window.location.href.indexOf('github.io') > -1) {
    return `https://ricardodemauro.github.io/TweetApp${CALLBACK_PATH}`;
  }
  else if(window.location.protocol.indexOf('https') > -1) {
    return REDIRECT_URI_HTTPS;
  }
  else {
    return REDIRECT_URI_HTTP;
  }
}

export const apiUri = () => {
  if (window.location.href.indexOf('localhost') > -1) {
    return localApi;
  }
  return azApi;
}

const config = {
  issuer: ISSUER,
  clientId: CLIENT_ID,
  redirectUri: redirectUri(),
  scope: SCOPES.split(/\s+/),
};

console.log(config)

export default config;