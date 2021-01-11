import React, { useState, useEffect } from "react";
import {RouteComponentProps, withRouter} from "react-router-dom";
import {WishlistClient, WishlistModel} from "../../nswag/api.generated";
import {IWishlistRepository} from "./IWishlistRepository";
import "./WishlistSummary.css"

interface WishlistProps {
    id: string;
}

interface Props extends RouteComponentProps<WishlistProps> {

    wishlistRepository : IWishlistRepository;
}

 const Wishlist : React.FC<Props> = (props) => {
    const [wishlist, setWishlist] = useState<WishlistModel|null>(null);
    const [newPersonName, setNewPersonName] = useState("");    
    const [addingNewPerson, setAddingNewPerson] = useState(false);
    
     useEffect(() => {
        let id = props.match.params.id;
        props.wishlistRepository.getWishlist(id).then(s => { 
            setWishlist(s);
            let numPeople = s?.people?.length ?? 0;
            if(numPeople === 0)
            {
                setAddingNewPerson(true);
            }
        });
         
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
        let addNewPersonDetails = (
            <>
                <input type="text"
                       cypress-name="NewPersonName"
                       onChange={(event) => setNewPersonName(event.target.value)}
                       value={newPersonName}
                       placeholder="Add a new email address..."
                       className='form-control col-9'></input>
                <span className='col-2'><button cypress-name="CreateNewPerson" onClick={createNewPersonClick} disabled={createButonIsDisabled()} className='btn btn-success'>Add New Person</button></span>
                <span className='col-1'><button cypress-name="CancelNewPerson" onClick={() => { setAddingNewPerson(false); setNewPersonName("") }} className='btn btn-dark'>Cancel</button></span>
            </>
        )
        
        let addNewRow = <div className='w-100' onClick={()=> setAddingNewPerson(true)}>+</div>
        
        let createNewPerson = (
            <a>
                <div className='newItem wishlistSummaryItem form-row'>
                    {addingNewPerson ? addNewPersonDetails : addNewRow}
                </div>
            </a>
        )
        
        let listOfPeople = (
            <div className='wishlistSummaries' cypress-name="ListOfPeople"> 
                {
                    wishlist.people?.map((s,i) => <div className='wishlistSummaryItem' key={`${s}${i}`}>{s}</div>)
                }
                {createNewPerson}
            </div>)
        
        return (
            <div>
                <h1 id="wishlistTitle">Wishlist - {wishlist.name}</h1>
                {/*{createNewPerson}*/}
                {listOfPeople}
            </div>
        )
    }

    return (
        <div>Loading...</div>
    );
}

export default withRouter(Wishlist);