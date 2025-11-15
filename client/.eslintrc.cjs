module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  extends: [
    'eslint:recommended',
    'plugin:react/recommended',
    'plugin:react/jsx-runtime',
    'plugin:react-hooks/recommended',
  ],
  ignorePatterns: ['dist', '.eslintrc.cjs'],
  parserOptions: { ecmaVersion: 'latest', sourceType: 'module' },
  settings: { react: { version: '18.2' } },
  plugins: ['react-refresh'],
  rules: {
    'react-refresh/only-export-components': [
      'warn',
      { allowConstantExport: true },
    ],
    // Enforce template literals instead of string concatenation
    'prefer-template': 'error',
    // Disallow unnecessary template literals
    'no-useless-concat': 'error',
    // Enforce consistent use of backticks for template literals
    'quotes': ['error', 'single', {
      'avoidEscape': true,
      'allowTemplateLiterals': false
    }],
    // Disallow template literals without expressions
    'no-template-curly-in-string': 'error',
  },
}
