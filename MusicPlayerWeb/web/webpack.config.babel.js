const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = () => ({
  entry: path.join(__dirname, 'Scripts/App/ReduxApp.jsx'),
  output: {
    path: path.join(__dirname, 'Scripts/Build'),
    filename: 'bundle.js',
  },
  module: {
    rules: [
      {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: [
              ['@babel/preset-env', { modules: false }],
              ['@babel/preset-react']
            ]
          }
        },
      },
      {
        test: /\.s?css$/i,
        use: [
          MiniCssExtractPlugin.loader,
          'css-loader',
          'postcss-loader',
          {
            loader: 'sass-loader',
            options: {
              implementation: require('sass'),
            },
          },
        ],
      },
      {
        test: /\.(eot|woff|woff2|ttf|svg|png|jpg|gif)$/,
        type: 'asset/resource',
      },
      {
        test: /\.js$/,
        include: /node_modules\/react-progress-circle/,
        type: 'javascript/auto',
        use: [
          {
            loader: 'babel-loader',
            options: {
              presets: [
                ['@babel/preset-env', { modules: false }],
                ['@babel/preset-react']
              ]
            }
          }
        ]
      }
    ],
  },
  plugins: [
    new HtmlWebpackPlugin({
      template: './Pages/index.html',
      filename: '../../Pages/index.html',
      inject: true,
    }),
    new MiniCssExtractPlugin({
      filename: 'App.css',
    }),
  ],
  devtool: 'source-map',
  resolve: {
    extensions: ['.js', '.jsx', '.scss', '.css'],
    alias: {
      'react-progress-circle': path.resolve(__dirname, 'node_modules/react-progress-circle/dist/all.min.js')
    }
  },
});
