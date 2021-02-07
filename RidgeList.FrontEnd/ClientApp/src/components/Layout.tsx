import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import {useGetLogin} from "./useLogin";

export const Layout = (props : any) => {
    let displayName = Layout.name;
    let login = useGetLogin(false);
    return (
      <div>
        <NavMenu login={login}/>
        <Container>
          {props.children}
        </Container>
      </div>
    );
  
}
