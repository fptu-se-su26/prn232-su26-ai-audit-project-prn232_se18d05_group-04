/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/Admin/**/*.cshtml",
    "./wwwroot/js/admin/**/*.js"
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ["Be Vietnam Pro", "Segoe UI", "Arial", "sans-serif"],
        mono: ["SFMono-Regular", "Consolas", "monospace"]
      }
    }
  },
  plugins: []
};