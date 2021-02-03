import React, {useState, useEffect, ChangeEvent, useRef} from "react";
import {RouteComponentProps, withRouter} from "react-router-dom";
import {WishlistClient, WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import {IWishlistRepository} from "./IWishlistRepository";
import "./WishlistSummary.css";
import { useGetLogin } from "../useLogin";
import {WishlistPersonRow} from "./WishlistPersonRow";
import {OtherPersonWishlistRow} from "./OtherPersonWishlistRow";
import { Button, Modal } from "react-bootstrap";
import {useWishlistClient} from "./useWishlistClient";
import * as signalR from "@microsoft/signalr";
import { Grid, Typography } from "@material-ui/core";
import { useMaterialStyles } from "../useMaterialStyles";

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
     const [newPersonIsGiftee, setNewPersonIsGiftee] = useState(true);    
     const [addingNewPerson, setAddingNewPerson] = useState(false);
     const login = useGetLogin(false);
     let [addingNewPersonButtonDisabled, setAddingNewPersonButtonDisabled] = useState(false);
     let wishlistClient = useWishlistClient();
     let nameInputRef = useRef<HTMLInputElement>(null);
     const classes = useMaterialStyles();
    
     useEffect(() => {
        let id = props.match.params.id;
        props.wishlistRepository.getWishlist(id).then(s => { 
            setWishlist(s);
        });

         const connection = new signalR.HubConnectionBuilder()
             .withUrl("/wishlisthub")
             .build();

         

         connection.start().then(s => {
             connection.invoke("Connect", id).then(s => {
                 connection.on("UpdateWishlist", (wishlistModel : WishlistModel) => {
                     setWishlist(wishlistModel);
                 });
             });
         }).catch(err => document.write(err));
         
     }, [wishlist?.id]);
    
     let createNewPersonClick = async () => {
        var newWishlist = await wishlistClient.addPerson(wishlist?.id, newPersonEmail, newPersonName, newPersonIsGiftee);
        setWishlist(newWishlist);
        setNewPersonName("");
        setNewPersonEmail("");
        setAddingNewPerson(false);
    }
    
     let addPersonButtonShouldBeDisabled = () => {
         return wishlist?.people?.some(s => s.email === login.Email);
     }
     
     let changeNewEmail = (event : ChangeEvent<HTMLInputElement>) => {
         let npe = event.target.value;
         setNewPersonEmail(event.target.value);
         let f = wishlist?.people?.some(s => s.email === npe) ?? false;
         setAddingNewPersonButtonDisabled(f);
     }
     
     let clickAddNewPerson = () => {
         setAddingNewPerson(true);
         setTimeout(() => nameInputRef?.current?.focus(), 0);
     }

    if (wishlist) {
        let onCLickCancelAddNewPerson = () => {
            setAddingNewPerson(false);
            setNewPersonName("");
        }
        
        let loggedInWishlist = wishlist.people?.find(s => s.email === login.Email) ?? {} as WishlistPersonModel;
        let otherGiftees = wishlist.people?.filter(s => s.email !== login.Email && s.giftee === true);
        let otherNonGiftees = wishlist.people?.filter(s => s.email !== login.Email && s.giftee === false);
        
        let createNewPerson = (
            <>
                <Button variant="outline-dark" size='lg' cypress-name='AddNewPerson' className='w-100' onClick={clickAddNewPerson}>
                Add Someone New
                </Button>

                <Modal show={addingNewPerson} onHide={onCLickCancelAddNewPerson}>
                    <Modal.Header closeButton>
                        <Modal.Title>Add Somone New!</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <div className='input-group-lg'>
                            <input type="text" ref={nameInputRef} className='form-control w-100' onChange={(event) => setNewPersonName(event.target.value)}
                                value={newPersonName} placeholder="What's their name?" cypress-name='NewPersonName'></input>
                            <input type="text" className='form-control w-100 mt-2' onChange={changeNewEmail}
                                value={newPersonEmail} placeholder="What's their email?" cypress-name='NewPersonEmail'></input>
                        </div>
                        <div className="form-check mt-3">
                            <input className="form-check-input" type="checkbox" checked={newPersonIsGiftee} id="areTheyGiftee" onChange={(e) => {setNewPersonIsGiftee(e.target.checked)}}/>
                                <label className="form-check-label" htmlFor="areTheyGiftee">
                                    Are they receiving gifts?   
                                </label>
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="primary" block={true} size='lg' disabled={addingNewPersonButtonDisabled} onClick={createNewPersonClick} cypress-name='CreateNewPerson'>
                        Add
                        </Button>
                        <Button variant="secondary" block={true} size='lg' onClick={onCLickCancelAddNewPerson}>
                            Close
                        </Button>
                    </Modal.Footer>
                </Modal>
            </>
        )
        
        let listOfOtherPeoplesIdeas = (
            <div className='mt-5'>
                <hr className='bigLine'></hr>
                <h3 className='text-center mt-5'>Other people's gift ideas</h3>
                <div className='wishlistSummaries' cypress-name="ListOfPeople">
                    {
                        otherGiftees?.map((s) => 
                            <OtherPersonWishlistRow 
                                cypress-name='WishlistPerson'
                                key={s.email} 
                                wishlistPerson={s} 
                                wishlistId={wishlist?.id} 
                                loggedInEmail={login.Email}
                                setWishlist={setWishlist}></OtherPersonWishlistRow> )
                    }
                </div>
            </div>)
        
        let otherNonGifteeSection = <div className='mt-5'>
            <hr className='bigLine'></hr>
            <h3 className='text-center mt-5'>Gift givers</h3>
            <table className='table'>
                {otherNonGiftees?.map(s => `${s.name} (${s.email})`).join(', ')}
            </table>
        </div>
        
        let addNewItems = <>
            <Typography component="h2" variant="h2" color="primary" align="center" id="wishlistTitle">
                Wishlist - {wishlist.name}
            </Typography>
            <div>
                <Typography component="h5" variant="h5" align="center" id="wishlistTitle">
                    What would you like?
                </Typography>
                <WishlistPersonRow cypress-name='WishlistPerson' wishlistPerson={loggedInWishlist} wishlistId={wishlist.id} setWishlist={setWishlist} loginDetails={login}></WishlistPersonRow>
            </div>
        </>
        
        return (
            <div className={classes.paper}>
                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        {loggedInWishlist.giftee ? addNewItems : <></>}
                    </Grid>
                
                    <Grid item xs={12}>
                        {otherGiftees?.length ?? 0 > 0 ? listOfOtherPeoplesIdeas : <></>}
                    </Grid>
                    <Grid item xs={12}>
                        {otherNonGiftees?.length ?? 0 > 0 ? otherNonGifteeSection : <></>}
                    </Grid>
                
                    <Grid item xs={12}>
                            {createNewPerson}
                    </Grid>
                </Grid>
            </div>
        )
    }

    return (
        <div>Loading...</div>
    );
}

export default withRouter(Wishlist);