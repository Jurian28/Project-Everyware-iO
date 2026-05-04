module.exports = {
  env: {
    production: {
      plugins: ['react-native-reanimated/plugin'],
    },
    development: {
      plugins: ['react-native-reanimated/plugin'],
    },
  },
  presets: [
    ['@babel/preset-env', { targets: { browsers: ['last 2 versions'] } }],
    ['@babel/preset-react', { runtime: 'automatic' }],
    '@babel/preset-typescript',
  ],
  plugins: [
    'react-native-web',
  ],
};