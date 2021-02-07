import {
    Button,
    Dialog, DialogActions, DialogContent,
    DialogTitle,
    IconButton, ListItem,
    ListItemSecondaryAction,
    ListItemText,
    Menu,
    MenuItem, TextField
} from "@material-ui/core";
import {Menu as MenuIcon} from "@material-ui/icons";
import React, {ChangeEvent, useState} from "react";
import {WishlistSummaryModel} from "../../nswag/api.generated";
import {useHistory} from "react-router-dom";
import {useWishlistClient} from "./useWishlistClient";

interface WishlistSummaryItemProps{
    summary: WishlistSummaryModel
    loadWishListSummaries : () => void;
}

export const WishlistSummaryItem = (props : WishlistSummaryItemProps) => {
    let history = useHistory();
    let wishlistClient = useWishlistClient();
    const [nameOfNewWishlist, setNameOfNewWishlist] = useState("");
    const [menuAnchorEl, setMenuAnchorEl] = useState(null);
    const [showClone, setShowClone] = useState(false);

    const handleOpenMenuClick = (event : any) => {
        setMenuAnchorEl(event.currentTarget);
    };

    const handleCloseMenu = () => {
        setMenuAnchorEl(null);
    };

    const handleCloneWishlist = async (wishlistId : string, newWishlistName : string) => {
        await wishlistClient.cloneWishlist(wishlistId, newWishlistName);
        await props.loadWishListSummaries();
        setShowClone(false);
    }

    const clickCloneAdd = () => {
        setShowClone(true);
        setMenuAnchorEl(null);
    }

    const onClickCancelClone = () => {
        setShowClone(false);
        setNameOfNewWishlist("");
    }

    const removeWishlistClick = async (wishlistId : string) => {
        let confirm = window.confirm('Are you sure?');
        if(confirm === true){
            await wishlistClient.removeWishlist(wishlistId);
            await props.loadWishListSummaries();
        }
    }

    let handleInputChange = (event : ChangeEvent<HTMLInputElement>) => {
        setNameOfNewWishlist(event.target.value);
    }
    
    return ( <><Menu
            id="simple-menu"
            anchorEl={menuAnchorEl}
            keepMounted
            open={Boolean(menuAnchorEl)}
            onClose={handleCloseMenu}
        >
            <MenuItem onClick={clickCloneAdd}>Clone</MenuItem>
            <MenuItem onClick={() => removeWishlistClick(props.summary.id)}>Remove</MenuItem>
        </Menu>
            <Dialog open={showClone} onClose={onClickCancelClone}>

                <DialogTitle>Clone Wishlist</DialogTitle>
                <DialogContent>
                    <TextField autoFocus margin="dense" value={nameOfNewWishlist} onChange={handleInputChange} label='Name of wishlist...' cypress-name='NameOfClonedWishlist' fullWidth></TextField>
                </DialogContent>
                <DialogActions>
                    <Button color="primary" cypress-name='Create' onClick={() => handleCloneWishlist(props.summary.id, nameOfNewWishlist)}>
                        Clone
                    </Button>
                    <Button color="secondary" onClick={onClickCancelClone}>
                        Close
                    </Button>
                </DialogActions>
            </Dialog>
    <ListItem divider key={props.summary.id} button>
        <ListItemText primary={props.summary.name} onClick={() => history.push(`/wishlist/${props.summary.id}`)} />
        <ListItemSecondaryAction onClick={handleOpenMenuClick}>
        <IconButton edge="end" aria-label="delete">
            <MenuIcon />
            </IconButton>
        </ListItemSecondaryAction>
    </ListItem></>
    );
}