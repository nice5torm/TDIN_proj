import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Orders } from './components/Counter';
import { CreateOrder } from './components/CreateOrder';

export const routes = <Layout>
    <Route exact path='/' component={Home} />
    <Route path='/counter' component={Orders} />
    <Route path='/createOrder/:id' component={CreateOrder} />
</Layout>;
