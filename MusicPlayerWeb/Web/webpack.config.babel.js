import path from 'path';  
import HtmlWebpackPlugin from 'html-webpack-plugin';
import ExtractTextPlugin from 'extract-text-webpack-plugin';

export default () => ({  
    entry: [
      path.join(__dirname, 'Scripts/App/ReduxApp.jsx'),
    ],
    output: {
        path: path.join(__dirname, '../bin/x64/Debug/Web/Scripts/Build'),
        filename: 'bundle.js',
    },
    module: {
        rules: [
          {
              test: /.jsx?$/,
              exclude: /node_modules/,
              include: path.join(__dirname, 'Scripts/App'),
              use: [
                {
                    loader: 'babel-loader',
                    options: {
                        babelrc: false,
                        //plugins: ['transform-runtime'],
                        presets: [
                          ['es2015', { modules: false }],
                          ['react']
                        ],
                    }
                }
              ]
          },
          {
              test: /\.(scss|sass)$/,
              use: ExtractTextPlugin.extract({
                  //resolve-url-loader may be chained before sass-loader if necessary
                  use: ['css-loader', 'sass-loader']
              })
          },
          {
              test: /\.(eot|woff|woff2|ttf|svg|png|jpg|gif)$/,
              loader: 'url-loader?limit=30000&name=[name]-[hash].[ext]'
          },
        ]
    },
    plugins: [
      new HtmlWebpackPlugin({
          filename: 'index.html',
          template: './Pages/index.html'
      }),
      new ExtractTextPlugin('App.css')
    ],
});
