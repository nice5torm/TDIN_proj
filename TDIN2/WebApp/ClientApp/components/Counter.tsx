import * as React from 'react';
import axios from 'axios';
import { RouteComponentProps } from 'react-router';

interface Client {
    id: number;
    name: string;
    email: string;
    ordersClient: Order[];
}
interface Order {
    guid: number;
    book: Book; 
    quantity: number;
    orderStatus: string;
    dispatchedDate: any;
    dispatchOccurence: any;
    orderType: string;
}
interface Book {
    id: number;
    title: string;
    price: number;
    amount: number; 
}

interface FetchDataExampleState {
    email: string; 
    orders: Order[];
}

export class Orders extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();

        this.state = {
            email: '',
            orders: []
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
        axios.request<Client>({
            url: 'http://localhost:2222/api/Client/GetClientByEmail?email=' + this.state.email
        })
            .then((response) => {
            this.setState({ orders: response.data.ordersClient })
            console.log(response.data.ordersClient);
        })
                
    }

    private static renderOrdersTable(orders: Order[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>Id</th>
                    <th>Title</th>
                    <th>Quantity</th>
                    <th>Type</th>
                    <th>Status</th>
                    <th>Dispatched Date</th>
                    <th>Dispatch Occurence</th>
                </tr>
            </thead>
            <tbody>
                {orders.map(order =>
                    <tr key={order.guid}>
                        <td>{order.guid}</td>
                        <td>{order.book.title}</td>
                        <td>{order.quantity}</td>
                        <td>{order.orderType}</td>
                        <td>{order.orderStatus}</td>
                        <td>{order.dispatchedDate}</td>
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
                        type="string"
                        name="email"
                        onChange={e => this.setState({ email: e.target.value })}
                    />
                </label>
                <input type="submit" value="Submit" />
            </form>
            <div id="orders">
                {Orders.renderOrdersTable(this.state.orders)}
            </div>
        </div>);
    }

   
   
}
