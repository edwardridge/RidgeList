import { WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import React from "react";
import {useWishlistClient} from "./useWishlistClient";
import Linkify from "react-linkify";
import { List, ListItem, Paper, ListItemText, ListItemIcon, Checkbox, Typography } from "@material-ui/core";

interface OtherPersonWishlistRowProps{
    wishlistPerson : WishlistPersonModel;
    loggedInId : string;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

const OtherPersonWishlistRow = (props : OtherPersonWishlistRowProps) => {
    let wishlistClient = useWishlistClient();
    
    let claimPresentClick = async (presentId : string) => {
        let wishlist = await wishlistClient
            .claimGift(props.wishlistId, props.loggedInId, presentId);
        props.setWishlist(wishlist);
    }

    let unclaimPresentClick = async (presentId : string) => {
        let wishlist = await wishlistClient
            .unclaimGift(props.wishlistId, presentId);
        props.setWishlist(wishlist);
    }

    let toggleClaim = async (claimed: boolean, claimedByYou: boolean, presentId: string) => {
        if (claimed === true && claimedByYou === false) return;
        return claimedByYou ? unclaimPresentClick(presentId) : claimPresentClick(presentId);
    }

    let getPresentIdeas = () => {
        if(props.wishlistPerson.presentIdeas?.length === 0){
            return <div className='lightGrey text-center'>They haven't added any gift ideas yet!</div>
        }
        return props.wishlistPerson.presentIdeas?.map(s => {
            let claimed = s.claimerEmail !== null && s.claimerEmail !== '';

            let claimedByYou = s.claimerId === props.loggedInId;

            let claimedBySomeoneElse = claimed === true && claimedByYou === false;
            let claimerText = claimed ? `- claimed by ${claimedByYou ? "you" : s.claimerName}` : '';
            
            let classes = `${claimed ? 'claimed' : ''}`;
            return (
                <ListItem divider button component="a" key={s.id} className={classes} onClick={() => toggleClaim(claimed, claimedByYou, s.id)}>
                    <ListItemIcon>
                        <Checkbox
                            disabled={claimed === true && !claimedByYou}
                            edge="start"
                            checked={claimedByYou}
                            tabIndex={-1}
                            disableRipple
                        />
                    </ListItemIcon>
                    <ListItemText cypress-name={`${s.id}-description`} className={claimedBySomeoneElse === true ? 'claimed' : '' }>
                        <Linkify>{s.description} {claimerText}</Linkify></ListItemText>
                </ListItem>
            )
        });
    }
    
    return (
        <Paper key={`${props.wishlistPerson.personId}`} className='mt-4'>
            <Typography component="h6" variant="h6" align="center" id="wishlistTitle">
                {props.wishlistPerson.name} <span className='lightGrey'> ({props.wishlistPerson.email})</span>
            </Typography>
            <List>
                {getPresentIdeas()}
            </List>
        </Paper>
    )
}

export default OtherPersonWishlistRow;