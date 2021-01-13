import React, {ChangeEvent, useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import {WishlistClient, WishlistModel, WishlistSummaryModel} from "../../nswag/api.generated";
import { Link, Redirect } from "react-router-dom";
import './WishlistSummary.css';
import { useGetLogin } from "../useGetLogin";

interface WishlishHomepageProps{
    wishlistClient: WishlistClient;
}

export const WishlistHomepage: React.FC<WishlishHomepageProps> = (props) => {
    const [creating, setCreating] = useState(false);
    const [nameOfNewWishlist, setNameOfNewWishlist] = useState("");
    const [wishlistSummaries, setWishlistSummaries] = useState([] as WishlistSummaryModel[]);
    const history = useHistory();
    const login = useGetLogin(false);
    
    useEffect(() => {
        loadWishListSummaries(login.Email);
    }, [wishlistSummaries.length]);

    let onClickCancel = () => {
        setCreating(false);
        setNameOfNewWishlist("");
    }
    
    let onClickCreate = async () => {
        let newWishlist = await props.wishlistClient.create(nameOfNewWishlist, login.Email, login.Name);
        setCreating(false);
        history.push("/wishlist/" + newWishlist.id);
    }

    let handleInputChange = (event : ChangeEvent<HTMLInputElement>) => {
        setNameOfNewWishlist(event.target.value);
    }
    
    let loadWishListSummaries = async (email : string) => {
        var summaries = await props.wishlistClient.getSummaries(email);
        setWishlistSummaries(summaries);
    }
    
    let onClickAddWishlist = () => {
        setCreating(true);
    }
    
    let createButtons;
    if (!creating) {
        createButtons = <div onClick={onClickAddWishlist} className='newItem wishlistSummaryItem' cypress-name='CreateNewWishlist'>...create new wishlist...</div>
    }
    else {
        createButtons = <div className='newItem wishlistSummaryItem'>
            <input type="text" value={nameOfNewWishlist} onChange={handleInputChange} placeholder='Name of wishlist...' cypress-name='NameOfWishlist'></input> 
            <button onClick={onClickCreate} className='ml-2 btn btn-success' cypress-name='Create'>Create</button>
            <button onClick={onClickCancel} className='ml-2 btn btn-dark' cypress-name='Cancel'>Cancel</button>
        </div>
    }

    let summaries =
        <div className='wishlistSummaries'>
            {
                wishlistSummaries.map(s =>
                        <Link to={`wishlist/${s.id}`}> <div key={s.name} className='wishlistSummaryItem'>{s.name}</div></Link>)
            }
            <a>
                {createButtons}
            </a>
        </div>

    return (
        <div>
            <h2>Your Wishlists - {login.Name} <span className='emailDetails'>({login.Email})</span></h2>
            { summaries }
        </div>
    );
}