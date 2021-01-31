import React, {useRef, useState} from "react";
import { LoginDetails } from "../useLogin";
import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import { Button, Modal } from "react-bootstrap";
import {useWishlistClient} from "./useWishlistClient";
import Linkify from "react-linkify";

interface WishlistPersonRowProps{
    loginDetails : LoginDetails;
    wishlistPerson : WishlistPersonModel;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

export const WishlistPersonRow = (props : WishlistPersonRowProps) => {
    const [newItemDescription, setNewItemDescription] = useState("");
    const [showAddItem, setShowAddItem] = useState(false);
    const wishlistClient = useWishlistClient();
    const inputRef = useRef<HTMLTextAreaElement>(null);
    
    let clickAddItem = async (closeAddItem: boolean) => {
        let wishlist = await wishlistClient.addGiftIdea(props.wishlistId, props.loginDetails.Email, newItemDescription);
        props.setWishlist(wishlist);
        setNewItemDescription("");
        if (closeAddItem){
            setShowAddItem(false);
        }
        else{
            setTimeout(() => inputRef?.current?.focus(),0);
        }
            
    }

    let removePresentIdea = async (id: string) => {
        let wishlist = await wishlistClient.removeGiftIdea(props.wishlistId, props.loginDetails.Email, id);
        props.setWishlist(wishlist);
    }

    let onClickCancelAddItem = () => {
        setShowAddItem(false);
    }
    
    let clickNewItemButton = () => {
        setShowAddItem(true);
        setTimeout(() => inputRef?.current?.focus(),0);
    }

    let addItems = (
        <div className='loggedInPersonGifts'>
            
            <table className='table '>
                <tbody>
                {props.wishlistPerson.presentIdeas?.map(s => {
                    return (
                        <tr className='row' key={s.id}>
                            <td className='col-8 col-md-10'><Linkify>{s.description}</Linkify></td>
                            <td className='col-4 col-md-2'>
                                <button className='btn btn-outline-danger w-100 btn-lg' onClick={() => removePresentIdea(s.id)}>Remove</button>
                            </td>
                        </tr>)
                })}
                </tbody>
            </table>
            <div className='mt-2'>
                
                <Button variant="outline-primary" size='lg' cypress-name='AddNewItemButton' className='w-100' onClick={clickNewItemButton}>
                    Add New Gift Idea
                </Button>

                <Modal show={showAddItem} onHide={onClickCancelAddItem}>
                    <Modal.Header closeButton>
                        <Modal.Title>Add New Gift Idea</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <textarea ref={inputRef} rows={8} className='form-control w-100' value={newItemDescription} onChange={(event) => { setNewItemDescription(event.target.value) }} placeholder='What would you like? You can also include links!' cypress-name='AddItem'></textarea>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button size='lg' block={true} variant="primary" onClick={() => { clickAddItem(false) }}>
                            Save And Add More
                        </Button>
                        <Button size='lg' block={true} variant="primary" cypress-name='SaveItemButton' onClick={() => { clickAddItem(true) }}>
                            Save And Close
                        </Button>
                        <Button size='lg' block={true} variant="secondary" onClick={onClickCancelAddItem}>
                            Close
                        </Button>
                    </Modal.Footer>
                </Modal>
                
            </div>
    </div>
    );
   
    return (
        
        <div className='mt-4' key={`${props.wishlistPerson.email}`}>
            {addItems}
        </div>
    )
}