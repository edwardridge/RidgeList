import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import React from "react";

interface OtherPersonWishlistRowProps{
    wishlistPerson : WishlistPersonModel;
    loggedInEmail : string;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

export const OtherPersonWishlistRow = (props : OtherPersonWishlistRowProps) => {
    let claimPresentClick = async (presentId : string) => {
        let wishlist = await new WishlistClient()
            .claimPresent(props.wishlistId, props.loggedInEmail, presentId);
        props.setWishlist(wishlist);
    }

    let unclaimPresentClick = async (presentId : string) => {
        let wishlist = await new WishlistClient()
            .unclaimPresent(props.wishlistId, presentId);
        props.setWishlist(wishlist);
    }

    return (
        <div className='wishlistSummaryItem' key={`${props.wishlistPerson.email}`}>
            <div>
                <span className='d-inline-block col-10'>
                    {props.wishlistPerson.name}
                    <span className='emailDetails'> ({props.wishlistPerson.email})</span>
                </span>
            </div>
            <div className='personItems'>
                {props.wishlistPerson.presentIdeas?.map(s => {
                    let claimed = s.claimerEmail !== null && s.claimerEmail !== '';
                    let claimedByYou = s.claimerEmail === props.loggedInEmail;
                    let unclaim = claimedByYou ? <button className='btn btn-success' onClick={() => unclaimPresentClick(s.id)}>Oops, I won't get this</button> : null;
                    let claimSection = s.claimerName ?
                        <span> - claimed by {claimedByYou ? "you" : s.claimerName} {unclaim}</span> :
                        <button className='btn btn-success' onClick={() => claimPresentClick(s.id)}>I'll get this!</button>;

                    return (
                        <div key={s.id} className={claimed && !claimedByYou ? 'claimed' : ''}>
                            <span>{s.description}</span>
                            {claimSection}
                        </div>
                    )
                })}
            </div>
        </div>
    )
}