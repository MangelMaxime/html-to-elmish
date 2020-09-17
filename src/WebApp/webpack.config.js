const path = require("path");
const webpack = require("webpack");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

var babelOptions = {
    presets: [
        ["@babel/preset-env", {
            "targets": {
                "browsers": ["last 2 versions"]
            },
            "modules": false,
            "useBuiltIns": "usage"
        }],
        "@babel/react"
    ],
    "plugins": [
        "@babel/plugin-transform-runtime",
        "@babel/plugin-proposal-class-properties"
    ]
};

var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: resolve('./output/index.html'),
        template: resolve('index.html')
    }),
    new MonacoWebpackPlugin({
        languages: [
            "fsharp",
            "html",
            "css",
            "javascript"
        ],
        features: [
            'accessibilityHelp',
            'bracketMatching',
            'caretOperations',
            'clipboard',
            'codelens',
            'colorDetector',
            'comment',
            'contextmenu',
            // 'coreCommands',
            'cursorUndo',
            // 'dnd',
            'find',
            // 'folding',
            // 'format',
            'gotoDeclarationCommands',
            'gotoDeclarationMouse',
            'gotoError',
            'gotoLine',
            'hover',
            'inPlaceReplace',
            'inspectTokens',
            // 'iPadShowKeyboard',
            'linesOperations',
            'links',
            'multicursor',
            'parameterHints',
            // 'quickCommand',
            // 'quickFixCommands',
            // 'quickOutline',
            // 'referenceSearch',
            // 'rename',
            'smartSelect',
            // 'snippets',
            'suggest',
            'toggleHighContrast',
            'toggleTabFocusMode',
            'transpose',
            'wordHighlighter',
            'wordOperations'
        ]
    })
];

module.exports = function(env, argv) {
    var isProduction = argv.mode == "production"
    console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

    return {
        devtool: false,
        mode: isProduction ? "production" : "development",
        entry: isProduction ? // We don't use the same entry for dev and production, to make HMR over style quicker for dev env
            {
                app: [
                    "@babel/polyfill",
                    resolve('HtmlToElmish.fsproj'),
                    resolve('sass/main.scss')
                ]
            } : {
                app: [
                    "@babel/polyfill",
                    resolve('HtmlToElmish.fsproj')
                ],
                style: [
                    resolve('sass/main.scss')
                ]
            },
        output: {
            path: resolve('./output'),
            filename: isProduction ? '[name].[hash].js' : '[name].js'
        },
        optimization: {
            splitChunks: {
                cacheGroups: {
                    commons: {
                        test: /[\\/]node_modules[\\/]/,
                        name: 'vendor',
                        chunks: 'all'
                    },
                    fable: {
                        test: /[\\/]fable-core[\\/]/,
                        name: 'fable',
                        chunks: 'all'
                    }
                }
            }
        },
        plugins: isProduction ?
            commonPlugins.concat([
                new MiniCssExtractPlugin({
                    filename: 'style.[hash].css'
                }),
                // new CopyWebpackPlugin([
                //     { from: './public' }
                // ]),
                // ensure that we get a production build of any dependencies
                // this is primarily for React, where this removes 179KB from the bundle
                new webpack.DefinePlugin({
                    'process.env.NODE_ENV': '"production"'
                })
            ])
            : commonPlugins.concat([
                new webpack.HotModuleReplacementPlugin(),
                new webpack.NamedModulesPlugin()
            ]),
        resolve: {
            modules: [
                "node_modules/",
                resolve("./../../node_modules/")
            ]
        },
        devServer: {
            contentBase: resolve('./public/'),
            publicPath: "/",
            port: 8080,
            hot: true,
            inline: true
        },
        module: {
            rules: [
                {
                    test: /\.fs(x|proj)?$/,
                    use: {
                        loader: "fable-loader",
                        options: {
                            babel: babelOptions,
                            define: isProduction ? [] : ["DEBUG"],
                            extra: { optimizeWatch: true }
                        }
                    }
                },
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: babelOptions
                    },
                },
                {
                    test:  /\.(sass|scss|css)$/,
                    use: [
                        isProduction ? MiniCssExtractPlugin.loader : 'style-loader',
                        'css-loader',
                        {
                            loader: 'sass-loader',
                            options: {
                            // Prefer `dart-sass`
                            implementation: require('sass'),
                            },
                        },
                    ],
                },
                {
                    test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*$|$)/,
                    use: ["file-loader"]
                }
            ]
        }
    }
};
