import {WishlistClient, WishlistModel} from "../../nswag/api.generated";

export interface IWishlistRepository{
    getWishlist(id: string) : Promise<WishlistModel>;
}

export class WishlistRepository implements IWishlistRepository{
    getWishlist(id: string): Promise<WishlistModel> {
        return new WishlistClient().getWishlist(id);
    }
    
}