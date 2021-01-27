import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import React from "react";
import {useWishlistClient} from "./useWishlistClient";

interface OtherPersonWishlistRowProps{
    wishlistPerson : WishlistPersonModel;
    loggedInEmail : string;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

export const OtherPersonWishlistRow = (props : OtherPersonWishlistRowProps) => {
    let wishlistClient = useWishlistClient();
    
    let claimPresentClick = async (presentId : string) => {
        let wishlist = await wishlistClient
            .claimPresent(props.wishlistId, props.loggedInEmail, presentId);
        props.setWishlist(wishlist);
    }

    let unclaimPresentClick = async (presentId : string) => {
        let wishlist = await wishlistClient
            .unclaimPresent(props.wishlistId, presentId);
        props.setWishlist(wishlist);
    }

    return (
        <div className='wishlistSummaryItem mb-3' key={`${props.wishlistPerson.email}`}>
            <div>
                <span className='d-inline-block col-12 text-center'>
                    {props.wishlistPerson.name}
                    <span className='emailDetails'> ({props.wishlistPerson.email})</span>
                </span>
            </div>
            <table className='table'>
                {props.wishlistPerson.presentIdeas?.map(s => {
                    let claimed = s.claimerEmail !== null && s.claimerEmail !== '';
                    let claimedByYou = s.claimerEmail === props.loggedInEmail;
                    let unclaim = claimedByYou ? <button className='btn btn-outline-danger w-100' onClick={() => unclaimPresentClick(s.id)}>Unclaim</button> : null;
                    let claimerText = claimed ? `- claimed by ${claimedByYou ? "you" : s.claimerName}` : '';
                    let claimSection = s.claimerName ?
                        <>{unclaim}</> :
                        <button className='btn btn-outline-success w-100' onClick={() => claimPresentClick(s.id)}>Claim</button>;

                    let classes = `mt-1 ml-0 mr-0 row ${claimed ? 'claimed' : ''}`;
                    return (
                        <tr key={s.id} className={classes}>
                            <td className='col-8 col-md-10' cypress-name={`${s.id}-description`}>{s.description} {claimerText}</td>
                            <td className='col-4 col-md-2 text-right' cypress-name={`${s.id}-buttons`}>{claimSection}</td>
                        </tr>
                    ) 
                })}
            </table>
        </div>
    )
}