var path = require("path");
var webpack = require("webpack");


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
    ],
    "plugins": [
        "@babel/plugin-transform-runtime",
    ]
};

var isProduction = process.argv.indexOf("-p") >= 0;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

module.exports = function(env, argv) {
    var isProduction = argv.mode == "production"
    console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

    return {
        devtool: "source-map",
        entry: resolve('./Tests.fsproj'),
        mode: isProduction ? "production" : "development",
        output: {
            filename: 'bundle.js',
            path: resolve('./dist/'),
        },
        devServer: {
            contentBase: [
                resolve('./static'),
                resolve('./dist'),
            ],
            port: 8080
        },
        module: {
            rules: [
                {
                    test: /\.fs(x|proj)?$/,
                    use: {
                        loader: "fable-loader",
                        options: {
                            babel: babelOptions,
                            define: isProduction ? [] : ["DEBUG"]
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
                }
            ]
        }
    }
};
