import React, {useState} from "react";
import { LoginDetails } from "../useLogin";
import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";

interface WishlistPersonRowProps{
    loginDetails : LoginDetails;
    wishlistPerson : WishlistPersonModel;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

interface OtherPersonWishlistRowProps{
    wishlistPerson : WishlistPersonModel;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

export const OtherPersonWishlistRow = (props : OtherPersonWishlistRowProps) => {
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
                    return (
                        <div key={s.id}>
                            <span>{s.description}</span>
                            <button className='btn btn-success'>I'll get this!</button>
                        </div>
                    )
                })}
            </div>
        </div>
    )
}

export const WishlistPersonRow = (props : WishlistPersonRowProps) => {
    const [newItemDescription, setNewItemDescription] = useState("");

    let clickAddItem = () => {
        new WishlistClient().addPresentIdea(props.wishlistId, props.loginDetails.Email, newItemDescription).then(s => { 
            props.setWishlist(s);
            setNewItemDescription("");
        });
    }
    
    let addItems = (<div className='personItems'>
        <ul>
        {props.wishlistPerson.presentIdeas?.map(s => {
            return <li key={s.id}>{s.description}</li>
        })}
        </ul>
        <div className='mt-lg-4'>
            <input cypress-name='AddItem' value={newItemDescription} onChange={(event) => {setNewItemDescription(event.target.value)}} className='col-10' type="text" placeholder='What would you like?'></input>
            <button cypress-name='AddItemButton' onClick={clickAddItem} className='btn btn-primary col-2'>Add gift idea</button>
        </div>
    </div>);
   
    return (
        
        <div className='wishlistSummaryItem' key={`${props.wishlistPerson.email}`}>
            <div>
                <span className='d-inline-block col-10'>
                    {props.wishlistPerson.name} 
                    <span className='emailDetails'> ({props.wishlistPerson.email})</span>
                </span>
            </div>
            {addItems}
        </div>
    )
}