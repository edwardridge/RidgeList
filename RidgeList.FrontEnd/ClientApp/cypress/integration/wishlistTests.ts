import {LoginDetails} from "../../src/components/useLogin";


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

let setLoginCookie = (email : string, name: string) => {
    let loginDetails = new LoginDetails(email, name);
    return cy.setCookie('login', JSON.stringify(loginDetails));
}

describe('Wishlist page', () => {
    beforeEach(() => {
        setLoginCookie(emailAddress, "Test");
    });
    
    it('allows new people to be added', () => {
        cy.createWishlist('allows new people to be added');

        addNewPerson('ed@ed.com', 'Ed 2');
        
        cy.contains('Ed 2');
    });

    it('doesnt allow duplicate person to be added', () => {
        cy.createWishlist('doesnt allow duplicate person to be added');

        addNewPerson('ed@ed.com', 'Ed 2');

        cy.getByCyName('AddNewPerson').click();
        cy.getByCyName('NewPersonEmail').type('ed@ed.com');
        cy.getByCyName('CreateNewPerson').should('be.disabled');
        cy.getByCyName('NewPersonEmail').type('ed_diff@ed.com');
        cy.getByCyName('CreateNewPerson').should('be.enabled');
    });

    it('allows present idea to be added', () => {
        cy.createWishlist('allows present idea to be added');

        addNewPerson('ed@ed.com', 'Ed 2');

        addItem('New present idea');
        cy.contains('New present idea');
    });

    it('allows you to claim and unclaim someone elses present', () => {
        // cy.pause();
        cy.createWishlist('allows you to claim and unclaim someone elses present');

        addNewPerson('ed@ed.com', 'Ed 2');
        addItem('New present idea');
        
        setLoginCookie('ed@ed.com', "Ed 2");
        cy.visit('/').contains('allows you to claim and unclaim someone elses present').click();

        cy.contains('New present idea').next('td').click();
        cy.contains('New present idea').parent().within(() => cy.contains('Unclaim'));
    });
    
    let addItem = (description : string) => {
        cy.getByCyName('AddNewItemButton').click().getByCyName('AddItem').type(description);
        cy.getByCyName('SaveItemButton').click();
    }
    
    let addNewPerson = (email : string, name : string) => {
        return cy.getByCyName('AddNewPerson')
            .click()
            .getByCyName('NewPersonEmail')
            .type(email)
            .getByCyName('NewPersonName')
            .type(name)
            .getByCyName('CreateNewPerson')
            .click();
    }
});



let createRandomName = () => {
    let rand = Math.floor((Math.random() * 10000) + 1);
    return `[Test] From Cypress ${rand}`;
}