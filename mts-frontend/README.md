# mts-frontend

This project contains the [Vue.js](https://vuejs.org/)-based web frontend of OpenMTS.

## Scripts

- `npm install`: Install dependencies.
- `npm run serve`: Compile and host with hot-reloading for development.
- `npm run build`: Build for production.

## Formatting

This project includes a Prettier configuration file. Add Prettier to your editor (eg. [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode) for VS Code) and enable formatting-on-save.

## Configuring for Production

### API Endpoint

When building for production, set the backend API endpoint in the `.env` file:

```
VUE_APP_SERVER_ENDPOINT=http://localhost:5000
```

### Public Path

If the frontend isn't hosted on its host's web server root, set the public path in the `vue.config.js` file when building. If for example, the frontend is hosted at `http://192.168.0.55/openmts-frontend`, set the public path to `./openmts-frontend`:

```js
module.exports = {
  publicPath:
    process.env.NODE_ENV === "production" ? "./openmts-frontend" : "/",
};
```