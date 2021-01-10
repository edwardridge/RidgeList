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
    const [creatingNewPerson, setCreatingNewPerson] = useState(false);
    
    useEffect(() => {
        let id = props.match.params.id;
        props.wishlistRepository.getWishlist(id).then(s => setWishlist(s));
    }, [wishlist?.id]);
    
    let startAddNewPerson = () => {
        setCreatingNewPerson(true);
    }

     let createNewPersonClick = async () => {
        var newWishlist = await new WishlistClient().addPerson(wishlist?.id, newPersonName);
        setWishlist(newWishlist);
        setCreatingNewPerson(false);
    }

    if (wishlist) {
        let listOfPeople = <ul cypress-name="ListOfPeople"> {wishlist.people?.map((s,i) => <li key={`${s}${i}`}>{s}</li>)}</ul>
        
        let createNewPerson;
        if(!creatingNewPerson){
            createNewPerson = <button onClick={startAddNewPerson} cypress-name='AddPerson'>Add Person</button>
        }else{
            createNewPerson = (
                <div>
                <input type="text" 
                       cypress-name="NewPersonName" 
                       onChange={(event) => setNewPersonName(event.target.value)}
                        placeholder="Enter the new email address"></input>
                <button cypress-name="CreateNewPerson" onClick={createNewPersonClick}>Create</button>
                </div>
            )
        }
        
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