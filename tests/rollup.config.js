import * as path from 'path';
import fable from 'rollup-plugin-fable';
import serve from 'rollup-plugin-serve'
import livereload from 'rollup-plugin-livereload'

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

export default {
    input: resolve('./Tests.fsproj'),
    output: {
        file: resolve('./dist/tests.bundle.js'),
        format: 'umd', // 'amd', 'cjs', 'es', 'iife', 'umd',
        name: 'FableREPL',
        sourcemap: true
    },
    plugins: [
        serve({
            contentBase: ['tests/static', 'tests/dist'],
            port: 8080
        }),
        livereload(),
        fable({})
    ],
};
