const path = require("path")
const webpack = require("webpack")
const HtmlWebpackPlugin = require('html-webpack-plugin')
const MiniCssExtractPlugin = require('mini-css-extract-plugin')
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

var isGitPod = process.env.GITPOD_INSTANCE_ID !== undefined;

function getDevServerUrl() {
    if (isGitPod) {
        const url = execSync(`gp url 8080`);
        return url.toString().trim();
    } else {
        return `http://localhost:8080`;
    }
}

module.exports = (_env, options) => {

    var isDevelopment = options.mode === "development";

    return {
        entry: './fableBuild/App.js',
        mode: isDevelopment ? "development" : "production",
        output: {
            path: resolve('./output'),
            filename: isDevelopment ? '[name].js' : '[name].[fullhash].js',
        },
        devtool: isDevelopment ? 'eval-source-map' : false,
        optimization: {
            // Split the code coming from npm packages into a different file.
            // 3rd party dependencies change less often, let the browser cache them.
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
            },
            moduleIds: isDevelopment ? "named" : undefined,
        },
        plugins:
            [
                new HtmlWebpackPlugin({
                    filename: "./index.html",
                    template: "./index.html"
                }),
                new MiniCssExtractPlugin()
            ].filter(Boolean),
        devServer: {
            public: getDevServerUrl(),
            contentBase: resolve("public"),
            publicPath: "/",
            port: 8080,
            hot: true,
            inline: true,
            disableHostCheck: true
        },
        module: {
            rules: [
                {
                    test: /\.(sass|scss|css)$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        'css-loader',
                        {
                            loader: 'sass-loader',
                            options: {
                                // Prefer `dart-sass`
                                implementation: require('sass'),
                            }
                        },
                    ],
                },
                {
                    test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)$/,
                    use: ["file-loader"]
                }
            ]
        }
    }
}
