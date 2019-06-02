import * as React from 'react';
import axios from 'axios';
import { RouteComponentProps } from 'react-router';
import { Link } from 'react-router-dom';

interface Book {
    id: number; 
    title: string; 
    price: number; 
    amount: number; 
}

interface FetchDataExampleState {
    books: Book[];
}

export class Home extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();

        this.state = {
            books:[],
        };
    }

    componentDidMount() {
        axios.request<Book[]>({
            url: 'http://localhost:2222/api/Book/GetBooks',
        }).then((response) => {
            this.setState({ books: response.data })
            console.log(response.data);
        })
    }

    public render() {
        return <div>
            <h1> Catalog </h1>
            <table className='table'>
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Price</th>
                        <th>Stock</th>
                        <th> </th>
                    </tr>
                </thead>
                <tbody>
                    {this.state.books.map(book =>
                        <tr key={book.id}>
                            <td>{book.title}</td>
                            <td>{book.price}</td>
                            <td>{book.amount}</td>
                            <td><Link to={`/createOrder/${book.id}`}> Create Order </Link></td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;
    }
}
