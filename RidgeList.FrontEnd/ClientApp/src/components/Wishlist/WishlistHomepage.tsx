import React, {ChangeEvent, useState, useEffect, useRef} from "react";
import { useHistory } from "react-router-dom";
import {WishlistClient, WishlistSummaryModel} from "../../nswag/api.generated";
import './WishlistSummary.css';
import {LoginDetails, useGetLogin} from "../useLogin";
import { useMaterialStyles } from "../useMaterialStyles";
import {
    Button,
    Checkbox,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    FormControlLabel,
    Grid, IconButton,
    List,
    ListItem, ListItemSecondaryAction,
    ListItemText, Menu, MenuItem,
    Paper,
    TextField,
    Typography
} from "@material-ui/core";
import {Menu as MenuIcon} from "@material-ui/icons"

interface WishlishHomepageProps{
    wishlistClient: WishlistClient;
    login : LoginDetails;
}

export const WishlistHomepage = (props : WishlishHomepageProps) => {
    const [nameOfNewWishlist, setNameOfNewWishlist] = useState("");
    const [creatorIsGiftee, setCreatorIsGiftee] = useState(true);
    const [loadingSummaries, setLoadingSummaries] = useState(true);

    const [wishlistSummaries, setWishlistSummaries] = useState([] as WishlistSummaryModel[]);
    const history = useHistory();
    const [show, setShow] = useState(false);
    const [showClone, setShowClone] = useState(false);
    const inputRef = useRef<HTMLInputElement>(null);
    const classes = useMaterialStyles();
    const [menuAnchorEl, setMenuAnchorEl] = useState(null);

    useEffect(() => {
        loadWishListSummaries();
    }, [wishlistSummaries.length, props.login.UserId]);

    let onClickCancel = () => {
        setNameOfNewWishlist("");
        setShow(false);
    }
    
    let onClickCreate = async () => {
        let newWishlist = await props.wishlistClient.create(nameOfNewWishlist, props.login.UserId, creatorIsGiftee);
        history.push("/wishlist/" + newWishlist.id);
        setShow(false);
    }

    let handleInputChange = (event : ChangeEvent<HTMLInputElement>) => {
        setNameOfNewWishlist(event.target.value);
    }
    
    let loadWishListSummaries = async () => {
        var summaries = await props.wishlistClient.getSummaries(props.login.UserId);
        setWishlistSummaries(summaries);
        setLoadingSummaries(false);
    }
    
    let onClickAddWishlist = () => {
        setShow(true);
        setTimeout(() => {inputRef?.current?.focus();},0);
    }
    
    let createButtons = <>
        <Button fullWidth variant="contained" cypress-name='CreateNewWishlist' onClick={onClickAddWishlist}>
            Create New...
        </Button>

        <Dialog open={show} onClose={onClickCancel}>

            <DialogTitle>Create New Wishlist</DialogTitle>
            <DialogContent>
                <TextField autoFocus margin="dense" value={nameOfNewWishlist} onChange={handleInputChange} label='Name of wishlist...' cypress-name='NameOfWishlist' fullWidth></TextField>
                <FormControlLabel control={<Checkbox checked={creatorIsGiftee} onChange={(e) => { setCreatorIsGiftee(e.target.checked) }} name="areTheyGiftee" color="primary" />} label="Are you receiving gifts?" />
            </DialogContent>
            <DialogActions>
                <Button color="primary" cypress-name='Create' onClick={onClickCreate}>
                    Create
                </Button>
                <Button color="secondary" onClick={onClickCancel}>
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    </>

    const handleOpenMenuClick = (event : any) => {
        setMenuAnchorEl(event.currentTarget);
    };

    const handleCloseMenu = () => {
        setMenuAnchorEl(null);
    };
    
    const handleCloneWishlist = async (wishlistId : string, newWishlistName : string) => {
        await props.wishlistClient.cloneWishlist(wishlistId, newWishlistName);
        await loadWishListSummaries();
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
    
    let summaries =
        <Paper >
            <List component="nav">
                {
                    wishlistSummaries.map(s => { return <ListItem divider key={s.name} button>
                            <ListItemText primary={s.name} onClick={() => history.push(`/wishlist/${s.id}`)} />
                            <ListItemSecondaryAction onClick={handleOpenMenuClick}>
                                <IconButton edge="end" aria-label="delete">
                                    <MenuIcon />
                                </IconButton>
                            </ListItemSecondaryAction>
                        <Menu
                            id="simple-menu"
                            anchorEl={menuAnchorEl}
                            keepMounted
                            open={Boolean(menuAnchorEl)}
                            onClose={handleCloseMenu}
                        >
                            <MenuItem onClick={clickCloneAdd}>Clone</MenuItem>
                        </Menu>
                        <Dialog open={showClone} onClose={onClickCancelClone}>

                            <DialogTitle>Clone Wishlist</DialogTitle>
                            <DialogContent>
                                <TextField autoFocus margin="dense" value={nameOfNewWishlist} onChange={handleInputChange} label='Name of wishlist...' cypress-name='NameOfClonedWishlist' fullWidth></TextField>
                            </DialogContent>
                            <DialogActions>
                                <Button color="primary" cypress-name='Create' onClick={() => handleCloneWishlist(s.id, nameOfNewWishlist)}>
                                    Clone
                                </Button>
                                <Button color="secondary" onClick={onClickCancelClone}>
                                    Close
                                </Button>
                            </DialogActions>
                        </Dialog>
                        </ListItem>
                    })}
            </List>
        </Paper>

    let summarySection = wishlistSummaries?.length > 0 ? summaries : <></>
    
    return (
        <div className={classes.paper}>
            <Grid container spacing={3}>
                <Grid item xs={12}>
                    <Typography component="h1" variant="h5" color="primary" align="center">
                        Your Wishlists
                    </Typography>
                </Grid>
                <Grid item xs={12}>
                    { loadingSummaries ? <CircularProgress /> : summarySection }
                </Grid>
                <Grid item xs={12}>
                    {createButtons}
                </Grid>
            </Grid>
        </div>
    );
}