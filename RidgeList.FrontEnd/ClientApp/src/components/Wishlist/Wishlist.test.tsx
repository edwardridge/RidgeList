import React from "react";
import { render, unmountComponentAtNode } from "react-dom";
import { act } from "react-dom/test-utils";
import {default as Wishlist} from "./Wishlist";
import { mocked } from 'ts-jest/utils';
import {WishlistClient, WishlistModel} from "../../nswag/api.generated";
import {MemoryRouter} from "react-router-dom";
import {IWishlistRepository, WishlistRepository} from "./IWishlistRepository";

let container : Element;
beforeEach(() => {
    // setup a DOM element as a render target
    container = document.createElement("div");
    document.body.appendChild(container);
});

afterEach(() => {
    // cleanup on exiting
    unmountComponentAtNode(container);
    container.remove();
    container = {} as Element;
});

it("renders the name correctly", async () => {
    let mockRepository = new MockWishlistRepository();
    mockRepository.setWishlist({id: "123", name: "Fake"});
    
    await act(async () => {
        render(<MemoryRouter initialEntries={["/wishlist/234"]}> <Wishlist wishlistRepository={mockRepository}/></MemoryRouter>, container);
    });

    expect(container.querySelector("#wishlistTitle")?.textContent).toBe("Wishlist - Fake");
});

class MockWishlistRepository implements IWishlistRepository{
    private wishlist : WishlistModel = {} as WishlistModel;
    
    getWishlist = (id: string): Promise<WishlistModel> => {
        return Promise.resolve(this.wishlist);
    }
    
    setWishlist = (wishlist : WishlistModel) => {
        this.wishlist = wishlist;
    }
}