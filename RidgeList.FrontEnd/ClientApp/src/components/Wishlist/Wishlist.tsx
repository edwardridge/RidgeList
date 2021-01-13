import React, { useState, useEffect } from "react";
import {RouteComponentProps, withRouter} from "react-router-dom";
import {WishlistClient, WishlistModel} from "../../nswag/api.generated";
import {IWishlistRepository} from "./IWishlistRepository";
import "./WishlistSummary.css";
import { useGetLogin } from "../useGetLogin";

interface WishlistProps {
    id: string;
}

interface Props extends RouteComponentProps<WishlistProps> {

    wishlistRepository : IWishlistRepository;
}

 const Wishlist : React.FC<Props> = (props) => {
     const [wishlist, setWishlist] = useState<WishlistModel|null>(null);
     const [newPersonName, setNewPersonName] = useState("");
     const [newPersonEmail, setNewPersonEmail] = useState("");    
     const [addingNewPerson, setAddingNewPerson] = useState(false);
     const login = useGetLogin(false);
    
     useEffect(() => {
        let id = props.match.params.id;
        props.wishlistRepository.getWishlist(id).then(s => { 
            setWishlist(s);
            let numPeople = s?.people?.length ?? 0;
            if(numPeople === 1)
            {
                setAddingNewPerson(true);
            }
        });
         
     }, [wishlist?.id]);
    
     let createNewPersonClick = async () => {
        var newWishlist = await new WishlistClient().addPerson(wishlist?.id, newPersonEmail, newPersonName);
        setWishlist(newWishlist);
        setNewPersonName("");
        setNewPersonEmail("");
    }
    
    let createButonIsDisabled = () => {
        return (wishlist?.people?.map(s => s.email).indexOf(newPersonEmail) ?? 0) > -1;
    }

    if (wishlist) {
        let addNewPersonDetails = (
            <>
                <input type="text"
                       cypress-name="NewPersonName"
                       onChange={(event) => setNewPersonName(event.target.value)}
                       value={newPersonName}
                       placeholder="Name..."
                       className='form-control col-5'></input>
                <input type="text"
                       cypress-name="NewPersonEmail"
                       onChange={(event) => setNewPersonEmail(event.target.value)}
                       value={newPersonEmail}
                       placeholder="Email Address..."
                       className='form-control col-4'></input>
                <span className='col-2'><button cypress-name="CreateNewPerson" onClick={createNewPersonClick} disabled={createButonIsDisabled()} className='btn btn-success'>Add New Person</button></span>
                <span className='col-1'><button cypress-name="CancelNewPerson" onClick={() => { setAddingNewPerson(false); setNewPersonName("") }} className='btn btn-dark'>Cancel</button></span>
            </>
        )
        
        let addNewRow = <div className='w-100' cypress-name="AddNewPerson" onClick={()=> setAddingNewPerson(true)}>+</div>
        
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
                    wishlist.people?.map((s,i) => {
                        let isLoggedInUser = s.email === login.Email ? " - you" : "";
                        return <div className='wishlistSummaryItem' key={`${s.email}${s.name}${i}`}>{s.name} <span className='emailDetails'>({s.email}{isLoggedInUser})</span></div>
                    })
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