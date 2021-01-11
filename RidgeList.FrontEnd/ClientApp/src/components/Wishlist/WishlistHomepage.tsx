import React, {ChangeEvent, useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import {WishlistClient, WishlistModel, WishlistSummaryModel} from "../../nswag/api.generated";
import { Link, Redirect } from "react-router-dom";
import './WishlistSummary.css';
import Cookie from "js-cookie";

interface WishlishHomepageProps{
    wishlistClient: WishlistClient;
}

export const WishlistHomepage: React.FC<WishlishHomepageProps> = (props) => {
    const [creating, setCreating] = useState(false);
    const [userEmail, setUsersEmail] = useState("");
    const [nameOfNewWishlist, setNameOfNewWishlist] = useState("");
    const [wishlistSummaries, setWishlistSummaries] = useState([] as WishlistSummaryModel[]);
    const history = useHistory();
    
    useEffect(() => {
        loadWishListSummaries();
    }, [wishlistSummaries.length]);
    
    useEffect(() => {
        setUsersEmail(Cookie.get("email") ?? "");
    }, []);

    let onClickCancel = () => {
        setCreating(false);
        setNameOfNewWishlist("");
    }
    
    let onClickCreate = async () => {
        let newWishlist = await props.wishlistClient.create(nameOfNewWishlist);
        setCreating(false);
        history.push("/wishlist/" + newWishlist.id);
    }

    let handleInputChange = (event : ChangeEvent<HTMLInputElement>) => {
        setNameOfNewWishlist(event.target.value);
    }
    
    let loadWishListSummaries = async () => {
        var summaries = await props.wishlistClient.getSummaries();
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
            <h1>Your Wishlists - {userEmail}</h1>
            { summaries }
        </div>
    );
}