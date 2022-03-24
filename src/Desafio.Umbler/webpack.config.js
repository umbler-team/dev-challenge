const path = require('path')

const prod = process.argv.indexOf('-p') !== -1

const config = {
  entry: {
    site: './src/js/app'
  },
  output: {
    path: path.join(__dirname, './wwwroot/js'),
    filename: '[name].js'
  },
  devtool: prod ? 'cheap-module-source-map' : 'cheap-module-eval-source-map',
  module: {
    rules: [{ test: /\.(js|jsx)$/, include: path.join(__dirname, 'src/js'), use: 'babel-loader' }]
  },
  externals: {
    jquery: 'jQuery'
  }
}

module.exports = config
