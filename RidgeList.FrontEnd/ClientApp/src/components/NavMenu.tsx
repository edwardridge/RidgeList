import React, {ChangeEvent,  useState} from 'react';
import { useHistory} from 'react-router-dom';
import './NavMenu.css';
import {
    AppBar,
    Button,  Dialog, DialogActions, DialogContent,
    DialogTitle,
    FormControl,
    TextField,
    Toolbar,
    Typography
} from "@material-ui/core";
import { useMaterialStyles } from './useMaterialStyles';
import { ExitToApp, Person } from '@material-ui/icons';
import {LoginDetails} from "./useLogin";
import {UserClient, UserModel} from "../nswag/api.generated";

interface NavMenuProps{
  login : LoginDetails;
}

export const NavMenu = (props : NavMenuProps) => {
    let history = useHistory();
    const styles = useMaterialStyles()
    let [editingDetails, setEditingDetails] = useState(false);
    let [userDetails, setUserDetails] = useState({} as UserModel);
    let [newName, setNewName] = useState("");
    
    let userClient = new UserClient();
    
    const onClickYourInfo = async () => {
        let userDeets = await userClient.getUserDetails(props.login.UserId);
        setUserDetails(userDeets);
        setNewName(userDeets?.name ?? "");
        setEditingDetails(true);
    }
    
    let onCLickCancelEditingDetails = () => {
        setEditingDetails(false);
    }
    
    let editUserName = (event : ChangeEvent<HTMLInputElement>) => {
        setNewName(event.target.value);
    }
    let newPersonDetailsSaveClick = async () => {
        userDetails.name = newName;
        await userClient.setUserDetails(userDetails);
        setEditingDetails(false);
    }
    
    let editUserDetails =
        <Dialog open={editingDetails} fullWidth onClose={onCLickCancelEditingDetails}>
            <DialogTitle>
                Change Your Info
            </DialogTitle>
            <DialogContent>
                <FormControl fullWidth>
                    <TextField autoFocus className={styles.margin} onChange={editUserName}
                           label="Name" value={newName} fullWidth cypress-name='NewName'></TextField>
                    
                </FormControl>
            </DialogContent>
            <DialogActions>
                <Button color="primary"  onClick={newPersonDetailsSaveClick} cypress-name='CreateNewPerson'>
                    Save
                </Button>
                <Button color="secondary" onClick={onCLickCancelEditingDetails}>
                    Close
                </Button>
            </DialogActions>
        </Dialog>
    
    const goTo = (url : string) => {
        history.push(url);
    };
    
    const navItems = <>
        <Button startIcon={<Person />} onClick={onClickYourInfo}>
            Your Info
        </Button>
        <Button startIcon={<ExitToApp />} onClick={() => {goTo('/logout')}}>
            Logout
        </Button>
        {editUserDetails}
    </>
    
    return (
        <div className={styles.flexGrow1}>
                <AppBar position="static">
                    <Toolbar>
                        <Button className={`${styles.flexGrow1} ${styles.alignLeft}`} onClick={() => history.push('/')}>
                            <Typography variant="h4" className={styles.flexGrow1}>
                                RidgeList
                            </Typography>
                        </Button>
                        { props.login.IsLoggedIn ? navItems : null }
                    </Toolbar>
                </AppBar>
        </div>
    )
}

