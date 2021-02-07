import {
    Button,
    Checkbox,
    Dialog, DialogActions,
    DialogContent,
    DialogTitle,
    FormControl,
    FormControlLabel, Grid, IconButton,
    Input
} from "@material-ui/core";
import React, {ChangeEvent, useRef, useState} from "react";
import {useWishlistClient} from "./useWishlistClient";
import {useMaterialStyles} from "../useMaterialStyles";
import {WishlistClient, WishlistModel} from "../../nswag/api.generated";
import {DeleteForever} from "@material-ui/icons";
import {LoginDetails} from "../useLogin";

interface EditWishlistProps{
    setWishlist: (wishlistModel: WishlistModel) => void;
    wishlist: WishlistModel;
    login: LoginDetails;
}

export const EditWishlist = (props : EditWishlistProps) => {
    let wishlistClient = useWishlistClient();
    let [editWishlist, setEditWishlist] = useState(false);
    const classes = useMaterialStyles();
    
    let clickRemovePerson = async (personId : string) => {
        let newWishlist = await wishlistClient.removePerson(props.wishlist.id, personId);
        props.setWishlist(newWishlist);
    }

    let clickChangeIsGiftee = async (personId : string, isGiftee : boolean) => {
        let newWishlist = await wishlistClient.changeIsGiftee(props.wishlist.id, personId, !isGiftee);
        props.setWishlist(newWishlist);
    }
    
    return (
        <>
            <Button type="submit" variant="contained" fullWidth cypress-name='AddNewPerson' onClick={() => setEditWishlist(true)}>
                Edit Wishlist
            </Button>

            <Dialog open={editWishlist} fullWidth onClose={() => setEditWishlist(false)}>
                <DialogTitle>
                    Edit Wishlist
                </DialogTitle>
                <DialogContent>
                    <FormControl fullWidth>
                        {
                            props.wishlist?.people?.map(s => 
                            {
                                return <Grid container spacing={3}> 
                                    <Grid item xs={5}>{s.name}</Grid> 
                                    <Grid item xs={5}>
                                        <FormControlLabel className={classes.margin} control={<Checkbox checked={s.giftee} id="areTheyGiftee" onChange={(e) => { clickChangeIsGiftee(s.personId, s.giftee) }} color="primary" />}
                                                          label="Receiving gifts?" />
                                    </Grid>
                                    <Grid item xs={1} >
                                        {s.personId === props.login.UserId ? <></> : <IconButton onClick={() => clickRemovePerson(s.personId)}><DeleteForever color="secondary"></DeleteForever ></IconButton>}
                                    </Grid>
                                    
                                </Grid>
                            }
                            )}
                    </FormControl>
                </DialogContent>
                <DialogActions>
                    <Button color="secondary" onClick={() => setEditWishlist(false)}>
                        Close
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    )
}