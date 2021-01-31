import React, {ChangeEvent, useState, useEffect, useRef} from "react";
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
    const [creatorIsGiftee, setCreatorIsGiftee] = useState(true);
    const [wishlistSummaries, setWishlistSummaries] = useState([] as WishlistSummaryModel[]);
    const history = useHistory();
    const login = useGetLogin(false);
    const [show, setShow] = useState(false);
    const inputRef = useRef<HTMLInputElement>(null);

    useEffect(() => {
        loadWishListSummaries(login.Email);
    }, [wishlistSummaries.length]);

    let onClickCancel = () => {
        setNameOfNewWishlist("");
        setShow(false);
    }
    
    let onClickCreate = async () => {
        let newWishlist = await props.wishlistClient.create(nameOfNewWishlist, login.Email, login.Name, creatorIsGiftee);
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
        setTimeout(() => {inputRef?.current?.focus();},0);
    }
    
    let createButtons = <>
        <Button className='w-100' size='lg' variant="outline-primary" cypress-name='CreateNewWishlist' onClick={onClickAddWishlist}>
            Create New...
        </Button>

        <Modal show={show} onHide={onClickCancel}>
            <Modal.Header closeButton>
                <Modal.Title>Create New Wishlist</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <div className='input-group input-group-lg'>
                    <input ref={inputRef} autoFocus={true} type="text" className='w-100 form-control' value={nameOfNewWishlist} onChange={handleInputChange} placeholder='Name of wishlist...' cypress-name='NameOfWishlist'></input>
                    <div className="form-check mt-3">
                        <input className="form-check-input" type="checkbox" checked={creatorIsGiftee} id="areTheyGiftee" onChange={(e) => {setCreatorIsGiftee(e.target.checked)}}/>
                        <label className="form-check-label" htmlFor="areTheyGiftee">
                            Are you receiving gifts?
                        </label>
                    </div>
                </div>
            </Modal.Body>
            <Modal.Footer>
                <Button block={true} variant="primary" size='lg' cypress-name='Create' onClick={onClickCreate}>
                    Create
                </Button>
                <Button block={true} variant="secondary" size='lg' onClick={onClickCancel}>
                    Close
                </Button>
            </Modal.Footer>
        </Modal>
    </>

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
            <h2 className='text-center'>Your Wishlists</h2>

            <div>
                {createButtons}
            </div>
            { summaries }
        </div>
    );
}