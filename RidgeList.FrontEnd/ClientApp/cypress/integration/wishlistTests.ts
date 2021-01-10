describe('Homepage', () => {
    beforeEach(() => {
        cy.visit('/');
    })
    
    it('creates new wishlist"', () => {
        cy.getByCyName('CreateNewWishlist').click();
        let name = createRandomName();
        
        cy.getByCyName("NameOfWishlist").type(name);
        cy.getByCyName('Create').click();
        cy.url().should('include', '/wishlist');
        
        cy.getByCyName('AddPerson').click();
        cy.getByCyName('NewPersonName').type('Edward Ridge');
        cy.getByCyName('CreateNewPerson').click();
        cy.contains('Edward Ridge');
    });
    
    let createRandomName = () => {
        let rand = Math.floor((Math.random() * 10000) + 1);
        return `From Cypress ${rand}`;
    }
});