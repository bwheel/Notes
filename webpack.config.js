const path = require('path');
const CopyPlugin = require("copy-webpack-plugin");

module.exports = {
    entry: './Notes/static/index.js',
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, 'Notes/wwwroot/js')
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            },
            {
                test: /\.(png|svg|jpg|gif)$/,
                use: ['file-loader']
            }
        ]
    },
    plugins: [
        new CopyPlugin({
            patterns: [
                { from: "./Notes/static/images/favicon.ico", to: "../favicon.ico" },
            ],
        }),
    ],
};
