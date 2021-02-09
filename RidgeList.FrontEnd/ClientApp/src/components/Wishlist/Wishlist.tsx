import React, {useState, useEffect} from "react";
import {RouteComponentProps, withRouter} from "react-router-dom";
import {WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import {IWishlistRepository} from "./IWishlistRepository";
import "./WishlistSummary.css";
import {LoginDetails} from "../useLogin";
import * as signalR from "@microsoft/signalr";
import {
    CircularProgress,
    Grid,
    Paper,
    Typography
} from "@material-ui/core";
import { useMaterialStyles } from "../useMaterialStyles";

const EditWishlist = React.lazy(() => import('./EditWishlist'));
const OtherPersonWishlistRow = React.lazy(() => import('./OtherPersonWishlistRow'));
const WishlistPersonRow = React.lazy(() => import('./WishlistPersonRow'));
const CreateNewPerson = React.lazy(() => import('./CreateNewPerson'));

interface WishlistProps {
    id: string;
}

interface Props extends RouteComponentProps<WishlistProps> {
    wishlistRepository : IWishlistRepository;
    login: LoginDetails;
}

 const Wishlist = (props : Props) => {
     const [wishlist, setWishlist] = useState<WishlistModel|null>(null);
     
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
    
     
    if (wishlist) {

        let loggedInWishlist = wishlist.people?.find(s => s.personId === props.login.UserId) ?? {} as WishlistPersonModel;
        let otherGiftees = wishlist.people?.filter(s => s.personId !== props.login.UserId && s.giftee === true);
        let otherNonGiftees = wishlist.people?.filter(s => s.personId !== props.login.UserId && s.giftee === false);
        
      
        
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
                                loggedInId={props.login.UserId}
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
            <Paper className='mt-2'><WishlistPersonRow cypress-name='WishlistPerson' wishlistPerson={loggedInWishlist} wishlistId={wishlist.id} setWishlist={setWishlist} loginDetails={props.login}></WishlistPersonRow></Paper>
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
                
                    <Grid item xs={6}>
                        <CreateNewPerson setWishlist={setWishlist} wishlist={wishlist}></CreateNewPerson>
                    </Grid>
                    <Grid item xs={6}>
                        <EditWishlist setWishlist={setWishlist} wishlist={wishlist} login={props.login}></EditWishlist>
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