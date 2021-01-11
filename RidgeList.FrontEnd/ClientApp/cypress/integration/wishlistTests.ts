before(() => {
    cy.deleteOldTestWishlists();
});

describe('Login', () => {
    beforeEach(() => {
        cy.clearCookie('email').visit('/');
    })

    it('allows you to login using your email address', () => {
        cy.getByCyName('EmailLogin').type('edwardridge@gmail.com');
        cy.getByCyName('LoginButton').click();
        cy.url().should('include', '/wishlists');
    });
});

describe('Wishlist summary page', () => {
    beforeEach(() => {
        cy.setCookie('email', 'edwardridge@gmail.com').visit('/wishlists');
    });

    it('can create new wishlist"', () => {
        cy.getByCyName('CreateNewWishlist').click();

        let name = createRandomName();

        cy.getByCyName("NameOfWishlist").type(name);
        cy.getByCyName('Create').click();
        cy.url().should('include', '/wishlist');
    });
});

describe('Wishlist page', () => {
    beforeEach(() => {
        cy.setCookie('email', 'edwardridge@gmail.com');
    });
    
    it('allows names to be added', () => {
        cy.createWishlist();

        addNewPerson('Edward Ridge');
        
        cy.contains('Edward Ridge');
        cy.getByCyName('ListOfPeople').within((a) => {
            cy.get('.wishlistSummaryItem').should('have.length', 2);
        });
    });

    it('doesnt allow duplicate to be added', () => {
        cy.createWishlist();

        addNewPerson('Edward Ridge');
        cy.getByCyName('NewPersonName').type('Edward Ridge');
        cy.getByCyName('CreateNewPerson').should('be.disabled');
        cy.getByCyName('NewPersonName').type('2');
        cy.getByCyName('CreateNewPerson').should('be.enabled');
    });
    
    let addNewPerson = (email : string) => {
        cy.getByCyName('NewPersonName').type(email);
        cy.getByCyName('CreateNewPerson').click();
    }
});



let createRandomName = () => {
    let rand = Math.floor((Math.random() * 10000) + 1);
    return `[Test] From Cypress ${rand}`;
}