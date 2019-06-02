import * as React from 'react';
import axios from 'axios';
import { RouteComponentProps, RouteProps } from 'react-router';
import { MessageQueue } from '';

interface Client {
    id: number;
    name: string;
    email: string;
    address: string; 
}
interface Book {
    id: number;
    title: string;
    price: number;
    amount: number;
}
interface Order {
    guid: number; 
    bookId: number ;
    quantity: number;
    orderStatus: number;
    orderType: number;
    clientId: number;
}


interface FetchDataExampleState {
    book: Book;
    orderid: number;
    name: string;
    email: string;
    address: string;
    quantity: number;
    client: Client;
}

export class CreateOrder extends React.Component<RouteComponentProps<any>, FetchDataExampleState> {
    constructor(props:any) {
        super(props);

        this.state = {
            book:
            {
                id: parseInt(this.props.match.params.id),
                title:'',
                price:0,
                amount:0
            },
            orderid:0,
            name: '',
            email: '',
            address: '',
            quantity: 0, 
            client: {
                id: 0,
                name: '',
                email: '',
                address: ''
            }
        };
    }
    
    componentDidMount() {
        console.log(this.props.match.params.id)
        axios.request<Book>({
            url: 'http://localhost:2222/api/Book/GetBook?id=' + this.state.book.id,
        }).then((response) => {
            this.setState({ book: response.data })
            console.log(response.data);
        })
    }


    postOrder(e: any) {
        e.preventDefault();
        axios.request<Client>({
            url: 'http://localhost:2222/api/Client/GetClientByEmail?email=' + this.state.email
        })
            .then((response) => {
                if (response.status == 200) {
                    this.setState({client: response.data })
                }
                else {
                    axios.post('http://localhost:2222/api/Client/CreateClient', { email: this.state.email, address: this.state.address, name: this.state.name })
                        .then(() => {
                            axios.request<Client>({
                                url: 'http://localhost:2222/api/Client/GetClientByEmail?email=' + this.state.email,
                            }).then((response) => {
                                this.setState({ client: response.data })
                            })
                        })
                }
            }
        ).then(() => {
            if (this.state.quantity > this.state.book.amount) {
                axios.post('http://localhost:2222/api/Order/CreateOrder', {
                    bookId: this.props.match.params.id, clientId: this.state.client.id, quantity: this.state.quantity, orderStatus: 0, orderType: 0 //ver state
                }).then((response) => {
                    this.setState({ orderid: response.data.guid })
                })
                MessageQueue.sendMessageToWarehouse(this.state.book.title, this.state.quantity, this.state.orderid) //verMessageQueue
            }
            else
            {
                axios.post('http://localhost:2222/api/Order/CreateOrder', {
                    bookId: this.props.match.params.id, clientId: this.state.client.id, quantity: this.state.quantity, orderStatus: 0, orderType: 0  //ver state e dispatcheddate
                });
                axios.put('http://localhost:2222/api/Book/EditBook', { id: this.state.book.id, title: this.state.book.title, amount: this.state.book.amount - this.state.quantity, price: this.state.book.price });
            }
            
            })
    }

  

    public render() {
        return <div>
            <h1> Create Order</h1>
            <h3> Title: {this.state.book.title}</h3>
            <h4> Stock: {this.state.book.amount}</h4>
            <h4> Price: {this.state.book.price}</h4>
            <form onSubmit={e => this.postOrder(e)}>              
                <br/>
                <label>
                    Quantity:
                    <input
                        type="number"
                        name="quantity"
                        onChange={e => this.setState({ quantity: parseInt(e.target.value) })}
                    />
                </label>
                <br/>
                <label>
                    Name:
                    <input
                        type="string"
                        name="name"
                        onChange={e => this.setState({ name: e.target.value })}
                    />
                </label>
                <br/>
                <label>
                    Email:
                    <input
                        type="string"
                        name="email"
                        onChange={e => this.setState({ email: e.target.value })}
                    />
                </label>
                <br/>
                <label>
                    Address:
                    <input
                        type="string"
                        name="address"
                        onChange={e => this.setState({ address: e.target.value })}
                    />
                </label>
                <br/>
                <input type="submit" value="Submit" />
            </form>
        </div>
            ;
    }
}