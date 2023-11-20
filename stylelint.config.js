module.exports = {
  extends: ['stylelint-config-recommended'],
  overrides: [
    {
      files: ["**/*.scss"],
      customSyntax: "postcss-scss"
    }
  ],
  rules: {
    "at-rule-no-unknown": [
      true,
      {
        ignoreAtRules: [
          "apply",
          "include",
          "responsive",
          "screen",
          "tailwind",
          "use",
          "variants",
        ],
      },
    ],
    "declaration-block-trailing-semicolon": null,
    "no-descending-specificity": null,
    "no-empty-source": null,
    "unit-disallowed-list": ["px"]
  },
};
