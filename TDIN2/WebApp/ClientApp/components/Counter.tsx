import * as React from 'react';
import * as request from 'request';
import { RouteComponentProps } from 'react-router';
import { Order } from './Models';
import { Client } from './Models';

interface FetchDataExampleState {
    readonly orders: Order[];
    readonly client: Client;
    email: string; 
}

export class Counter extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();

        this.state = {
            email: '',
            orders: [],
            client: new Client(null)
        };
    }

    handleGetClient(e: any) {
        e.preventDefault();
        let options: any = {
            json: true
        }
        this.setState(
            { email: e.target.value }
        )
        request.get("http://localhost:2222/api/Client/GetClientByEmail?email=$this.state.email$", options, (response: any, body: any) => {
            let client = new Client(body);
        })
    }

    private static renderOrdersTable(orders: Order[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>id</th>
                    <th>title</th>
                    <th>quantity</th>
                    <th>orderType</th>
                    <th>orderStatus</th>
                    <th>dispatchDate</th>
                    <th>dispatchOccurence</th>
                </tr>
            </thead>
            <tbody>
                {orders.map(order =>
                    <tr key={order.id}>
                        <td>{order.id}</td>
                        <td>{order.title}</td>
                        <td>{order.quantity}</td>
                        <td>{order.orderType}</td>
                        <td>{order.orderStatus}</td>
                        <td>{order.dispatchDate}</td>
                        <td>{order.dispatchOccurence}</td>
                    </tr>
                )}
            </tbody>
        </table>;
    }
  
    public render() {
        return (<div>
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
            <div id="orders">
                {Counter.renderOrdersTable(this.state.orders)}
            </div>
        </div>);
    }

   
   
}
