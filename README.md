# ShoppingList.Angular

A simple shopping list app built with Angular and Azure Static Web Apps. Uses Azure Functions for the API backend.

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 17.0.0.

## Running

Use the Static Web Apps CLI to run this code. Read more at https://azure.github.io/static-web-apps-cli/

Run `npx swa start` to run both the front and back end pieces, then navigate to http://localhost:4280. Note that any breakpoints
you set in the C# code will not be hit.

To allow debugging the API services and support hot reload, Open api.csproj in Visual Studio and start a debugging session. It is 
expected this will start the API on port 7135 (http://localhost:7135/).

Then run this command in a VSCode terminal:

`npx swa start http://localhost:4200 --run "npm start" --api-devserver-url http://localhost:7135/`

Or use the shortcut defined in package.json: `npm run start:swa-no-api`
