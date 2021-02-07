import React, {useState} from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';

import { WishlistHomepage } from './components/Wishlist/WishlistHomepage';
import { default as Wishlist } from './components/Wishlist/Wishlist';
import './custom.css'
import {WishlistClient} from './nswag/api.generated'
import {WishlistRepository} from "./components/Wishlist/IWishlistRepository";
import {Login} from "./components/Login/Login";
import {Logout} from "./components/Login/Logout";
import { Container, CssBaseline } from '@material-ui/core';
import {useGetLogin} from "./components/useLogin";

export const App = (props:any) => {
    // static displayName = App.name;
    let login = useGetLogin(false);

    const [logiVal, setLoginVal] = useState(login);
    return (
        <Layout>
            <Container component="main" maxWidth="md">
                <CssBaseline />
                <Route exact path='/'>
                    <Login setLoginVal={setLoginVal}></Login>
                </Route>
                <Route path='/wishlists' >
                    <WishlistHomepage login={logiVal} wishlistClient={new WishlistClient()}></WishlistHomepage>
                </Route>
                <Route path='/wishlist/:id'>
                    <Wishlist login={logiVal} wishlistRepository={new WishlistRepository()} />
                </Route>
                <Route path='/logout' >
                    <Logout setLoginVal={setLoginVal}></Logout>
                </Route>
            </Container>
        </Layout>
    );
  }

