import React, {Component, useEffect} from 'react';
import {Link, useHistory} from 'react-router-dom';
import './NavMenu.css';
import {AppBar, Button, Fab, Grid, IconButton, Menu, MenuItem, Toolbar, Typography} from "@material-ui/core";
import { useMaterialStyles } from './useMaterialStyles';
import { ExitToApp, Person } from '@material-ui/icons';
import {green} from "@material-ui/core/colors";
import {useGetLogin} from "./useLogin";

interface NavMenuProps{
  isLoggedIn : boolean;
}

interface NavMenuState{
  collapsed: boolean;
}


export const NavMenu = (props : NavMenuProps) => {
    let displayName = NavMenu.name;
    let history = useHistory();
    const [anchorEl, setAnchorEl] = React.useState(null);
    const styles = useMaterialStyles();
    
    const goTo = (url : string) => {
        history.push(url);
    };
    const navItems = <>
        <Button startIcon={<Person />} >
            Your Info
        </Button>
        <Button startIcon={<ExitToApp />} onClick={() => {goTo('/logout')}}>
            Logout
        </Button>
    </>
    
    return (
        <div className={styles.flexGrow1}>
                <AppBar position="static">
                    <Toolbar>
                        <Typography variant="h4" className={styles.flexGrow1}>
                            RidgeList
                        </Typography>
                        { props.isLoggedIn ? navItems : null }
                    </Toolbar>
                </AppBar>
        </div>
    )
}

