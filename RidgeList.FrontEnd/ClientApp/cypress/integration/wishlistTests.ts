describe('Homepage', () => {
    it('creates new wishlist"', () => {
        cy.visit('/');

        getByCyName('CreateNewWishlist').click();
        let rand = Math.floor((Math.random() * 10000) + 1);
        let name = `From Cypress ${rand}`;
        getByCyName("NameOfWishlist").type(name);
        getByCyName('Create').click();
        cy.contains(name).click();
        cy.url().should('include', '/wishlist');
        
        cy.contains('Add Person').click();
        cy.contains('New person');
    })
})

let getByCyName = (name : string) => {
    return cy.get(`[cypress-name="${name}"]`);
}