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
                    let unclaim = claimedByYou ? <button className='btn btn-outline-danger w-100' onClick={() => unclaimPresentClick(s.id)}>Unclaim</button> : null;
                    let claimerText = claimed ? `- claimed by ${claimedByYou ? "you" : s.claimerName}` : '';
                    let claimSection = s.claimerName ?
                        <>{unclaim}</> :
                        <button className='btn btn-outline-success w-100' onClick={() => claimPresentClick(s.id)}>Claim</button>;

                    let classes = `mt-lg-1 row ${claimed && !claimedByYou ? 'claimed' : ''}`;
                    return (
                        <div key={s.id} className={classes}>
                            <span className='col-8 col-md-10'>{s.description} {claimerText}</span>
                            <span className='col-4 col-md-2 text-right'>{claimSection}</span>
                        </div>
                    )
                })}
            </div>
        </div>
    )
}