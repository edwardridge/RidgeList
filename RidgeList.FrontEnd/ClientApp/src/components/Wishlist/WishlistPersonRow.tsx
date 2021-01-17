import React, {useState} from "react";
import { LoginDetails } from "../useLogin";
import {WishlistPersonModel} from "../../nswag/api.generated";

interface WishlistPersonRowProps{
    loginDetails : LoginDetails;
    wishlistPerson : WishlistPersonModel;
}

export const WishlistPersonRow : React.FC<WishlistPersonRowProps> = (props) => {
    let isLoggedInUser = props.wishlistPerson.email === props.loginDetails.Email ? " - you" : "";
    const [isEditing, setIsEditing] = useState(false);
    
    let toggleEditing = () => {
        setIsEditing(!isEditing);
    }

    let editing = (<div className='personItems'>
        <div className='mt-lg-4'>
            <input cypress-name='addItem' className='col-10' type="text" placeholder='...add...'></input>
            <button cypress-name='addItemButton' className='btn btn-primary col-2'>Add item</button>
        </div>
    </div>);
    
    return (
        
        <div className='wishlistSummaryItem' key={`${props.wishlistPerson.email}`}>
            <div>
                <span className='d-inline-block col-10'>
                    {props.wishlistPerson.name} 
                    <span className='emailDetails'> ({props.wishlistPerson.email}{isLoggedInUser})</span>
                </span>
                <button onClick={toggleEditing} className='btn btn-dark col-2'>{isEditing ? "Close" : "Add items"}</button>
            </div>
            {isEditing ? editing : null}
        </div>
    )
}