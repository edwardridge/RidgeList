// import {WishlistTestClient} from "../../src/nswag/api.generated"

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
    });
    
    it('allows names to be added', () => {
        cy.createWishlist();

        addNewPerson('Edward Ridge');
        
        cy.contains('Edward Ridge');
        cy.getByCyName('ListOfPeople').within((a) => {
            cy.get('li').should('have.length', 1);
        });
    });

    it('doesnt allow duplicate to be added', () => {
        cy.createWishlist();

        addNewPerson('Edward Ridge');
        addNewPerson('Edward Ridge');
        
        cy.contains('Edward Ridge');
        cy.getByCyName('ListOfPeople').within((a) => {
            cy.get('li').should('have.length', 1);
        });
    });
    
    let addNewPerson = (email : string) => {
        cy.getByCyName('AddPerson').click();
        cy.getByCyName('NewPersonName').type(email);
        cy.getByCyName('CreateNewPerson').click();
    }
    
    let createRandomName = () => {
        let rand = Math.floor((Math.random() * 10000) + 1);
        return `From Cypress ${rand}`;
    }
});