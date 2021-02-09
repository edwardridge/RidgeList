import React, {useRef, useState} from "react";
import { LoginDetails } from "../useLogin";
import { WishlistModel, WishlistPersonModel} from "../../nswag/api.generated";
import {useWishlistClient} from "./useWishlistClient";
import Linkify from "react-linkify";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, IconButton, List, ListItem, ListItemIcon, ListItemText, TextField } from "@material-ui/core";
import { DeleteForever } from '@material-ui/icons';

interface WishlistPersonRowProps{
    loginDetails : LoginDetails;
    wishlistPerson : WishlistPersonModel;
    wishlistId: string;
    setWishlist: (wishlist : WishlistModel) => void;
}

const WishlistPersonRow = (props : WishlistPersonRowProps) => {
    const [newItemDescription, setNewItemDescription] = useState("");
    const [showAddItem, setShowAddItem] = useState(false);
    const wishlistClient = useWishlistClient();
    const inputRef = useRef<HTMLTextAreaElement>(null);
    
    let clickAddItem = async (closeAddItem: boolean) => {
        let wishlist = await wishlistClient.addGiftIdea(props.wishlistId, props.loginDetails.UserId, newItemDescription);
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
        let wishlist = await wishlistClient.removeGiftIdea(props.wishlistId, props.loginDetails.UserId, id);
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
        <List component="nav">
                {props.wishlistPerson.presentIdeas?.map(s => {
                    return (
                        <ListItem divider key={s.id}>
                            <ListItemText><Linkify>{s.description}</Linkify></ListItemText>
                            <ListItemIcon>
                                <IconButton edge="end" onClick={() => removePresentIdea(s.id)}><DeleteForever color="secondary" fontSize="large"></DeleteForever ></IconButton>
                            </ListItemIcon>
                        </ListItem>)
                })}
            <ListItem divider button component="a">
                <Button type="submit" fullWidth variant="contained" cypress-name='AddNewItemButton' color="primary" onClick={clickNewItemButton}>
                    Add New Gift Idea
                </Button>
           </ListItem>
            <div className='mt-2'>
                <Dialog open={showAddItem} onClose={onClickCancelAddItem}>
                    <DialogTitle>Add New Gift Idea</DialogTitle>
                    <DialogContent>
                        <TextField multiline fullWidth rows={8} value={newItemDescription} onChange={(event) => { setNewItemDescription(event.target.value) }} placeholder='What would you like? You can also include links!' cypress-name='AddItem'></TextField>
                    </DialogContent>
                    <DialogActions> 
                        <Button color="primary" onClick={() => { clickAddItem(false) }}>
                            Save And Add More
                        </Button>
                        <Button cypress-name='SaveItemButton' color="primary" onClick={() => { clickAddItem(true) }}>
                            Save And Close
                        </Button>
                        <Button color="secondary" onClick={onClickCancelAddItem}>
                            Close
                        </Button>
                    </DialogActions>
                </Dialog>
                
            </div>
        </List>
    );
   
    return (
        
        <div>
            {addItems}
        </div>
    )
}

export default WishlistPersonRow;