import React, { useState, useEffect } from "react";
import {RouteComponentProps, withRouter} from "react-router-dom";
import {WishlistClient, WishlistModel} from "../../nswag/api.generated";
import {IWishlistRepository} from "./IWishlistRepository";

interface WishlistProps {
    id: string;
}

interface Props extends RouteComponentProps<WishlistProps> {

    wishlistRepository : IWishlistRepository;
}

 const Wishlist : React.FC<Props> = (props) => {
    const [wishlist, setWishlist] = useState<WishlistModel|null>(null);
    const [newPersonName, setNewPersonName] = useState("");
    
    useEffect(() => {
        let id = props.match.params.id;
        props.wishlistRepository.getWishlist(id).then(s => setWishlist(s));
    }, [wishlist?.id]);
    
     let createNewPersonClick = async () => {
        var newWishlist = await new WishlistClient().addPerson(wishlist?.id, newPersonName);
        setWishlist(newWishlist);
        setNewPersonName("");
    }
    
    let createButonIsDisabled = () => {
        return (wishlist?.people?.indexOf(newPersonName) ?? 0) > -1;
    }

    if (wishlist) {
        let listOfPeople = <ul cypress-name="ListOfPeople"> {wishlist.people?.map((s,i) => <li key={`${s}${i}`}>{s}</li>)}</ul>
        
        let createNewPerson = (
                <div>
                <input type="text" 
                       cypress-name="NewPersonName" 
                       onChange={(event) => setNewPersonName(event.target.value)}
                       value={newPersonName}
                       placeholder="Enter the new email address"></input>
                <button cypress-name="CreateNewPerson" onClick={createNewPersonClick} disabled={createButonIsDisabled()}>Create</button>
                </div>
            )
        
        return (
            <div>
                <h1 id="wishlistTitle">Wishlist - {wishlist.name}</h1>
                {createNewPerson}
                {listOfPeople}
            </div>
        )
    }

    return (
        <div>Loading...</div>
    );
}

export default withRouter(Wishlist);