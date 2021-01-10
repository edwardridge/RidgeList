// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add("login", (email, password) => { ... })
export {}
declare global {
    namespace Cypress {
        interface Chainable {
            getByCyName: typeof getByCyName;
            createWishlist : typeof createWishList;
        }
    }
}

export function getByCyName(cyName : string) {
    return cy.get(`[cypress-name="${cyName}"]`);
}

export function createWishList() {
    cy.request('POST', '/WishlistTest/createTestWishlist').then(response =>
    {
        cy.visit(`/wishlist/${response.body}`);
    });
}

Cypress.Commands.add("getByCyName", getByCyName);

Cypress.Commands.add("createWishlist", createWishList);
//
//
// -- This is a child command --
// Cypress.Commands.add("drag", { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add("dismiss", { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite("visit", (originalFn, url, options) => { ... })
