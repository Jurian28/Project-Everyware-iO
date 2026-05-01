const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const webpack = require('webpack');

module.exports = {
    entry: path.resolve(__dirname, 'index.web.js'),
    module: {
        rules: [
            {
                test: /\.m?js$/,
                resolve: {
                    fullySpecified: false,
                },
            },
            {
                test: /\.(js|jsx|ts|tsx)$/,
                exclude: /node_modules\/(?!react-native-reanimated|react-native-gesture-handler|react-native-drawer-layout)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        cacheDirectory: true,
                        configFile: false,  // ← ignore babel.config.js entirely
                        presets: [
                            ['@babel/preset-env', { targets: { browsers: ['last 2 versions'] } }],
                            ['@babel/preset-react', { runtime: 'automatic' }],
                            '@babel/preset-typescript',
                        ],
                        plugins: [
                            'react-native-web',
                            'react-native-reanimated/plugin',
                        ],
                    },
                },
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg|ico|webp)$/,
                type: 'asset/resource',
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/,
                type: 'asset/resource',
            },
        ],
    },
    resolve: {
        extensionAlias: {
            '.js': ['.web.js', '.js'],
        },
        alias: {
            'react-native$': 'react-native-web',
            'react-native-keychain': path.resolve(__dirname, 'src/web-stubs/keychain.js'),
            'react-native-worklets': false,
            '@react-native-masked-view/masked-view': false,
            '@react-navigation/elements/lib/module/MaskedViewNative': path.resolve(
                __dirname,
                'node_modules/@react-navigation/elements/lib/module/MaskedView.js'
            ),
        },
        extensions: ['.web.tsx', '.web.ts', '.web.js', '.tsx', '.ts', '.js'],
        conditionNames: ['browser', 'require', 'default'],
        fallback: {
            process: require.resolve('process/browser'),
        },
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: path.resolve(__dirname, 'public/index.html'),
        }),
        new webpack.DefinePlugin({
            __DEV__: JSON.stringify(process.env.NODE_ENV !== 'production'),
            __BUNDLE_START_TIME__: JSON.stringify(Date.now()),  // ← fixed, was Date.Now (link)
            __VERSION__: JSON.stringify('1.0.0'),
            'process.env': JSON.stringify(process.env),
            'process.env.NODE_ENV': JSON.stringify(process.env.NODE_ENV || 'development'),
        }),
        new webpack.NormalModuleReplacementPlugin(
            /^\.\/useBackButton$/,
            path.resolve(__dirname, 'src/web-stubs/useBackButton.js')
        ),
        new webpack.NormalModuleReplacementPlugin(
            /^\.\/useDocumentTitle$/,
            path.resolve(__dirname, 'src/web-stubs/useDocumentTitle.js')
        ),
        new webpack.NormalModuleReplacementPlugin(
            /^\.\/useLinking$/,
            path.resolve(__dirname, 'src/web-stubs/useLinking.js')
        ),
        new webpack.NormalModuleReplacementPlugin(
            /addCancelListener/,
            path.resolve(__dirname, 'src/web-stubs/addCancelListener.js')
        ),
        new webpack.NormalModuleReplacementPlugin(
            /MaskedView/,
            (resource) => {
                if (resource.request === '../MaskedView') {
                    resource.request = path.resolve(
                        __dirname,
                        'node_modules/@react-navigation/elements/lib/module/MaskedView.js'
                    );
                }
            }
        ),
        new HtmlWebpackPlugin({
            template: path.resolve(__dirname, 'public/index.html'),
            favicon: path.resolve(__dirname, 'public/favicon.ico'),
        }),
    ],
    devServer: {
        port: 8004,
        historyApiFallback: true,
        client: {
            webSocketURL: 'ws://localhost:8004/ws',  // ← explicit WebSocket URL
        },
    },
};