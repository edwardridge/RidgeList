import React, {useState} from "react";
import { LoginDetails } from "../useLogin";
import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";

interface WishlistPersonRowProps{
    loginDetails : LoginDetails;
    wishlistPerson : WishlistPersonModel;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

export const WishlistPersonRow = (props : WishlistPersonRowProps) => {
    const [newItemDescription, setNewItemDescription] = useState("");

    let clickAddItem = () => {
        new WishlistClient().addPresentIdea(props.wishlistId, props.loginDetails.Email, newItemDescription).then(s => { 
            props.setWishlist(s);
            setNewItemDescription("");
        });
    }

    let removePresentIdea = async (id: string) => {
        let wishlist = await new WishlistClient().removePresentIdea(props.wishlistId, props.loginDetails.Email, id);
        props.setWishlist(wishlist);
    }

    let addItems = (
        <>
            <div>
                <div>
                    <input cypress-name='AddItem' value={newItemDescription} onChange={(event) => {setNewItemDescription(event.target.value)}} className='col-10' type="text" placeholder='What would you like?'></input>
                    <button cypress-name='AddItemButton' onClick={clickAddItem} className='btn btn-primary col-2'>Add gift idea</button>
                </div>
            </div>
            <div className='personItems'>
                {props.wishlistPerson.presentIdeas?.map(s => {
                    return <div key={s.id}>{s.description} <button className='btn btn-danger' onClick={() => removePresentIdea(s.id)}>Remove</button> </div>
                })}
            </div>
    </>
    );
   
    return (
        
        <div className='wishlistSummaryItem' key={`${props.wishlistPerson.email}`}>
            {addItems}
        </div>
    )
}