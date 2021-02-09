import React, {Suspense, useState} from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';

//import { default as Wishlist } from './components/Wishlist/Wishlist';
import './custom.css'
import {WishlistClient} from './nswag/api.generated'
import {WishlistRepository} from "./components/Wishlist/IWishlistRepository";

import { Container, CssBaseline } from '@material-ui/core';
import {useGetLogin} from "./components/useLogin";

const Logout = React.lazy(() => import('./components/Login/Logout'));
const Login = React.lazy(() => import('./components/Login/Login'));
const WishlistHomepage = React.lazy(() => import('./components/Wishlist/WishlistHomepage'));
const Wishlist = React.lazy(() => import('./components/Wishlist/Wishlist'));

const App = (props:any) => {
    let login = useGetLogin(false);

    const LoadingMessage = (props: any) => {
        return (
            <div>I'm loading...</div>
            );
    }

    const [logiVal, setLoginVal] = useState(login);
    return (
        <Suspense fallback={<LoadingMessage />}>
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
        </Suspense>
    );
}

export default App;

