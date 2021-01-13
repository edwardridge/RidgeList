import {LoginDetails} from "../../src/components/useGetLogin";


before(() => {
    cy.deleteOldTestWishlists();
});

const emailAddress = "test@testwishlist.com";

describe('Login', () => {
    beforeEach(() => {
        cy.clearCookie('login').visit('/');
    })

    it('allows you to login using your email address', () => {
        cy.getByCyName('EmailLogin')
                .type(emailAddress)
            .getByCyName('NameLogin')
                .type('Test')
            .getByCyName('LoginButton')
                .click();
        
        cy.url().should('include', '/wishlists');
    });

    it('returns you to login page if you dont have a cookie', () => {
        cy.getByCyName('EmailLogin')
            .type(emailAddress)
            .getByCyName('NameLogin')
            .type('Test')
            .getByCyName('LoginButton')
            .click();

        cy.clearCookie('login')
            .visit('/wishlists');
        
        cy.url().should('not.include', '/wishlists');
    });
});

describe('Wishlist summary page', () => {
    beforeEach(() => {
        let loginDetails = new LoginDetails(emailAddress, "Test");
        cy.setCookie('login', JSON.stringify(loginDetails)).visit('/wishlists');
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
        let loginDetails = new LoginDetails(emailAddress, "Test");
        cy.setCookie('login', JSON.stringify(loginDetails));
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
        
        cy.getByCyName('NewPersonEmail').type('ed@ed.com').pause();
        cy.getByCyName('CreateNewPerson').should('be.disabled');
        cy.getByCyName('NewPersonEmail').type('ed_diff@ed.com');
        cy.getByCyName('CreateNewPerson').should('be.enabled');
    });
    
    let addNewPerson = (email : string, name : string) => {
        // cy.getByCyName("AddNewPerson").click();
        cy.getByCyName('NewPersonEmail').type(email);
        cy.getByCyName('NewPersonName').type(name);
        cy.getByCyName('CreateNewPerson').click();
    }
});



let createRandomName = () => {
    let rand = Math.floor((Math.random() * 10000) + 1);
    return `[Test] From Cypress ${rand}`;
}