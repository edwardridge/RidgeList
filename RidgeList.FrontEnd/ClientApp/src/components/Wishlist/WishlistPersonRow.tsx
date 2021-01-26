import React, {useState} from "react";
import { LoginDetails } from "../useLogin";
import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import { Button, Modal } from "react-bootstrap";

interface WishlistPersonRowProps{
    loginDetails : LoginDetails;
    wishlistPerson : WishlistPersonModel;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

export const WishlistPersonRow = (props : WishlistPersonRowProps) => {
    const [newItemDescription, setNewItemDescription] = useState("");
    const [showAddItem, setShowAddItem] = useState(false);

    let clickAddItem = (closeAddItem: boolean) => {
        new WishlistClient().addPresentIdea(props.wishlistId, props.loginDetails.Email, newItemDescription).then(s => { 
            props.setWishlist(s);
            setNewItemDescription("");
            if (closeAddItem)
                setShowAddItem(false);
        });
    }

    let removePresentIdea = async (id: string) => {
        let wishlist = await new WishlistClient().removePresentIdea(props.wishlistId, props.loginDetails.Email, id);
        props.setWishlist(wishlist);
    }

    let onClickCancelAddItem = () => {
        setShowAddItem(false);
    }

    let addItems = (
        <>
            
            <div className='personItems'>
                {props.wishlistPerson.presentIdeas?.map(s => {
                    return (
                        <div className='row mt-lg-2' key={s.id}>
                            <span className='col-8 col-md-10'>{s.description}</span>
                            <span className='col-4 col-md-2'>
                                <button className='btn btn-outline-danger w-100' onClick={() => removePresentIdea(s.id)}>Remove</button>
                            </span>
                        </div>)
                })}
            </div>
            <div className='mt-lg-2'>
                <Button variant="outline-primary" className='w-100' onClick={() => { setShowAddItem(true) }}>
                    Add New Item
                </Button>

                <Modal show={showAddItem} onHide={onClickCancelAddItem}>
                    <Modal.Header closeButton>
                        <Modal.Title>Add New Item!</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <input type="text" className='w-100' value={newItemDescription} onChange={(event) => { setNewItemDescription(event.target.value) }} placeholder='What do you want?' cypress-name='AddItem'></input>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={onClickCancelAddItem}>
                            Close
                        </Button>
                        <Button variant="primary" onClick={() => { clickAddItem(false) }}>
                            Save And Add More
                        </Button>
                        <Button variant="primary" onClick={() => { clickAddItem(true) }}>
                            Save And Close
                        </Button>
                    </Modal.Footer>
                </Modal>
                
            </div>
    </>
    );
   
    return (
        
        <div className='wishlistSummaryItem' key={`${props.wishlistPerson.email}`}>
            {addItems}
        </div>
    )
}