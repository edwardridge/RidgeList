import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import React from "react";
import {useWishlistClient} from "./useWishlistClient";
import Linkify from "react-linkify";

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
            .claimGift(props.wishlistId, props.loggedInEmail, presentId);
        props.setWishlist(wishlist);
    }

    let unclaimPresentClick = async (presentId : string) => {
        let wishlist = await wishlistClient
            .unclaimGift(props.wishlistId, presentId);
        props.setWishlist(wishlist);
    }
    let getPresentIdeas = () => {
        if(props.wishlistPerson.presentIdeas?.length === 0){
            return <div className='lightGrey text-center'>They haven't added any gift ideas yet!</div>
        }
        return props.wishlistPerson.presentIdeas?.map(s => {
            let claimed = s.claimerEmail !== null && s.claimerEmail !== '';
            let claimedByYou = s.claimerEmail === props.loggedInEmail;
            let unclaim = claimedByYou ? <button className='btn btn-lg btn-outline-danger w-100'
                                                 onClick={() => unclaimPresentClick(s.id)}>Unclaim</button> : null;
            let claimerText = claimed ? `- claimed by ${claimedByYou ? "you" : s.claimerName}` : '';
            let claimSection = s.claimerName ?
                <>{unclaim}</> :
                <button className='btn btn-lg btn-outline-success w-100'
                        onClick={() => claimPresentClick(s.id)}>Claim</button>;

            let classes = `mt-1 ml-0 mr-0 row ${claimed ? 'claimed' : ''}`;
            return (
                <tr key={s.id} className={classes}>
                    <td className='col-8 col-md-10' cypress-name={`${s.id}-description`}>
                        <Linkify>{s.description} {claimerText}</Linkify></td>
                    <td className='col-4 col-md-2 text-right' cypress-name={`${s.id}-buttons`}>{claimSection}</td>
                </tr>
            )
        });
    }
    
    return (
        <div className='wishlistSummaryItem mb-3' key={`${props.wishlistPerson.email}`}>
            <div>
                <span className='d-inline-block col-12 text-center'>
                    <h4>{props.wishlistPerson.name}</h4>
                </span>
            </div>
            <table className='table'>
                {getPresentIdeas()}
            </table>
        </div>
    )
}