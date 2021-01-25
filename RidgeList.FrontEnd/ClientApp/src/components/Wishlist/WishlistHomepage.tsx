import React, {ChangeEvent, useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import {WishlistClient, WishlistSummaryModel} from "../../nswag/api.generated";
import { Link } from "react-router-dom";
import './WishlistSummary.css';
import { useGetLogin } from "../useLogin";
import { Modal, ModalDialog, Button } from "react-bootstrap"

interface WishlishHomepageProps{
    wishlistClient: WishlistClient;
}

export const WishlistHomepage = (props : WishlishHomepageProps) => {
    const [creating, setCreating] = useState(false);
    const [nameOfNewWishlist, setNameOfNewWishlist] = useState("");
    const [wishlistSummaries, setWishlistSummaries] = useState([] as WishlistSummaryModel[]);
    const history = useHistory();
    const login = useGetLogin(false);
    const [show, setShow] = useState(false);

    useEffect(() => {
        loadWishListSummaries(login.Email);
    }, [wishlistSummaries.length]);

    let onClickCancel = () => {
        setNameOfNewWishlist("");
        setShow(false);
    }
    
    let onClickCreate = async () => {
        let newWishlist = await props.wishlistClient.create(nameOfNewWishlist, login.Email, login.Name);
        setCreating(false);
        history.push("/wishlist/" + newWishlist.id);
        setShow(false);
    }

    let handleInputChange = (event : ChangeEvent<HTMLInputElement>) => {
        setNameOfNewWishlist(event.target.value);
    }
    
    let loadWishListSummaries = async (email : string) => {
        var summaries = await props.wishlistClient.getSummaries(email);
        setWishlistSummaries(summaries);
    }
    
    let onClickAddWishlist = () => {
        setShow(true);
    }
    
    let createButtons = <>
        <Button variant="primary" onClick={onClickAddWishlist}>
            Create New...
        </Button>

        <Modal show={show} onHide={onClickCancel}>
            <Modal.Header closeButton>
                <Modal.Title>Create New Wishlist</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <input type="text" className='w-100' value={nameOfNewWishlist} onChange={handleInputChange} placeholder='Name of wishlist...' cypress-name='NameOfWishlist'></input>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onClickCancel}>
                    Close
          </Button>
                <Button variant="primary" onClick={onClickCreate}>
                    Create
          </Button>
            </Modal.Footer>
        </Modal>
    </>
        //<div className="modal" id="exampleModal" tabIndex={-1} role="dialog">
        //    <div className="modal-dialog" role="document">
        //        <div className="modal-content">
        //            <div className="modal-header">
        //                <h5 className="modal-title">Create New Wishlist</h5>
        //                <button type="button" className="close" data-dismiss="modal" aria-label="Close">
        //                    <span aria-hidden="true">&times;</span>
        //                </button>
        //            </div>
        //            <div className="modal-body">
        //                <input type="text" value={nameOfNewWishlist} onChange={handleInputChange} placeholder='Name of wishlist...' cypress-name='NameOfWishlist'></input>
        //            </div>
        //            <div className="modal-footer">
        //                <button onClick={onClickCreate} className='btn btn-success' cypress-name='Create'>Create</button>
        //                <button onClick={onClickCancel} className='btn btn-dark' cypress-name='Cancel' data-dismiss="modal">Cancel</button>
        //            </div>
        //        </div>
        //    </div>
        //</div>
        

    let summaries =
        <div className='wishlistSummaries'>
            {
                wishlistSummaries.map(s =>
                    <Link key={s.name} to={`wishlist/${s.id}`}> <div key={s.name} className='wishlistSummaryItem'>{s.name}</div></Link>)
            }
            <a>
                
            </a>
        </div>

    return (
        <div>
            <h2>Your Wishlists</h2>

            <div>
                {createButtons}
            </div>
            { summaries }
        </div>
    );
}