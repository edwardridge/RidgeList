import React, {useState, useEffect, ChangeEvent, useRef} from "react";
import {RouteComponentProps, withRouter} from "react-router-dom";
import {WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import {IWishlistRepository} from "./IWishlistRepository";
import "./WishlistSummary.css";
import { useGetLogin } from "../useLogin";
import {WishlistPersonRow} from "./WishlistPersonRow";
import {OtherPersonWishlistRow} from "./OtherPersonWishlistRow";
import {useWishlistClient} from "./useWishlistClient";
import * as signalR from "@microsoft/signalr";
import {
    Button,
    Checkbox,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    FormControl,
    FormControlLabel,
    Grid,
    Input,
    Paper,
    Typography
} from "@material-ui/core";
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
         
     }, [wishlist?.id, props.match.params.id, props.wishlistRepository]);
    
     let createNewPersonClick = async () => {
        var newWishlist = await wishlistClient.addPerson(wishlist?.id, newPersonEmail, newPersonName, newPersonIsGiftee);
        setWishlist(newWishlist);
        setNewPersonName("");
        setNewPersonEmail("");
        setAddingNewPerson(false);
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

        let loggedInWishlist = wishlist.people?.find(s => s.personId === login.UserId) ?? {} as WishlistPersonModel;
        let otherGiftees = wishlist.people?.filter(s => s.personId !== login.UserId && s.giftee === true);
        let otherNonGiftees = wishlist.people?.filter(s => s.personId !== login.UserId && s.giftee === false);
        
        let createNewPerson = (
            <>
                <Button type="submit" variant="contained" fullWidth cypress-name='AddNewPerson' onClick={clickAddNewPerson}>
                Add Someone New
                </Button>

                <Dialog open={addingNewPerson} fullWidth onClose={onCLickCancelAddNewPerson}>
                    <DialogTitle>
                        Add Somone New!
                    </DialogTitle>
                    <DialogContent>
                        <FormControl fullWidth>
                            <Input type='text' autoFocus className={classes.margin} onChange={(event) => setNewPersonName(event.target.value)}
                                value={newPersonName} fullWidth placeholder="What's their name?" cypress-name='NewPersonName'></Input>
                            <Input type='text' className={classes.margin} onChange={changeNewEmail}
                                value={newPersonEmail} fullWidth placeholder="What's their email?" cypress-name='NewPersonEmail'></Input>
                       
                            <FormControlLabel className={classes.margin} control={<Checkbox checked={newPersonIsGiftee} id="areTheyGiftee" onChange={(e) => { setNewPersonIsGiftee(e.target.checked) }} color="primary" />}
                                label="Are they receiving gifts?" />
                        </FormControl>
                    </DialogContent>
                    <DialogActions>
                        <Button color="primary" disabled={addingNewPersonButtonDisabled} onClick={createNewPersonClick} cypress-name='CreateNewPerson'>
                        Add
                        </Button>
                        <Button color="secondary" onClick={onCLickCancelAddNewPerson}>
                            Close
                        </Button>
                    </DialogActions>
                </Dialog>
            </>
        )
        
        let listOfOtherPeoplesIdeas = (
            <div className='mt-5'>
                <Typography component="h5" variant="h5" align="center" id="wishlistTitle">
                    Other people's gift ideas
                </Typography>
                <div cypress-name="ListOfPeople">
                    {
                        otherGiftees?.map((s) => 
                            <OtherPersonWishlistRow 
                                cypress-name='WishlistPerson'
                                key={s.email} 
                                wishlistPerson={s} 
                                wishlistId={wishlist?.id} 
                                loggedInId={login.UserId}
                                setWishlist={setWishlist}></OtherPersonWishlistRow> )
                    }
                </div>
            </div>)
        
        let otherNonGifteeSection = <div className='mt-5'>
            <Typography component="h6" variant="h6" color="secondary" align="center">
                Gift givers
            </Typography>
            <div>
                {otherNonGiftees?.map(s => `${s.name} (${s.email})`).join(', ')}
            </div>
        </div>
        
        let addNewItems = <>
            <Typography component="h3" variant="h3" color="primary" align="center" id="wishlistTitle">
                {wishlist.name}
            </Typography>
            <Typography className='mt-4' component="h5" variant="h5" align="center" id="wishlistTitle">
                What would you like?
            </Typography>
            <Paper className='mt-2'><WishlistPersonRow cypress-name='WishlistPerson' wishlistPerson={loggedInWishlist} wishlistId={wishlist.id} setWishlist={setWishlist} loginDetails={login}></WishlistPersonRow></Paper>
        </>
        
        return (
            <div className={classes.paper}>
                <Grid container spacing={3}>
                    <Grid item xs={12}>
                        {loggedInWishlist.giftee ? addNewItems : <></>}
                    </Grid>
                
                    <Grid item xs={12}>
                        {otherGiftees && otherGiftees.length > 0 ? listOfOtherPeoplesIdeas : <></>}
                    </Grid>
                    <Grid item xs={12}>
                        {otherNonGiftees && otherNonGiftees.length > 0 ? otherNonGifteeSection : <></>}
                    </Grid>
                
                    <Grid item xs={12}>
                            {createNewPerson}
                    </Grid>
                </Grid>
            </div>
        )
    }

    return (
        <CircularProgress />
    );
}

export default withRouter(Wishlist);