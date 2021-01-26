import React, { useState, useEffect } from "react";
import {RouteComponentProps, withRouter} from "react-router-dom";
import {WishlistClient, WishlistModel} from "../../nswag/api.generated";
import {IWishlistRepository} from "./IWishlistRepository";
import "./WishlistSummary.css";
import { useGetLogin } from "../useLogin";
import {WishlistPersonRow} from "./WishlistPersonRow";
import {OtherPersonWishlistRow} from "./OtherPersonWishlistRow";
import { Button, Modal } from "react-bootstrap";

interface WishlistProps {
    id: string;
}

interface Props extends RouteComponentProps<WishlistProps> {
    wishlistRepository : IWishlistRepository;
}

 const Wishlist = (props : Props) => {
     const [wishlist, setWishlist] = useState<WishlistModel|null>(null);
     const [newPersonName, setNewPersonName] = useState("");
     const [newPersonEmail, setNewPersonEmail] = useState("");    
     const [addingNewPerson, setAddingNewPerson] = useState(false);
     const login = useGetLogin(false);
    
     useEffect(() => {
        let id = props.match.params.id;
        props.wishlistRepository.getWishlist(id).then(s => { 
            setWishlist(s);
        });
         
     }, [wishlist?.id]);
    
     let createNewPersonClick = async () => {
        var newWishlist = await new WishlistClient().addPerson(wishlist?.id, newPersonEmail, newPersonName);
        setWishlist(newWishlist);
        setNewPersonName("");
        setNewPersonEmail("");
    }

    if (wishlist) {
        let onCLickCancelAddNewPerson = () => {
            setAddingNewPerson(false);
            setNewPersonName("");
        }
        
        let loggedInWishlist = wishlist.people?.find(s => s.email === login.Email);
        let otherPeople = wishlist.people?.filter(s => s.email !== login.Email);

        let createNewPerson = (
            <>
                <Button variant="outline-dark" className='w-100' onClick={() => setAddingNewPerson(true)}>
                Add Someone New
                </Button>

                <Modal show={addingNewPerson} onHide={onCLickCancelAddNewPerson}>
                    <Modal.Header closeButton>
                        <Modal.Title>Add Somone New!</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <input type="text" className='w-100' onChange={(event) => setNewPersonName(event.target.value)}
                            value={newPersonName} placeholder="What's their name?" cypress-name='AddItem'></input>
                        <input type="text" className='w-100' onChange={(event) => setNewPersonEmail(event.target.value)}
                            value={newPersonEmail} placeholder="What's their email?" cypress-name='AddItem'></input>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={onCLickCancelAddNewPerson}>
                        Close
                        </Button>
                        <Button variant="primary" onClick={createNewPersonClick}>
                        Add
                        </Button>
                    </Modal.Footer>
                </Modal>
            </>
        )
        
        let listOfOtherPeoplesIdeas = (
            
            <div className='wishlistSummaries' cypress-name="ListOfPeople">
                {
                    otherPeople?.map((s) => 
                        <OtherPersonWishlistRow 
                            key={s.email} 
                            wishlistPerson={s} 
                            wishlistId={wishlist?.id} 
                            loggedInEmail={login.Email}
                            setWishlist={setWishlist}></OtherPersonWishlistRow> )
                }
            </div>)
        
        return (
            <div>
                <h1 className='text-center' id="wishlistTitle">Wishlist - {wishlist.name}</h1>
                <div>
                    <h4 className='text-center mt-4'>What would you like?</h4>
                    <WishlistPersonRow wishlistPerson={loggedInWishlist ?? {}} wishlistId={wishlist.id} setWishlist={setWishlist} loginDetails={login}></WishlistPersonRow>
                </div>
                <div className='mt-5'>
                    <h4 className='text-center mt-4'>Other giftees wishlists</h4>
                    <div> {createNewPerson}</div>
                    {listOfOtherPeoplesIdeas}
                </div>
            </div>
        )
    }

    return (
        <div>Loading...</div>
    );
}

export default withRouter(Wishlist);