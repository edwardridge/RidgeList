before(() => {
    cy.deleteOldTestWishlists();
});

const emailAddress = "test@testwishlist.com";

describe('Login', () => {
    beforeEach(() => {
        cy.clearCookie('email').visit('/');
    })

    it('allows you to login using your email address', () => {
        cy.getByCyName('EmailLogin').type(emailAddress);
        cy.getByCyName('LoginButton').click();
        cy.url().should('include', '/wishlists');
    });
});

describe('Wishlist summary page', () => {
    beforeEach(() => {
        cy.setCookie('email', emailAddress).visit('/wishlists');
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
        cy.setCookie('email', emailAddress);
    });
    
    it('allows new people to be added', () => {
        cy.createWishlist();

        addNewPerson('ed@ed.com', 'Ed 2');
        
        cy.contains('Ed 2');
        cy.getByCyName('ListOfPeople').within((a) => {
            cy.get('.wishlistSummaryItem').should('have.length', 3);
        });
    });

    it('doesnt allow duplicate to be added', () => {
        cy.createWishlist();

        addNewPerson('ed@ed.com', 'Ed 2');
        
        cy.getByCyName('NewPersonEmail').type('ed@ed.com');
        cy.getByCyName('CreateNewPerson').should('be.disabled');
        cy.getByCyName('NewPersonEmail').type('ed_diff@ed.com');
        cy.getByCyName('CreateNewPerson').should('be.enabled');
    });
    
    let addNewPerson = (email : string, name : string) => {
        cy.getByCyName("AddNewPerson").click();
        cy.getByCyName('NewPersonEmail').type(email);
        cy.getByCyName('NewPersonName').type(name);
        cy.getByCyName('CreateNewPerson').click();
    }
});



let createRandomName = () => {
    let rand = Math.floor((Math.random() * 10000) + 1);
    return `[Test] From Cypress ${rand}`;
}