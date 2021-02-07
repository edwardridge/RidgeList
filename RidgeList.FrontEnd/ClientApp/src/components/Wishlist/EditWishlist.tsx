import {
    Button,
    Checkbox,
    Dialog, DialogActions,
    DialogContent,
    DialogTitle,
    FormControl,
    FormControlLabel, Grid, IconButton,
    Input, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow
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
                    <TableContainer component={Paper}>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>Name</TableCell>
                                    <TableCell align='right' width='small'>Receving Gifts?</TableCell>
                                    <TableCell ></TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {
                                    props.wishlist?.people?.map(s =>
                                        {
                                            return <TableRow key={s.personId}>
                                                <TableCell component="th" scope="row">{s.name}</TableCell>
                                                <TableCell align='right' width='small'>
                                                    <Checkbox checked={s.giftee} id="areTheyGiftee" onChange={(e) => { clickChangeIsGiftee(s.personId, s.giftee) }} color="primary" />
                                                </TableCell>
                                                <TableCell align='right' width='small' size='small'>
                                                    {s.personId === props.login.UserId ? <></> : <IconButton onClick={() => clickRemovePerson(s.personId)}><DeleteForever color="secondary"></DeleteForever ></IconButton>}
                                                </TableCell>
                                            </TableRow>
                                        })}
                            </TableBody>
                        </Table>
                    </TableContainer>
                        
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