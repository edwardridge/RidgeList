import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { HelloWorld } from './components/HelloWorld';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';

import { WishlistHomepage } from './components/Wishlist/WishlistHomepage';
import { Wishlist } from './components/Wishlist/Wishlist';
import './custom.css'
import {WishlistClient} from './nswag/api.generated'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
          <Route exact path='/' >
              <WishlistHomepage wishlistClient={new WishlistClient()}></WishlistHomepage>
          </Route>
          <Route path='/wishlist/:id' component={Wishlist}>
          </Route>
          <Route path='/original' component={HelloWorld} />
          <Route path='/counter' component={Counter} />
          <Route path='/fetch-data' component={FetchData} />
      </Layout>
    );
  }
}
