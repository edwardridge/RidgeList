import React, {ChangeEvent, useState, useEffect, useRef} from "react";
import { useHistory } from "react-router-dom";
import {WishlistClient, WishlistSummaryModel} from "../../nswag/api.generated";
import { Link } from "react-router-dom";
import './WishlistSummary.css';
import { useGetLogin } from "../useLogin";
import { useMaterialStyles } from "../useMaterialStyles";
import { Button, Checkbox, Dialog, DialogActions, DialogContent, DialogTitle, Divider, FormControlLabel, Grid, List, ListItem, ListItemText, Paper, TextField, Typography } from "@material-ui/core";

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
    const classes = useMaterialStyles();

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
        <Button fullWidth cypress-name='CreateNewWishlist' onClick={onClickAddWishlist}>
            Create New...
        </Button>

        <Dialog open={show} onClose={onClickCancel}>

            <DialogTitle>Create New Wishlist</DialogTitle>
            <DialogContent>
                <div className='input-group input-group-lg'>
                    <TextField autoFocus margin="dense" value={nameOfNewWishlist} onChange={handleInputChange} label='Name of wishlist...' cypress-name='NameOfWishlist' fullWidth></TextField>
                    <FormControlLabel control={<Checkbox checked={creatorIsGiftee} onChange={(e) => { setCreatorIsGiftee(e.target.checked) }} name="areTheyGiftee" color="primary" />} label="Are you receiving gifts?" />
                </div>
            </DialogContent>
            <DialogActions>
                <Button cypress-name='Create' onClick={onClickCreate}>
                    Create
                </Button>
                <Button onClick={onClickCancel}>
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    </>

    let summaries =
        <div className={classes.root}>
            <List component="nav">
                {
                    wishlistSummaries.map(s =>
                        <ListItem divider key={s.name} onClick={() => history.push(`/wishlist/${s.id}`)} button><ListItemText primary={s.name} /></ListItem>)
                }
            </List>
        </div>

    return (
        <div className={classes.paper}>
            <Grid container spacing={3}>
                <Grid item xs={12}>
                    <Typography component="h1" variant="h5" color="primary" align="center">
                        Your Wishlists
                    </Typography>
                </Grid>
                <Grid item xs={12}>
                    { summaries }
                </Grid>
                <Grid item xs={12}>
                    {createButtons}
                </Grid>
            </Grid>
        </div>
    );
}