module.exports = {
  // https://eslint.org/docs/user-guide/configuring#configuration-cascading-and-hierarchy
  // This option interrupts the configuration hierarchy at this file
  // Remove this if you have an higher level ESLint config file (it usually happens into a monorepos)
  root: true,

  // https://eslint.vuejs.org/user-guide/#how-to-use-custom-parser
  // Must use parserOptions instead of "parser" to allow vue-eslint-parser to keep working
  // `parser: 'vue-eslint-parser'` is already included with any 'plugin:vue/**' config and should be omitted
  parserOptions: {

    parser: '@babel/eslint-parser',
    requireConfigFile: false,
    ecmaVersion: 2018, // Allows for the parsing of modern ECMAScript features
    sourceType: 'module', // Allows for the use of imports
  },

  env: {
    browser: true,
  },

  // Rules order is important, please avoid shuffling them
  extends: [
    // Base ESLint recommended rules
    // 'eslint:recommended',

    // Uncomment any of the lines below to choose desired strictness,
    // but leave only one uncommented!
    // See https://eslint.vuejs.org/rules/#available-rules
    'plugin:vue/vue3-essential', // Priority A: Essential (Error Prevention)
    'plugin:vue/vue3-strongly-recommended', // Priority B: Strongly Recommended (Improving Readability)
    'plugin:vue/vue3-recommended', // Priority C: Recommended (Minimizing Arbitrary Choices and Cognitive Overhead)

    'airbnb-base',

  ],

  plugins: [
    // https://eslint.vuejs.org/user-guide/#why-doesn-t-it-work-on-vue-file
    // required to lint *.vue files
    'vue',

  ],

  globals: {
    ga: 'readonly', // Google Analytics
    cordova: 'readonly',
    __statics: 'readonly',
    __QUASAR_SSR__: 'readonly',
    __QUASAR_SSR_SERVER__: 'readonly',
    __QUASAR_SSR_CLIENT__: 'readonly',
    __QUASAR_SSR_PWA__: 'readonly',
    process: 'readonly',
    Capacitor: 'readonly',
    chrome: 'readonly',
  },

  // add your custom rules here
  rules: {
    'no-console': ['error', { allow: ['warn', 'error', 'log'] }],
    'brace-style': ['error', '1tbs', { allowSingleLine: true }],
    'linebreak-style': 0,
    'no-param-reassign': 'off',
    'import/first': 'off',
    'no-nested-ternary': 'off',
    'import/extensions': 'off',
    'import/no-unresolved': 'off',
    'vue/attribute-hyphenation': 'off',
    'import/no-extraneous-dependencies': 'off',
    'import/prefer-default-export': 'off',
    'prefer-promise-reject-errors': 'off',
    'prefer-destructuring': 'off',
    'guard-for-in': 'off',
    'no-restricted-syntax': 'off',
    'no-plusplus': 'off',
    'spaced-comment': 'off',
    'no-trailing-spaces': 'off',
    'space-unary-ops': 'off',
    'no-throw-literal': 'off',
    'import/named': 'off',
    'import/namespace': 'error',
    'import/default': 'error',
    'import/export': 'error',
    'no-unused-vars': 'warn',
    'max-len': ['warn', { code: 180 }],
    'space-in-parens': 'warn',
    'object-curly-newline': 'off',
    'vue/max-attributes-per-line': ['warn', {
      singleline: 3,
      multiline: 1,
    }],
    'no-empty': 'off',
    'no-useless-return': 'warn',
    'no-unreachable': 'warn',
    'dot-notation': 'warn',
    'vue/no-unused-components': 'warn',
    'no-multiple-empty-lines': 'warn',
    'keyword-spacing': 'warn',
    'object-curly-spacing': 'warn',
    'vue/no-unused-vars': 'warn',
    'no-bitwise': 'off',
    'no-return-await': 'off',
    'no-await-in-loop': 'off',
    'no-else-return': 'off',
    'no-case-declarations': 'off',
    'vue/singleline-html-element-content-newline': 'off',
    // allow debugger during development only
    'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',
    'no-continue': 'off',
    'no-loop-func': 'off',
    'arrow-parens': 'off',
    // TODO new from update (review later if it's ok to turn off)
    'function-paren-newline': 'off',
    'vue/multi-word-component-names': 'off',
    'vue/no-v-text-v-html-on-component': 'off',
    'no-promise-executor-return': 'off',
    'no-unsafe-optional-chaining': 'off',
    'prefer-regex-literals': 'off',
    'function-call-argument-newline': 'off',
    'vue/v-on-event-hyphenation': 'off',
    'no-use-before-define': 'off',
    'no-underscore-dangle': 'off',
  },
  overrides: [
    {
      files: ['./src/components/base/*'],
      rules: {
        'vue/component-definition-name-casing': ['off', 'pascal-case', 'off', 'kebab-case'],
      },
    },
    {
      files: ['./src/i18n/**'],
      rules: {
        'max-len': ['off'],
      },
    },
  ],
};
