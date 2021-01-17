import React, {useState} from "react";
import { LoginDetails } from "../useLogin";
import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";

interface WishlistPersonRowProps{
    loginDetails : LoginDetails;
    wishlistPerson : WishlistPersonModel;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

export const WishlistPersonRow : React.FC<WishlistPersonRowProps> = (props) => {
    let isLoggedInUser = props.wishlistPerson.email === props.loginDetails.Email ? " - you" : "";
    const [isEditing, setIsEditing] = useState(false);
    const [newItemDescription, setNewItemDescription] = useState("");

    let toggleEditing = () => {
        setIsEditing(!isEditing);
    }

    let clickAddItem = () => {
        new WishlistClient().addPresentIdea(props.wishlistId, props.loginDetails.Email, newItemDescription).then(s => { 
            props.setWishlist(s);
            setNewItemDescription("");
        });
    }
    
    let editing = (<div className='personItems'>
        {props.wishlistPerson.presentIdeas?.map(s => {
            return <div key={s.id}>{s.description}</div>
        })}
        <div className='mt-lg-4'>
            <input cypress-name='AddItem' value={newItemDescription} onChange={(event) => {setNewItemDescription(event.target.value)}} className='col-10' type="text" placeholder='...add...'></input>
            <button cypress-name='AddItemButton' onClick={clickAddItem} className='btn btn-primary col-2'>Add item</button>
        </div>
    </div>);
    
    return (
        
        <div className='wishlistSummaryItem' key={`${props.wishlistPerson.email}`}>
            <div>
                <span className='d-inline-block col-10'>
                    {props.wishlistPerson.name} 
                    <span className='emailDetails'> ({props.wishlistPerson.email}{isLoggedInUser})</span>
                </span>
                <button cypress-name='ToggleAddItemsButton' onClick={toggleEditing} className='btn btn-dark col-2'>{isEditing ? "Close" : "Add items"}</button>
            </div>
            {isEditing ? editing : null}
        </div>
    )
}