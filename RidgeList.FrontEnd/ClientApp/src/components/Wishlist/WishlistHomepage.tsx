import React, {ChangeEvent, useState, useEffect} from "react";
import { useHistory } from "react-router-dom";
import {WishlistClient, WishlistModel, WishlistSummaryModel} from "../../nswag/api.generated";
import { Link, Redirect } from "react-router-dom";

interface WishlishHomepageProps{
    wishlistClient: WishlistClient;
}

export const WishlistHomepage: React.FC<WishlishHomepageProps> = (props) => {
    const [creating, setCreating] = useState(false);
    const [nameOfNewWishlist, setNameOfNewWishlist] = useState("");
    const [wishlistSummaries, setWishlistSummaries] = useState([] as WishlistSummaryModel[]);
    const history = useHistory();
    
    useEffect(() => {
        loadWishListSummaries();
    }, [wishlistSummaries.length]);

    let onClickNewWishlist = () => {
        setCreating(true);
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
    
    let createButtons;
    if (!creating) {
        createButtons = <button onClick={onClickNewWishlist} cypress-name='CreateNewWishlist'>Create New Wishlist</button>
    }
    else {
        createButtons = <div><input type="text" value={nameOfNewWishlist} onChange={handleInputChange} placeholder='Name of wishlist...' cypress-name='NameOfWishlist'></input> <button onClick={onClickCreate} cypress-name='Create'>Create</button></div>
    }

    let summaries =
        <ul>
            {
                wishlistSummaries.map(s =>
                    <li key={s.name}>
                        <Link to={`wishlist/${s.id}`}> {s.name}</Link>
                    </li>)
            }
        </ul>

    return (
        <div>
            { createButtons}
            { summaries}
        </div>
    );
}