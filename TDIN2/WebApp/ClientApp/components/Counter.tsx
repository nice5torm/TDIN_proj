import * as React from 'react';
import axios from 'axios';
import { RouteComponentProps } from 'react-router';


export class Counter extends React.Component<RouteComponentProps<{}>, {}> {
    constructor() {
        super();

        this.state = {
            email: '',
            clientId: '',
            orders: []
        };
    }

    handleGetClient(e: any) {
        e.preventDefault();
        this.setState(
            { email: e.target.value }
        )
    }
  
    public render() {
        console.log(this.state);
        return <div>
            <h1>Client Orders</h1>

            <p>Please give your email</p>

            <form onSubmit={e => this.handleGetClient(e)}>
                <label>
                    Email:
                    <input
                        type="email"
                        name="email"
                        onChange={e => this.setState({ email: e.target.value })}
                    />
                </label>
                <input type="submit" value="Submit" />
            </form>


        </div>;
    }

   
   
}
