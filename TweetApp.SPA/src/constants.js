export const CALLBACK_PATH = '/implicit/callback';
export const OKTA_DOMAIN = 'dev-669728.okta.com';
export const CLIENT_ID = '0oa4gpq1oyOsCygrt357';

export const ISSUER = `https://${OKTA_DOMAIN}/oauth2/default`;
export const HOST = window.location.host;
export const ORIGN = window.location.origin;
export const REDIRECT_URI_HTTP = `http://${HOST}${CALLBACK_PATH}`;
export const REDIRECT_URI_HTTPS = `https://${HOST}${CALLBACK_PATH}`;
export const SCOPES = 'openid profile email';

const config = {
  issuer: ISSUER,
  clientId: CLIENT_ID,
  redirectUri: window.location.href.indexOf('localhost') > -1 ? REDIRECT_URI_HTTP : REDIRECT_URI_HTTPS,
  scope: SCOPES.split(/\s+/),
};

console.log(config)

export default config;