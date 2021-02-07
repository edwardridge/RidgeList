import {
    Button,
    Checkbox,
    Dialog, DialogActions,
    DialogContent,
    DialogTitle,
    FormControl,
    FormControlLabel,
    Input
} from "@material-ui/core";
import React, {ChangeEvent, useRef, useState} from "react";
import {useWishlistClient} from "./useWishlistClient";
import {useMaterialStyles} from "../useMaterialStyles";
import {WishlistModel} from "../../nswag/api.generated";

interface CreateNewPersonProps{
    setWishlist: (wishlistModel: WishlistModel) => void;
    wishlist: WishlistModel;
}

export const CreateNewPerson = (props : CreateNewPersonProps) => {
    const [newPersonName, setNewPersonName] = useState("");
    const [newPersonEmail, setNewPersonEmail] = useState("");
    const [newPersonIsGiftee, setNewPersonIsGiftee] = useState(true);
    const [addingNewPerson, setAddingNewPerson] = useState(false);
    let [addingNewPersonButtonDisabled, setAddingNewPersonButtonDisabled] = useState(false);
    let wishlistClient = useWishlistClient();
    let nameInputRef = useRef<HTMLInputElement>(null);

    let createNewPersonClick = async () => {
        var newWishlist = await wishlistClient.addPerson(props.wishlist?.id, newPersonEmail, newPersonName, newPersonIsGiftee);
        props.setWishlist(newWishlist);
        setNewPersonName("");
        setNewPersonEmail("");
        setAddingNewPerson(false);
    }

    let changeNewEmail = (event : ChangeEvent<HTMLInputElement>) => {
        let npe = event.target.value;
        setNewPersonEmail(event.target.value);
        let f = props.wishlist?.people?.some(s => s.email === npe) ?? false;
        setAddingNewPersonButtonDisabled(f);
    }

    let clickAddNewPerson = () => {
        setAddingNewPerson(true);
        setTimeout(() => nameInputRef?.current?.focus(), 0);
    }

    let onCLickCancelAddNewPerson = () => {
        setAddingNewPerson(false);
        setNewPersonName("");
    }

    const classes = useMaterialStyles();
    
    return (
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
}